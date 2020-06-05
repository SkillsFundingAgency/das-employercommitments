using System;
using FluentValidation.Attributes;
using SFA.DAS.EmployerCommitments.Web.Validators;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships
{
    [Validator(typeof(EditApprenticeshipStopDateViewModelValidator))]
    public sealed class EditApprenticeshipStopDateViewModel : ViewModelBase
    {
        public string ApprenticeshipULN { get; set; }

        public string ApprenticeshipHashedId { get; set; }
        public string ApprenticeshipName { get; set; }

        public DateTime EarliestDate { get; set; }

        public DateTime CurrentStopDate { get; set; }
        public DateTime? AcademicYearRestriction { get; set; }
        public DateTime ApprenticeshipStartDate { get; set; }
        public bool EarliestDateIsStartDate => EarliestDate.Equals(NewStopDate.DateTime);

        public DateTimeViewModel NewStopDate { get; set; }

        
    }
}