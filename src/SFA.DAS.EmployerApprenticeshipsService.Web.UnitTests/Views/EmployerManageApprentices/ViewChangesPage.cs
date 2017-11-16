using System;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Web.Extensions;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.EmployerCommitments.Web.Views.EmployerManageApprentices;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Views.EmployerManageApprentices
{
    [TestFixture]
    public class ViewChangesPage : ViewModelTestingBase<UpdateApprenticeshipViewModel, ViewChanges>
    {
        private ApprenticeshipDetailsViewModel _originalApprenticeship;

        private DateTime _dateOfBirth;

        private DateTimeViewModel _dateOfBirthModel;

        private const string Uln = "IAMAULN";

        private const string FirstName = "IAMAFIRSTNAME";

        private const string Lastname = "IAMALASTNAME";

        [SetUp]
        public void Setup()
        {
            _dateOfBirth = DateTime.Today;
            _originalApprenticeship = new ApprenticeshipDetailsViewModel { ULN = Uln };
            _dateOfBirthModel = new DateTimeViewModel(_dateOfBirth);
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
        public void ShouldDisplayDateOfBirth()
        {
            var dateOfBirth = DateTime.Today;

            var model = new UpdateApprenticeshipViewModel
            {
                DateOfBirth = _dateOfBirthModel,
                OriginalApprenticeship = _originalApprenticeship
            };

            AssertParsedContent(model, "#dateOfBirth", dateOfBirth.ToGdsFormat());
        }

        [Test]
        public void ShouldNotDisplayDateOfBirth()
        {
            var model = new UpdateApprenticeshipViewModel
            {
                DateOfBirth = new DateTimeViewModel(default(DateTime?)),
                OriginalApprenticeship = _originalApprenticeship
            };

            AssertParsedContent(model, "#dateOfBirth", string.Empty);
        }
    }
}