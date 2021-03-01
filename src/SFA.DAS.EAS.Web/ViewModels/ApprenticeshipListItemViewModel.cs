using System;
using System.Collections.Generic;
using SFA.DAS.Commitments.Api.Types.TrainingProgramme;
using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.EmployerCommitments.Application.Extensions;
using SFA.DAS.EmployerCommitments.Domain.Extensions;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Web.Extensions;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public sealed class ApprenticeshipListItemViewModel
    {
        public string ApprenticeName { get; set; }
        public DateTime? ApprenticeDateOfBirth { get; set; }

        public string TrainingCode { get; set; }

        public string TrainingName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public decimal? Cost { get; set; }

        public string HashedApprenticeshipId { get; set; }

        public bool CanBeApproved { get; set; }

        public IEnumerable<OverlappingApprenticeship> OverlappingApprenticeships { get; set; }

        public string ApprenticeUln { get; set; }

        public bool IsOverFundingLimit(TrainingProgramme trainingProgramme)
        {
            if (trainingProgramme == null)
                return false;

            if (!StartDate.HasValue)
                return false;

            var fundingCapAtStartDate = trainingProgramme.FundingCapOn(StartDate.Value);
            return Cost.HasValue && fundingCapAtStartDate > 0
                                 && Cost > fundingCapAtStartDate;
        }
    }
}