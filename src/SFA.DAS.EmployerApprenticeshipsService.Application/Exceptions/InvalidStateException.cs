using System;

namespace SFA.DAS.EmployerCommitments.Application.Exceptions
{
    public class InvalidStateException : Exception
    {
        public InvalidStateException(string message) : base(message) { }
    }
}