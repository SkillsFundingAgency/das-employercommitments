using FluentValidation.Attributes;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    [Validator(typeof(TransferConfirmationViewModel))]
    public class TransferConfirmationViewModel
    {
        public string TransferApprovalStatus { get; set; }
        public string TransferReceiverName { get; set; }
        public string UrlAddress { get; set; }
    }
}