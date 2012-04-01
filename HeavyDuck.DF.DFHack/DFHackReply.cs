using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dfproto;

namespace HeavyDuck.DF.DFHack
{
    public class DFHackReply<T>
    {
        internal DFHackReply(T data, DFHackCommandResult code, List<CoreTextNotification> notifications)
        {
            this.Data = data;
            this.ResultCode = code;
            this.Notifications = notifications;
        }

        public T Data { get; private set; }
        public DFHackCommandResult ResultCode { get; private set; }
        public List<CoreTextNotification> Notifications { get; private set; }
    }
}
