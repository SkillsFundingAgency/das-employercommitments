﻿using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Application.Domain.Commitment;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public class CommitmentViewModel
    {
        public string HashedId { get; set; }
        public string Name { get; set; }
        public string LegalEntityName { get; set; }
        public string ProviderName { get; set; }
    }

    public sealed class CommitmentDetailsViewModel : CommitmentViewModel
    {
        public RequestStatus Status { get; set; }
        public bool ShowApproveOnlyOption { get; set; }
        public string LatestMessage { get; set; }
        public bool HasApprenticeships { get; set; }
        public IList<ApprenticeshipListItemViewModel> Apprenticeships { get; set; }
        public IList<ApprenticeshipListItemGroupViewModel> ApprenticeshipGroups { get; set; }
        public string BackLinkUrl { get; set; }
        public bool HasOverlappingErrors { get; set; }
        public Dictionary<string, string> Warnings { get; set; }
        public bool IsReadOnly { get; set; }
        public Dictionary<string, string> Errors { get; set; }
        public string PageTitle { get; set; }
    }
}