using System;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships
{
    public class PriceChange
    {
        public string Title { get; set; }

        public DateTime CurrentStartDate { get; set; }

        public DateTime IlrStartDate { get; set; }

        public DateTime? IlrEndDate { get; set; }

        public decimal CurrentCost { get; set; }

        public decimal IlrCost { get; set; }

        public bool MissingPriceHistory { get; set; }
    }
}