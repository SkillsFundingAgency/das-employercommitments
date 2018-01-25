using System;
using FluentValidation.Attributes;
using SFA.DAS.EmployerCommitments.Web.Validators;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships
{
    [Validator(typeof(EditStopDateViewModelValidator))]
    public class EditStopDateViewModel : ViewModelBase
    {
        public DateTimeViewModel NewStopDate { get; set; }
    }
}