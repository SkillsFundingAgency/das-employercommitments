﻿using System;

namespace SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse
{
    public class Framework : ITrainingProgramme
    {
        public int FrameworkCode { get; set; }
        public string FrameworkName { get; set; }
        public string Id { get; set; }
        public int Level { get; set; }
        public int PathwayCode { get; set; }
        public string PathwayName { get; set; }
        public int ProgrammeType { get; set; }
        public string Title { get; set; }
        public int Duration { get; set; }
        public int MaxFunding { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}