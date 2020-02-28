using FluentValidation.Attributes;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EmployerCommitments.Web.Validators;
using System;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships
{
    [Validator(typeof(ChangeStatusViewModelValidator))]
    public sealed class ChangeStatusViewModel
    {
        public ChangeStatusViewModel()
        {
            DateOfChange = new DateTimeViewModel();
        }

        public ChangeStatusType? ChangeType { get; set; }

        public WhenToMakeChangeOptions WhenToMakeChange { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTimeViewModel DateOfChange { get; set; }

        public bool? ChangeConfirmed { get; set; }

        public bool AcademicYearBreakInTraining { get; set; }

        public DateTimeViewModel PauseDate { get; set; }
    }
}