using System;
using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Web.Extensions;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.EmployerCommitments.Web.Views.DataLock;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Views.DataLock
{
    [TestFixture]
    public class RequestChangesPage : ViewModelTestingBase<DataLockStatusViewModel, RequestChanges>
    {
        [Test]
        public void ShouldDisplayLearnerName()
        {
            const string learnerName = "IAMALEARNER";

            var model = new DataLockStatusViewModel
            {
                LearnerName = learnerName,
                CourseChanges = new List<CourseChange>(),
                PriceChanges = new List<PriceChange>()
            };

            AssertParsedContent(model, "#learnerName", learnerName);
        }

        [Test]
        public void ShouldDisplayUln()
        {
            const string uln = "IAMAULN";

            var model = new DataLockStatusViewModel
            {
                ULN = uln,
                CourseChanges = new List<CourseChange>(),
                PriceChanges = new List<PriceChange>()
            };

            AssertParsedContent(model, "#uln", uln);
        }

        [Test]
        public void ShouldDisplayDateOfBirth()
        {
            var dateOfBirth = DateTime.Today;

            var model = new DataLockStatusViewModel
                {
                    DateOfBirth = dateOfBirth,
                    CourseChanges = new List<CourseChange>(),
                    PriceChanges = new List<PriceChange>()
            };

            AssertParsedContent(model, "#dateOfBirth", dateOfBirth.ToGdsFormat());
        }

        [Test]
        public void ShouldNotDisplayDateOfBirth()
        {
            var model = new DataLockStatusViewModel
                {
                    DateOfBirth = null,
                    CourseChanges = new List<CourseChange>(),
                    PriceChanges = new List<PriceChange>()
            };

            AssertParsedContent(model, "#dateOfBirth", string.Empty);
        }
    }
}