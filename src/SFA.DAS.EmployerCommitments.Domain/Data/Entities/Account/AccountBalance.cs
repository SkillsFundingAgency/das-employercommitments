﻿namespace SFA.DAS.EmployerCommitments.Domain.Data.Entities.Account
{
    public class AccountBalance
    {
        public long AccountId { get; set; }
        public decimal Balance { get; set; }
        public int IsLevyPayer { get; set; }
    }
}