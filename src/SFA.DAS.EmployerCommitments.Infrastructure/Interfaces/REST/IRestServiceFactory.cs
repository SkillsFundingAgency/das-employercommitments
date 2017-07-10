namespace SFA.DAS.EmployerCommitments.Infrastructure.Interfaces.REST
{
    public interface IRestServiceFactory
    {
        IRestService Create(string baseUrl);
    }
}