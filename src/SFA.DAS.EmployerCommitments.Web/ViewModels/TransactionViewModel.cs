using System;
using SFA.DAS.EmployerCommitments.Domain.Models.Transaction;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public class TransactionViewModel   
    {
        public decimal CurrentBalance { get; set; }
        public DateTime CurrentBalanceCalcultedOn { get; set; }
        public AggregationData Data { get; set; }
    }
}