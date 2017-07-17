using FluentValidation.Attributes;
using SFA.DAS.EmployerCommitments.Web.Validators;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    [Validator(typeof(DeleteApprenticeshipConfirmationViewModelValidator))]
    public class DeleteApprenticeshipConfirmationViewModel
    {
        public string HashedAccountId { get; set; }
        public string HashedCommitmentId { get; set; }
        public string HashedApprenticeshipId { get; set; }
        public bool? DeleteConfirmed { get; set; }
        public string ApprenticeshipName { get; set; }
        public string DateOfBirth { get; set; }
    }
}