using System;

namespace SFA.DAS.EmployerCommitments.Web.Exceptions
{
    public class InvalidStateException : Exception
    {
        public InvalidStateException(string message) : base(message) { }
    }
}