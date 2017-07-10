namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IHashingService
    {
        string HashValue(long id);
        long DecodeValue(string id);
    }
}
