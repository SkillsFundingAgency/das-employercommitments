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

        private const string Lastname = "IAMALASTNAME";

        [SetUp]
        public void Setup()
        {
            _originalApprenticeship = new ApprenticeshipDetailsViewModel {ULN = Uln};
        }

        [Test]
        public void ShouldDisplayLearnerName()
        {
            var learnerName = $"{FirstName} {Lastname}";

            var model = new UpdateApprenticeshipViewModel
            {
                FirstName = FirstName,
                LastName = Lastname,
                OriginalApprenticeship = _originalApprenticeship
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
                TrainingName = TrainingName,
                OriginalApprenticeship = _originalApprenticeship
            };

            AssertParsedContent(model, "#trainingName", TrainingName);
        }
    }
}