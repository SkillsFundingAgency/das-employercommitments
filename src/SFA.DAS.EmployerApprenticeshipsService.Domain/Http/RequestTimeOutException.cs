namespace SFA.DAS.EmployerCommitments.Domain.Http
{
    public class RequestTimeOutException : HttpException
    {
        public RequestTimeOutException() : base(408, "Request has time out")
        {
        }
    }
}