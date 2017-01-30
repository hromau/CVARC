using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVARC.Core.Controllers.Network
{
    public class KickOutException : Exception
    {
        public KickOutException() : base("You have been disconnected from the server") {  }

        public KickOutException(string message) : base(message) { }

        public KickOutException(string message, Exception inner) : base(message, inner) { }
    }
}
