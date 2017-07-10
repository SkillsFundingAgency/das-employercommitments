namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IUserWhiteList
    {
        bool IsEmailOnWhiteList(string email);
    }
}
