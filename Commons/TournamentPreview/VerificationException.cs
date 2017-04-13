using System;
using System.Runtime.Serialization;

namespace TournamentPreview
{
    [Serializable]
    internal class VerificationException : Exception
    {

        public VerificationException(string message) : base(message)
        {
        }
        
    }
}