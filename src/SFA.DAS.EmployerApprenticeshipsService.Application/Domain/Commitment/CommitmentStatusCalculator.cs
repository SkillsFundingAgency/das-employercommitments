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
            bool hasApprenticeships = apprenticeshipCount > 0;

            if (transferSenderId.HasValue)
            {
                //if (!transferApprovalStatus.HasValue)
                //    throw new InvalidStateException("TransferSenderId supplied, but no TransferApprovalStatus");
                return GetTransferStatus(editStatus, transferApprovalStatus, lastAction, hasApprenticeships);
            }

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

        private RequestStatus GetTransferStatus(EditStatus edit, TransferApprovalStatus? transferApproval, LastAction lastAction, bool hasApprenticeships)
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
                                return RequestStatus.NewRequest;
                            case EditStatus.ProviderOnly:
                                return GetProviderOnlyStatus(lastAction, hasApprenticeships);
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
                    return RequestStatus.RejectedBySender;

                default:
                    throw new Exception("Unexpected TransferApprovalStatus");
            }
        }
    }
}