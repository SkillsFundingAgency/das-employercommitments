using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.ViewModels
{
    [TestFixture]
    public class GivenAnApprenticeshipFiltersViewModel
    {
        [TestFixture]
        public class WhenCallingHasValues
        {
            [Test]
            public void AndNoValuesThenReturnsFalse()
            {
                var sut = new ApprenticeshipFiltersViewModel();

                sut.HasValues().Should().BeFalse();
            }

            [Test, AutoData]
            public void AndStatusHasValuesThenReturnsTrue(
                List<string> statuses)
            {
                var sut = new ApprenticeshipFiltersViewModel
                {
                    Status = statuses
                };

                sut.HasValues().Should().BeTrue();
            }

            [Test, AutoData]
            public void AndRecordStatusHasValuesThenReturnsTrue(
                List<string> statuses)
            {
                var sut = new ApprenticeshipFiltersViewModel
                {
                    RecordStatus = statuses
                };

                sut.HasValues().Should().BeTrue();
            }

            [Test, AutoData]
            public void AndProviderHasValuesThenReturnsTrue(
                List<string> providers)
            {
                var sut = new ApprenticeshipFiltersViewModel
                {
                    Provider = providers
                };

                sut.HasValues().Should().BeTrue();
            }

            [Test, AutoData]
            public void AndCourseHasValuesThenReturnsTrue(
                List<string> courses)
            {
                var sut = new ApprenticeshipFiltersViewModel
                {
                    Course = courses
                };

                sut.HasValues().Should().BeTrue();
            }

            [Test, AutoData]
            public void AndFundingStatusHasValuesThenReturnsTrue(
                List<string> statuses)
            {
                var sut = new ApprenticeshipFiltersViewModel
                {
                    FundingStatus = statuses
                };

                sut.HasValues().Should().BeTrue();
            }

            [Test, AutoData]
            public void AndSearchInputHasValueThenReturnsTrue(
                string searchInput)
            {
                var sut = new ApprenticeshipFiltersViewModel
                {
                    SearchInput = searchInput
                };

                sut.HasValues().Should().BeTrue();
            }

            [Test, AutoData]
            public void AndPageNumberHasValueThenReturnsTrue(
                int pageNumber)
            {
                var sut = new ApprenticeshipFiltersViewModel
                {
                    PageNumber = pageNumber
                };

                sut.HasValues().Should().BeTrue();
            }
        }
    }
}