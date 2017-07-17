using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.Commitments.Api.Types.ProviderPayment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers
{
    public interface IApprenticeshipMapper
    {
        Task<Apprenticeship> MapFrom(ApprenticeshipViewModel viewModel);

        ApprenticeshipDetailsViewModel MapToApprenticeshipDetailsViewModel(Apprenticeship apprenticeship);

        ApprenticeshipViewModel MapToApprenticeshipViewModel(Apprenticeship apprenticeship);

        Task<UpdateApprenticeshipViewModel> CompareAndMapToApprenticeshipViewModel(Apprenticeship original, ApprenticeshipViewModel edited);

        Dictionary<string, string> MapOverlappingErrors(GetOverlappingApprenticeshipsQueryResponse overlappingErrors);

        ApprenticeshipUpdate MapFrom(UpdateApprenticeshipViewModel viewModel);

        UpdateApprenticeshipViewModel MapFrom(ApprenticeshipUpdate apprenticeshipUpdate);

        PaymentOrderViewModel MapPayment(IList<ProviderPaymentPriorityItem> data);

        IList<PriceChange> MapPriceChanges(IEnumerable<DataLockStatus> dataLockWithOnlyPriceMismatch, List<PriceHistory> history);
    }
}