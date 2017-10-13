using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentValidation.Attributes;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Web.Validators;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships
{
    [Validator(typeof(DataLockStatusViewModelViewModelValidator))]
    public class DataLockStatusViewModel : ViewModelBase
    {
        public ITrainingProgramme CurrentProgram { get; set; }

        public ITrainingProgramme IlrProgram { get; set; }

        public DateTime? PeriodStartData { get; set; }

        public string HashedAccountId { get; set; }

        public string HashedApprenticeshipId { get; set; }

        public string ProviderName { get; set; }

        public string LearnerName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string ChangesConfirmedError => GetErrorMessage(nameof(ChangesConfirmed));

        public bool? ChangesConfirmed { get; set; }

        public IList<PriceChange> PriceChanges { get; set; }

        public IEnumerable<CourseChange> CourseChanges { get; set; }

        public int TotalChanges => PriceChanges?.Count ?? 0 + CourseChanges?.Count() ?? 0;

    }
}