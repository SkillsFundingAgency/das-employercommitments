using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers
{
    public interface IApprovedApprenticeshipMapper
    {
        ApprovedApprenticeshipViewModel Map(Commitments.Api.Types.ApprovedApprenticeship.ApprovedApprenticeship source);
    }
}
