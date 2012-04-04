using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeavyDuck.DF.DFHack
{
    /// <summary>
    /// The exception thrown when there is an error communicating with DFHack.
    /// </summary>
    public class DFProtocolException : Exception
    {
        public DFProtocolException(string message) : this(message, null) { }
        public DFProtocolException(string message, DFHackCommandResult code) : this(message) { this.Code = code; }
        public DFProtocolException(string message, Exception innerException) : base(message, innerException)  { this.Code = DFHackCommandResult.CR_FAILURE; }

        public DFHackCommandResult Code { get; private set; }
    }
}
