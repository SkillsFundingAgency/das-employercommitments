using System;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships
{
    public class PriceHistoryViewModel
    {
        public long ApprenticeshipId { get; set; }

        public decimal Cost { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}