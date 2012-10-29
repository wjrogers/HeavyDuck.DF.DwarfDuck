using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using dfproto;
using Google.ProtocolBuffers;
using log4net;

namespace HeavyDuck.DF.DFHack
{
    /// <summary>
    /// Provides an friendly interface to the DFHack protobuf API.
    /// </summary>
    public class DFHackClient : IDisposable
    {
        private const int DEFAULT_TIMEOUT = 2000;
        private const short MESSAGE_ID_BIND = 0;

        private static readonly ILog m_logger = LogManager.GetLogger(typeof(DFHackClient));

        private readonly Dictionary<Tuple<string, string, string, string>, short> m_bindings
            = new Dictionary<Tuple<string, string, string, string>, short>();
        private readonly byte[] m_buffer_proto = new byte[RPCMessageHeader.MAX_MESSAGE_SIZE];
        private readonly byte[] m_buffer_header = new byte[Marshal.SizeOf(typeof(RPCMessageHeader))];
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
                ReadBytes(stream, buffer, buffer.Length);

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

        public DFHackReply<T> Receive<T>(Func<ByteString, T> parser) where T : class, IMessageLite
        {
            T data;
            ByteString chunk;
            CoreTextNotification notification;
            var stream = m_client.GetStream();
            var text = new List<CoreTextNotification>();
            RPCMessageHeader header;

            while (true)
            {
                // read a message header from the stream
                ReadBytes(stream, m_buffer_header, m_buffer_header.Length);
                header = ConvertBytesToStruct<RPCMessageHeader>(m_buffer_header);

                // interpret the header
                switch (header.id)
                {
                    case (short)DFHackReplyCode.RPC_REPLY_TEXT:
                        ReadBytes(stream, m_buffer_proto, header.size);

                        // parse a text notification message from the stream
                        chunk = ByteString.CopyFrom(m_buffer_proto, 0, header.size);
                        notification = CoreTextNotification.ParseFrom(chunk);
                        text.Add(notification);

                        break;
                    case (short)DFHackReplyCode.RPC_REPLY_RESULT:
                        ReadBytes(stream, m_buffer_proto, header.size);

                        // use the provided parser to read the data message
                        chunk = ByteString.CopyFrom(m_buffer_proto, 0, header.size);
                        data = parser(chunk);
                        return new DFHackReply<T>(data, DFHackCommandResult.CR_OK, text);

                    case (short)DFHackReplyCode.RPC_REPLY_FAIL:
                        return new DFHackReply<T>(null, (DFHackCommandResult)header.size, text);
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

        public DFHackReply<GetWorldInfoOut> GetWorldInfo()
        {
            var id = GetBinding(
                MethodInfo.GetCurrentMethod().Name,
                typeof(EmptyMessage).FullName,
                typeof(GetWorldInfoOut).FullName);
            var data_request = new EmptyMessage.Builder();

            // send the message
            Send(id, data_request.Build());
            
            // get the reply
            return Receive(GetWorldInfoOut.ParseFrom);
        }

        public DFHackReply<ListEnumsOut> ListEnums()
        {
            var id = GetBinding(
                MethodInfo.GetCurrentMethod().Name,
                typeof(EmptyMessage).FullName,
                typeof(ListEnumsOut).FullName);
            var data_request = new EmptyMessage.Builder();

            // send the message
            Send(id, data_request.Build());
            
            // get the reply
            return Receive(ListEnumsOut.ParseFrom);
        }

        public DFHackReply<ListJobSkillsOut> ListJobSkills()
        {
            var id = GetBinding(
                MethodInfo.GetCurrentMethod().Name,
                typeof(EmptyMessage).FullName,
                typeof(ListJobSkillsOut).FullName);
            var data_request = new EmptyMessage.Builder();

            // send the message
            Send(id, data_request.Build());
            
            // get the reply
            return Receive(ListJobSkillsOut.ParseFrom);
        }

        public DFHackReply<ListUnitsOut> ListUnits()
        {
            var id = GetBinding(
                MethodInfo.GetCurrentMethod().Name,
                typeof(ListUnitsIn).FullName,
                typeof(ListUnitsOut).FullName);
            var data_request = new ListUnitsIn.Builder()
            {
                ScanAll = true,
                Mask = new BasicUnitInfoMask.Builder()
                {
                    Labors = true,
                    Profession = true,
                    Skills = true,
                    MiscTraits = true,
                }.Build(),
            };

            // send the message
            Send(id, data_request.Build());
            
            // get the reply
            return Receive(ListUnitsOut.ParseFrom);
        }

        public DFHackReply<EmptyMessage> SetUnitLabors(SetUnitLaborsIn @in)
        {
            var id = GetBinding(
                MethodInfo.GetCurrentMethod().Name,
                typeof(SetUnitLaborsIn).FullName,
                typeof(EmptyMessage).FullName);

            // send the message
            Send(id, @in);
            
            // get the reply
            return Receive(EmptyMessage.ParseFrom);
        }

        #endregion

        # region Private Helper Methods

        /// <summary>
        /// Reads a specific number of bytes from a stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="buffer">The buffer to read into.</param>
        /// <param name="count">The number of bytes to read.</param>
        private static void ReadBytes(Stream input, byte[] buffer, int count)
        {
            for (int read = 0; read < count; )
                read += input.Read(buffer, read, count - read);
        }

        /// <summary>
        /// Marshals a structure into an unmanaged byte array.
        /// </summary>
        private static byte[] ConvertStructToBytes<T>(T value) where T : struct
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
        private static T ConvertBytesToStruct<T>(byte[] buffer) where T : struct
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
