using System;
using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Domain.Models.Payments;

namespace SFA.DAS.EmployerCommitments.Application.Queries.FindAccountProviderPayments
{
    public class FindAccountProviderPaymentsResponse
    {
        public string ProviderName { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime DateCreated { get; set; }
        public List<PaymentTransactionLine> Transactions { get; set; }
        public decimal Total { get; set; }
    }
}