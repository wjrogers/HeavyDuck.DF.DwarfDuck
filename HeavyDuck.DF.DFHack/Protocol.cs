using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HeavyDuck.DF.DFHack
{
    public enum DFHackCommandResult
    {
        CR_LINK_FAILURE = -3,    // RPC call failed due to I/O or protocol error
        CR_NEEDS_CONSOLE = -2,   // Attempt to call interactive command without console
        CR_NOT_IMPLEMENTED = -1, // Command not implemented, or plugin not loaded
        CR_OK = 0,               // Success
        CR_FAILURE = 1,          // Failure
        CR_WRONG_USAGE = 2,      // Wrong arguments or ui state
        CR_NOT_FOUND = 3         // Target object not found (for RPC mainly)
    }

    public enum DFHackReplyCode : short
    {
        RPC_REPLY_RESULT = -1,
        RPC_REPLY_FAIL = -2,
        RPC_REPLY_TEXT = -3,
        RPC_REQUEST_QUIT = -4
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct RPCHandshakeHeader
    {
        public const int PROTOCOL_VERSION = 1;

        static RPCHandshakeHeader()
        {
            // we must explicitly get the encoded bytes because the char[] is not terminated
            REQUEST_MAGIC = Encoding.ASCII.GetBytes("DFHack?\n");
            RESPONSE_MAGIC = Encoding.ASCII.GetBytes("DFHack!\n");
        }

        public static RPCHandshakeHeader GetRequest()
        {
            return new RPCHandshakeHeader
            {
                magic = REQUEST_MAGIC,
                version = PROTOCOL_VERSION,
            };
        }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] magic;
        public int version;

        public bool IsRequestMagic
        {
            get { return this.magic.SequenceEqual(REQUEST_MAGIC); }
        }

        public bool IsResponseMagic
        {
            get { return this.magic.SequenceEqual(RESPONSE_MAGIC); }
        }

        public static readonly byte[] REQUEST_MAGIC;
        public static readonly byte[] RESPONSE_MAGIC;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct RPCMessageHeader
    {
        public const int MAX_MESSAGE_SIZE = 8 * 1048756;

        public short id;
        public int size;
    }
}
