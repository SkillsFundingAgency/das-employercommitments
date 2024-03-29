﻿using System;
using SFA.DAS.EmployerCommitments.Domain.Models.UserProfile;

namespace SFA.DAS.EmployerCommitments.Domain.Models.AccountTeam
{
    public class Invitation
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime ExpiryDate { get; set; }
        public InvitationStatus Status { get; set; }
        public Role RoleId { get; set; }
    }
}