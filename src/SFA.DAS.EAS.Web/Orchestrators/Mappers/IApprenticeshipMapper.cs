using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers
{
    public interface IApprenticeshipMapper
    {
        Task<Apprenticeship> MapFrom(ApprenticeshipViewModel viewModel);

        ApprenticeshipDetailsViewModel MapToApprenticeshipDetailsViewModel(Apprenticeship apprenticeship, bool disableUlnReuseCheck=false);

        ApprenticeshipViewModel MapToApprenticeshipViewModel(Apprenticeship apprenticeship, CommitmentView commitment);

        Task<UpdateApprenticeshipViewModel> CompareAndMapToApprenticeshipViewModel(Apprenticeship original, ApprenticeshipViewModel edited);

        ApprenticeshipUpdate MapFrom(UpdateApprenticeshipViewModel viewModel);

        UpdateApprenticeshipViewModel MapFrom(ApprenticeshipUpdate apprenticeshipUpdate);

        IList<PriceChange> MapPriceChanges(IEnumerable<DataLockStatus> dataLockWithOnlyPriceMismatch, List<PriceHistory> history);

        Task<IEnumerable<CourseChange>> MapCourseChanges(IEnumerable<DataLockStatus> dataLocks, Apprenticeship apprenticeship, IList<PriceHistory> priceHistory);

        EditApprenticeshipStopDateViewModel MapToEditApprenticeshipStopDateViewModel(Apprenticeship apprenticeship);
    }
}