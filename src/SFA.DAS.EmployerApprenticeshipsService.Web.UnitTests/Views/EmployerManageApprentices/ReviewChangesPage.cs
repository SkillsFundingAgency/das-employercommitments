using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.EmployerCommitments.Web.Views.EmployerManageApprentices;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Views.EmployerManageApprentices
{
    [TestFixture]
    public class ReviewChangesPage : OrchestratedViewModelTestingBase<UpdateApprenticeshipViewModel, ReviewChanges>
    {
        private ApprenticeshipDetailsViewModel _originalApprenticeship;

        private const string Uln = "IAMAULN";

        private const string TrainingName = "IAMATRAININGNAME";

        private const string FirstName = "IAMAFIRSTNAME";

        private const string LastName = "IAMALASTNAME";

        [SetUp]
        public void Setup()
        {
            _originalApprenticeship = new ApprenticeshipDetailsViewModel
            {
                ULN = Uln,
                TrainingName = TrainingName,
                FirstName = FirstName,
                LastName = LastName
            };
        }

        [Test]
        public void ShouldDisplayLearnerName()
        {
            var learnerName = $"{FirstName} {LastName}";

            var model = new UpdateApprenticeshipViewModel
            {
                OriginalApprenticeship = _originalApprenticeship,
            };

            AssertParsedContent(model, "#learnerName", learnerName);
        }

        [Test]
        public void ShouldDisplayUln()
        {
            var model = new UpdateApprenticeshipViewModel
            {
                OriginalApprenticeship = _originalApprenticeship
            };

            AssertParsedContent(model, "#uln", Uln);
        }

        [Test]
        public void ShouldDisplayTrainingName()
        {
            var model = new UpdateApprenticeshipViewModel
            {
                OriginalApprenticeship = _originalApprenticeship
            };

            AssertParsedContent(model, "#trainingName", TrainingName);
        }
    }
}