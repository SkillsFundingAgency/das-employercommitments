using System;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Exceptions;

namespace SFA.DAS.EmployerCommitments.Application.Domain.Commitment
{
    internal sealed class CommitmentStatusCalculator
    {
        public RequestStatus GetStatus(EditStatus editStatus, int apprenticeshipCount, LastAction lastAction, AgreementStatus? overallAgreementStatus, long? transferSenderId, TransferApprovalStatus? transferApprovalStatus)
        {
            if (transferSenderId.HasValue)
            {
                return GetTransferStatus(editStatus, transferApprovalStatus, lastAction,  overallAgreementStatus);
            }

            if (editStatus == EditStatus.Both)
                return RequestStatus.Approved;

            if (editStatus == EditStatus.ProviderOnly)
                return GetProviderOnlyStatus(lastAction);

            if (editStatus == EditStatus.EmployerOnly)
                return GetEmployerOnlyStatus(lastAction, overallAgreementStatus);

            return RequestStatus.None;
        }

        private static RequestStatus GetProviderOnlyStatus(LastAction lastAction)
        {
            if (lastAction == LastAction.Amend)
                return RequestStatus.SentForReview;

            if (lastAction == LastAction.Approve)
                return RequestStatus.WithProviderForApproval;

            return RequestStatus.None;
        }

        private RequestStatus GetEmployerOnlyStatus(LastAction lastAction, AgreementStatus? overallAgreementStatus)
        {
            if (lastAction == LastAction.None)
                return RequestStatus.NewRequest;

            // LastAction.Approve > LastAction.Amend, but then AgreementStatus >= ProviderAgreed, so no need for > on LastAction??
            if (lastAction >= LastAction.Amend && overallAgreementStatus == AgreementStatus.NotAgreed)
                return RequestStatus.ReadyForReview;

            if (lastAction == LastAction.Approve)
                return RequestStatus.ReadyForApproval;

            return RequestStatus.None;
        }

        private RequestStatus GetTransferStatus(EditStatus edit, TransferApprovalStatus? transferApproval, LastAction lastAction, AgreementStatus? overallAgreementStatus)
        {
            const string invalidStateExceptionMessagePrefix = "Transfer funder commitment in invalid state: ";

            if (edit >= EditStatus.Neither)
                throw new Exception("Unexpected EditStatus");

            switch (transferApproval ?? TransferApprovalStatus.Pending)
            {
                case TransferApprovalStatus.Pending:
                    {
                        switch (edit)
                        {
                            case EditStatus.Both:
                                return RequestStatus.WithSenderForApproval;
                            case EditStatus.EmployerOnly:
                                //todo: need to set to draft after rejected by sender and edited by receiver (but not sent to provider)
                                return GetEmployerOnlyStatus(lastAction, overallAgreementStatus);
                            case EditStatus.ProviderOnly:
                                return GetProviderOnlyStatus(lastAction);
                            default:
                                throw new Exception("Unexpected EditStatus");
                        }
                    }

                case TransferApprovalStatus.Approved:
                    if (edit != EditStatus.Both)
                        throw new InvalidStateException($"{invalidStateExceptionMessagePrefix}If approved by sender, must be approved by receiver and provider");
                    return RequestStatus.None;

                case TransferApprovalStatus.Rejected:
                    if (edit != EditStatus.EmployerOnly)
                        throw new InvalidStateException($"{invalidStateExceptionMessagePrefix}If just rejected by sender, must be with receiver");
                    return RequestStatus.ReadyForReview;

                default:
                    throw new Exception("Unexpected TransferApprovalStatus");
            }
        }
    }
}