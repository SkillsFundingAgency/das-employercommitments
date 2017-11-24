using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.EmployerCommitments.Web.Views.DataLock;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Views.DataLock
{
    [TestFixture]
    public class RequestChangesPage : OrchestratedViewModelTestingBase<DataLockStatusViewModel, RequestChanges>
    {
        private const string Uln = "IAMAULN";
        private const string TrainingName = "IAMATRAININGNAME";

        private const string LearnerName = "IAMALEARNER";

        [Test]
        public void ShouldDisplayLearnerName()
        {
            var model = new DataLockStatusViewModel
            {
                LearnerName = LearnerName,
                CourseChanges = new List<CourseChange>(),
                PriceChanges = new List<PriceChange>()
            };

            AssertParsedContent(model, "#learnerName", LearnerName);
        }

        [Test]
        public void ShouldDisplayUln()
        {
            var model = new DataLockStatusViewModel
            {
                ULN = Uln,
                CourseChanges = new List<CourseChange>(),
                PriceChanges = new List<PriceChange>()
            };

            AssertParsedContent(model, "#uln", Uln);
        }

        [Test]
        public void ShouldDisplayTrainingName()
        {
            var model = new DataLockStatusViewModel
            {
                TrainingName = TrainingName,
                CourseChanges = new List<CourseChange>(),
                PriceChanges = new List<PriceChange>()
            };

            AssertParsedContent(model, "#trainingName", TrainingName);
        }
    }
}