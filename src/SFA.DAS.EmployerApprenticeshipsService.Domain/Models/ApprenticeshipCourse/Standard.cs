using System;

namespace SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse
{
    public class Standard : ITrainingProgramme
    {
        public string Id { get; set; }
        public long Code { get; set; }
        public string Title { get; set; }
        public string CourseName { get; set; }
        public int Level { get; set; }
        public int Duration { get; set; }
        public int MaxFunding { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}