using SFA.DAS.EmployerCommitments.Domain.Models.FeatureToggle;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IFeatureToggle
    {
        FeatureToggleLookup GetFeatures();
    }
}
