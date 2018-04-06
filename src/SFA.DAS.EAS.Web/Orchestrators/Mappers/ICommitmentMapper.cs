﻿using System;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers
{
    public interface ICommitmentMapper
    {
        CommitmentViewModel MapToCommitmentViewModel(CommitmentView commitment);
        Task<CommitmentListItemViewModel> MapToCommitmentListItemViewModelAsync(CommitmentListItem commitment, Func<CommitmentListItem, Task<string>> latestMessageFunc);
        TransferCommitmentViewModel MapToTransferCommitmentViewModel(CommitmentView commitment);
        TransferRequestViewModel MapToTransferRequestViewModel(TransferRequest transferRequest);
    }
}