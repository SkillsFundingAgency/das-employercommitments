namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IConfiguration
    {
        string DatabaseConnectionString { get; set; }

        string ServiceBusConnectionString { get; set; }
    }
}