using FeatureToggle;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IFeatureToggleService
    {
        IFeatureToggle Get<T>() where T : SimpleFeatureToggle, new();
    }
}