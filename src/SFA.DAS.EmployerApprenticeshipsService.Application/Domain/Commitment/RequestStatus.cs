using System.ComponentModel;

namespace SFA.DAS.EmployerCommitments.Application.Domain.Commitment
{
    public enum RequestStatus
    {
        None, // No use here.

        // New request or back with receiver/provider after rejection by sender, i.e. Draft status
        [Description("New request")]
        NewRequest,

        [Description("Sent to provider")]
        SentToProvider,

        [Description("Sent for review")]
        SentForReview,

        [Description("Ready for review")]
        ReadyForReview,

        [Description("With Provider for approval")]
        WithProviderForApproval,

        [Description("Ready for approval")]
        ReadyForApproval,

        [Description("Approved")]
        Approved,

        [Description("With sender for approval")]
        WithSenderForApproval,

        [Description("Rejected by sender")]
        RejectedBySender
    }
}