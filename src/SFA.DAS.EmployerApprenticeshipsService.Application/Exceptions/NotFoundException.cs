using System;

namespace SFA.DAS.EmployerCommitments.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public string ErrorMessage { get; set; }

        public NotFoundException(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }   
    }
}