﻿namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public sealed class TransferCourseSummaryViewModel
    {
        public string CourseTitle { get; set; }
        public int ApprenticeshipCount { get; set; }
        public string SummaryDescription => $"{CourseTitle} ({ApprenticeshipCount} Apprentices)";
    }
}