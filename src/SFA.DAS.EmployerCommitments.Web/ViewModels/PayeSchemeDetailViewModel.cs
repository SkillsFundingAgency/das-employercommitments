using System;
using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Domain.Models.Levy;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public class PayeSchemeDetailViewModel
    {
        public IEnumerable<DasEnglishFraction> Fractions { get; set; }
        public string EmpRef { get; set; }
        public string PayeSchemeName { get; set; }
        public DateTime EmpRefAdded { get; set; }
    }
}