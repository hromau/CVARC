using System;

namespace Pudge.GameExceptions
{
    public class PudgeIsDeadException : Exception
    {
        private new string Message{ get; set; }
        public PudgeIsDeadException(string message)
        {
            Message = message;
        }
        public override string ToString()
        {
            return Message;
        }
    }
}