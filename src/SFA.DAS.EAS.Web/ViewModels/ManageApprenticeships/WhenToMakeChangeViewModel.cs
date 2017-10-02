using System;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships
{
    public sealed class WhenToMakeChangeViewModel
    {
        public bool SkipStep { get; set; }
        public ChangeStatusViewModel ChangeStatusViewModel { get; set; }
        public DateTime EarliestDate { get; set; }
        public bool EarliestDateIsStartDate { get; set; }
    }
}