using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using dfproto;
using Google.ProtocolBuffers;
using log4net;

namespace HeavyDuck.DF.DFHack
{
    public class DFHackClient : IDisposable
    {
        private const int DEFAULT_TIMEOUT = 2000;
        private const short MESSAGE_ID_BIND = 0;

        private static readonly ILog m_logger = LogManager.GetLogger(typeof(DFHackClient));

        private readonly Dictionary<Tuple<string, string, string, string>, short> m_bindings
            = new Dictionary<Tuple<string, string, string, string>, short>();
        private TcpClient m_client;

        /// <summary>
        /// Close the connection and free resources used by the client.
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// Connect to the DFHack server.
        /// </summary>
        public void Open()
        {
            TcpClient nascent = null;

            // attempt to open the connection and perform the handshake
            try
            {
                // connect
                nascent = new TcpClient("localhost", 5000);

                // grab the handshake request and the stream
                var buffer = ConvertStructToBytes(RPCHandshakeHeader.GetRequest());
                var stream = nascent.GetStream();

                // send the handshake
                stream.Write(buffer, 0, buffer.Length);

                // read the response
                stream.ReadTimeout = DEFAULT_TIMEOUT;
                stream.Read(buffer, 0, buffer.Length);

                // check response magic
                var response = ConvertBytesToStruct<RPCHandshakeHeader>(buffer);
                if (response.version != RPCHandshakeHeader.PROTOCOL_VERSION)
                    throw new DFProtocolException("Protocol version mismatch.");
                if (!response.IsResponseMagic)
                    throw new DFProtocolException("Expected reponse magic, got " + Encoding.ASCII.GetString(response.magic));
            }
            catch (Exception ex)
            {
                m_logger.Error("Connection failed.", ex);
                if (nascent != null)
                    nascent.Close();

                throw;
            }

            // connection succeeded, keep the client
            m_client = nascent;
        }

        /// <summary>
        /// Close the connection and free resources used by the client.
        /// </summary>
        public void Close()
        {
            // silently ignore if we are already closed
            if (m_client == null) return;

            try
            {
                // send the quit message
                Send((short)DFHackReplyCode.RPC_REQUEST_QUIT);
            }
            catch (Exception ex)
            {
                m_logger.Error("Could not close the connection cleanly.", ex);
            }
            finally
            {
                m_client.Close();
                m_client = null;
            }
        }

        /// <summary>
        /// Sends a message to the server.
        /// </summary>
        /// <param name="id">The RPC message id.</param>
        /// <param name="data">The protobufs data, if any.</param>
        public void Send(short id, IMessageLite data = null)
        {
            var stream = m_client.GetStream();
            var header = ConvertStructToBytes(new RPCMessageHeader
            {
                id = id,
                size = (data == null ? 0 : data.SerializedSize),
            });

            stream.Write(header, 0, header.Length);

            if (data != null)
                data.WriteTo(stream);
        }

        public DFHackReply<T> Receive<T>(Func<ByteString, T> parser)
        {
            ByteString chunk;
            CoreTextNotification notification;
            var buffer_proto = new byte[RPCMessageHeader.MAX_MESSAGE_SIZE];
            var buffer = new byte[Marshal.SizeOf(typeof(RPCMessageHeader))];
            var stream = m_client.GetStream();
            var text = new List<CoreTextNotification>();
            RPCMessageHeader header;

            while (true)
            {
                // read a message header from the stream
                if (buffer.Length != stream.Read(buffer, 0, buffer.Length))
                    throw new DFProtocolException("Could not read header, not enough data in the stream");
                header = ConvertBytesToStruct<RPCMessageHeader>(buffer);

                // interpret the header
                switch (header.id)
                {
                    case (short)DFHackReplyCode.RPC_REPLY_TEXT:
                        if (header.size != stream.Read(buffer_proto, 0, header.size))
                            throw new DFProtocolException("Could not read enough bytes");

                        // parse a text notification  message from the stream
                        chunk = ByteString.CopyFrom(buffer_proto, 0, header.size);
                        notification = CoreTextNotification.ParseFrom(chunk);
                        text.Add(notification);

                        break;
                    case (short)DFHackReplyCode.RPC_REPLY_RESULT:
                        if (header.size != stream.Read(buffer_proto, 0, header.size))
                            throw new DFProtocolException("Could not read enough bytes");

                        // parse a text notification  message from the stream
                        chunk = ByteString.CopyFrom(buffer_proto, 0, header.size);
                        return new DFHackReply<T>(parser(chunk), DFHackCommandResult.CR_OK, text);

                    case (short)DFHackReplyCode.RPC_REPLY_FAIL:
                        throw new DFProtocolException("RPC failed", (DFHackCommandResult)header.size);
                }
            }
        }

        public short GetBinding(string method, string input_msg, string output_msg, string plugin = null)
        {
            var key = Tuple.Create(method, input_msg, output_msg, plugin);
            short id;

            if (!m_bindings.TryGetValue(key, out id))
            {
                var result = BindMethod(method, input_msg, output_msg, plugin);

                id = (short)result.Data.AssignedId;
                m_bindings[key] = id;
            }

            return id;
        }

        #region Public API Methods

        public DFHackReply<CoreBindReply> BindMethod(string method, string input_msg, string output_msg, string plugin = null)
        {
            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentNullException("method");
            if (string.IsNullOrWhiteSpace(input_msg))
                throw new ArgumentNullException("input_msg");
            if (string.IsNullOrWhiteSpace(output_msg))
                throw new ArgumentNullException("output_msg");

            // build the message data
            var data_request = new CoreBindRequest.Builder()
            {
                Method = method,
                InputMsg = input_msg,
                OutputMsg = output_msg,
            };
            if (!string.IsNullOrWhiteSpace(plugin))
                data_request.Plugin = plugin;

            // send it
            Send(MESSAGE_ID_BIND, data_request.Build());

            // receive the reply
            return Receive(CoreBindReply.ParseFrom);
        }

        public DFHackReply<ListUnitsOut> ListUnits()
        {
            var id = GetBinding(
                "ListUnits",
                typeof(ListUnitsIn).FullName,
                typeof(ListUnitsOut).FullName);
            var data_request = new ListUnitsIn.Builder() { ScanAll = true };

            // send the message
            Send(id, data_request.Build());
            
            // get the reply
            return Receive(ListUnitsOut.ParseFrom);
        }

        #endregion

        # region Private Helper Methods

        /// <summary>
        /// Marshals a structure into an unmanaged byte array.
        /// </summary>
        private byte[] ConvertStructToBytes<T>(T value) where T : struct
        {
            var buffer = new byte[Marshal.SizeOf(typeof(T))];
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            try
            {
                Marshal.StructureToPtr(value, handle.AddrOfPinnedObject(), false);
            }
            finally
            {
                handle.Free();
            }

            return buffer;
        }

        /// <summary>
        /// Marshals an unmanaged byte array into a struct.
        /// </summary>
        private T ConvertBytesToStruct<T>(byte[] buffer) where T : struct
        {
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }

        #endregion
    }
}
