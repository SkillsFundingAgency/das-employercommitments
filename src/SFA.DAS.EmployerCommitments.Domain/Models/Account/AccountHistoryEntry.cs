﻿using System;

namespace SFA.DAS.EmployerCommitments.Domain.Models.Account
{
    public class AccountHistoryEntry
    {
        public long AccountId { get; set; }
        public string PayeRef { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateRemoved { get; set; }
    }
}