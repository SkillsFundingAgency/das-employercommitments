using System;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Web.Enums;
using SFA.DAS.EmployerCommitments.Web.Exceptions;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators
{
    public sealed class CommitmentStatusCalculator : ICommitmentStatusCalculator
    {
        // all the consumers of this will eventually need to be updated to take account of transfers
        // we could fold-in GetTransferStatus into GetStatus (adding isTransfer & transferApprovalStatus as params)
        // but if we did that now, it would expand the scope of the current story beyond its boundaries
        // and require work that should be part of future transfer stories
        // but we might want to refactor this later on
        public RequestStatus GetStatus(EditStatus editStatus, int apprenticeshipCount, LastAction lastAction, AgreementStatus? overallAgreementStatus)
        {
            bool hasApprenticeships = apprenticeshipCount > 0;

            if (editStatus == EditStatus.Both)
                return RequestStatus.Approved;

            if (editStatus == EditStatus.ProviderOnly)
                return GetProviderOnlyStatus(lastAction, hasApprenticeships);

            if (editStatus == EditStatus.EmployerOnly)
                return GetEmployerOnlyStatus(lastAction, hasApprenticeships, overallAgreementStatus);

            return RequestStatus.None;
        }

        private static RequestStatus GetProviderOnlyStatus(LastAction lastAction, bool hasApprenticeships)
        {
            if (!hasApprenticeships || lastAction == LastAction.None)
                return RequestStatus.SentToProvider;

            if (lastAction == LastAction.Amend)
                return RequestStatus.SentForReview;

            if (lastAction == LastAction.Approve)
                return RequestStatus.WithProviderForApproval;

            return RequestStatus.None;
        }

        private RequestStatus GetEmployerOnlyStatus(LastAction lastAction, bool hasApprenticeships, AgreementStatus? overallAgreementStatus)
        {
            if (!hasApprenticeships || lastAction == LastAction.None)
                return RequestStatus.NewRequest;

            if (lastAction >= LastAction.Amend && overallAgreementStatus == AgreementStatus.NotAgreed)
                return RequestStatus.ReadyForReview;

            if (lastAction == LastAction.Approve)
                return RequestStatus.ReadyForApproval;

            return RequestStatus.None;
        }

        public RequestStatus GetTransferStatus(EditStatus edit, TransferApprovalStatus transferApproval)
        {
            const string invalidStateExceptionMessagePrefix = "Transfer funder commitment in invalid state: ";

            if (edit >= EditStatus.Neither)
                throw new Exception("Unexpected EditStatus");

            switch (transferApproval)
            {
                case TransferApprovalStatus.Pending:
                    return edit == EditStatus.Both ? RequestStatus.WithSender : RequestStatus.NewRequest;

                case TransferApprovalStatus.Approved:
                    if (edit != EditStatus.Both)
                        throw new InvalidStateException($"{invalidStateExceptionMessagePrefix}If approved by sender, must be approved by receiver and provider");
                    return RequestStatus.None;

                case TransferApprovalStatus.Rejected:
                    if (edit != EditStatus.EmployerOnly)
                        throw new InvalidStateException($"{invalidStateExceptionMessagePrefix}If just rejected by sender, must be with receiver");
                    return RequestStatus.WithSender;

                default:
                    throw new Exception("Unexpected TransferApprovalStatus");
            }
        }
    }
}