using System;
using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Domain.Models.Payments;

namespace SFA.DAS.EmployerCommitments.Web.Models
{
    public class ApprenticeshipPaymentGroup
    {
        public string ApprenticeCourseName { get; set; }
        public int? CourseLevel { get; set; }
        public DateTime? CourseStartDate { get; set; }
        public string TrainingCode { get; set; }
        public List<PaymentTransactionLine> Payments { get; set; }
    }
}