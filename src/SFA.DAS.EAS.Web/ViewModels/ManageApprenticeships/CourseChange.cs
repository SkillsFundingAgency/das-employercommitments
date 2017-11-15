using System;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships
{
    public class CourseChange
    {
        public DateTime CurrentStartDate { get; set; }

        public DateTime? CurrentEndDate { get; set; }

        public DateTime IlrStartDate { get; set; }

        public DateTime? IlrEndDate { get; set; }

        public string CurrentTrainingProgram { get; set; }

        public string IlrTrainingProgram { get; set; }
    }
}