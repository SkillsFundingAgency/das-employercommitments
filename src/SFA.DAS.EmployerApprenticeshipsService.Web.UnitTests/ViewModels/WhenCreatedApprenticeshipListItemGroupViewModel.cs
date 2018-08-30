using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.ViewModels
{
    [TestFixture]
    public class WhenCreatedApprenticeshipListItemGroupViewModel
    {
        #region SetUp

        private const int TestTrainingProgrammeFundingCap = 100;

        private IList<ApprenticeshipListItemViewModel> _singleApprenticeship;
        private ITrainingProgramme _testTrainingProgramme;

        [SetUp]
        public void SetUp()
        {
            _singleApprenticeship = new[]
            {
                new ApprenticeshipListItemViewModel
                {
                    StartDate = new DateTime(2020,2,2),
                    Cost = 500
                }
            };

            _testTrainingProgramme =  new Framework
            {
                FundingPeriods = new[]
                {
                    new FundingPeriod
                    {
                        EffectiveFrom = new DateTime(2020, 2, 1),
                        EffectiveTo = new DateTime(2020, 3, 1),
                        FundingCap = TestTrainingProgrammeFundingCap
                    }
                },
                EffectiveFrom = new DateTime(2020, 2, 1),
                EffectiveTo = new DateTime(2020, 3, 1)
            };
        }

        #endregion SetUp

        #region ApprenticeshipsOverFundingLimit

        [Test]
        public void AndNoTrainingProgrammeThenThereAreNoApprenticeshipsOverFundingLimit()
        {
            var group = new ApprenticeshipListItemGroupViewModel(_singleApprenticeship, null);

            Assert.AreEqual(0, group.ApprenticeshipsOverFundingLimit);
        }

        [Test]
        public void AndNoApprenticeshipsThenThereAreNoApprenticeshipsOverFundingLimit()
        {
            var trainingProgram = new Framework();

            var group = new ApprenticeshipListItemGroupViewModel(new ApprenticeshipListItemViewModel[0], trainingProgram);

            Assert.AreEqual(0, group.ApprenticeshipsOverFundingLimit);
        }

        [TestCase("2020-1-15",   "99", "2020-1-10", "2020-1-20", 100, 0, Description = "StarDatet in band, cost less than cap")]
        [TestCase("2020-1-15",  "100", "2020-1-10", "2020-1-20", 100, 0, Description = "StarDatet in band, cost same as cap")]
        [TestCase("2020-1-15",  "101", "2020-1-10", "2020-1-20", 100, 1, Description = "StartDate in band, cost just over cap")]
        [TestCase("2020-1-15",   null, "2020-1-10", "2020-1-20", 100, 0, Description = "StartDate in band, Apprenticeship has null cost")]
        [TestCase(null,         "101", "2020-1-10", "2020-1-20", 100, 0, Description = "Apprenticeship has no StartDate")]
        [TestCase("2019-12-31", "101", "2020-1-10", "2020-1-20", 100, 0, Description = "StartDate preceeds band, not over funding limit")]
        [TestCase("2020-2-1", "101", "2020-1-10", "2020-1-20", 100, 0, Description = "StartDate after band, not over funding limit")]
        public void AndSingleApprenticeshipThenApprenticeshipsOverFundingLimitIsCalculatedCorrectly(
            DateTime? apprenticeshipStartDate, decimal apprenticeshipCost, 
            DateTime? fundingPeriodFrom, DateTime? fundingPeriodTo, int fundingCap, int expectedApprenticeshipsOverFundingLimit)
        {
            var apprenticeships = new[]
            {
                new ApprenticeshipListItemViewModel
                {
                    StartDate = apprenticeshipStartDate,
                    Cost = apprenticeshipCost
                }
            };

            var trainingProgram = new Framework
            {
                FundingPeriods = new[]
                {
                    new FundingPeriod
                    {
                        EffectiveFrom = fundingPeriodFrom,
                        EffectiveTo = fundingPeriodTo,
                        FundingCap = fundingCap
                    }
                },
                EffectiveFrom = fundingPeriodFrom,
                EffectiveTo = fundingPeriodTo
            };

            var group = new ApprenticeshipListItemGroupViewModel(apprenticeships, trainingProgram);

            Assert.AreEqual(expectedApprenticeshipsOverFundingLimit, group.ApprenticeshipsOverFundingLimit);
        }

        [Test]
        public void AndMultipleApprenticeshipsThenApprenticeshipsOverFundingLimitIsCalculatedCorrectly()
        {
            var apprenticeships = new[]
            {
                new ApprenticeshipListItemViewModel
                {
                    StartDate = new DateTime(2020, 2, 2),
                    Cost = TestTrainingProgrammeFundingCap-1
                },
                new ApprenticeshipListItemViewModel
                {
                    StartDate = new DateTime(2020, 2, 3),
                    Cost = TestTrainingProgrammeFundingCap
                },
                new ApprenticeshipListItemViewModel
                {
                    StartDate = new DateTime(2020, 2, 4),
                    Cost = TestTrainingProgrammeFundingCap+1
                },
            };

            var group = new ApprenticeshipListItemGroupViewModel(apprenticeships, _testTrainingProgramme);

            Assert.AreEqual(1, group.ApprenticeshipsOverFundingLimit);
        }

        #endregion ApprenticeshipsOverFundingLimit

        #region CommonFundingCap

        [Test]
        public void AndNoTrainingProgrammeThenThereIsNoCommonFundingCap()
        {
            var group = new ApprenticeshipListItemGroupViewModel(_singleApprenticeship, null);

            Assert.AreEqual(null, group.CommonFundingCap);
        }

        [Test]
        public void AndNoApprenticeshipsThenThereIsNoCommonFundingCap()
        {
            var apprenticeships = new ApprenticeshipListItemViewModel[0];

            var group = new ApprenticeshipListItemGroupViewModel(apprenticeships, _testTrainingProgramme);

            Assert.AreEqual(null, group.CommonFundingCap);
        }

        [Test]
        public void AndOneApprenticeshipHasNoStartDateThenThereIsNoCommonFundingCap()
        {
            var apprenticeships = new[]
            {
                new ApprenticeshipListItemViewModel
                {
                    StartDate = new DateTime(2020,2,2)
                },
                new ApprenticeshipListItemViewModel
                {
                    StartDate = null
                }
            };

            var group = new ApprenticeshipListItemGroupViewModel(apprenticeships, _testTrainingProgramme);

            Assert.AreEqual(null, group.CommonFundingCap);
        }

        [Test]
        public void AndSingleApprenticeshipWithStartDateOutOfAllFundingBandsThenCommonFundingCapShouldNotBeSet()
        {
            var apprenticeships = new[]
            {
                new ApprenticeshipListItemViewModel
                {
                    StartDate = new DateTime(1969,7,20)
                }
            };

            var group = new ApprenticeshipListItemGroupViewModel(apprenticeships, _testTrainingProgramme);

            Assert.AreEqual(null, group.CommonFundingCap);
        }

        [Test]
        public void AndSingleApprenticeshipThenTheCommonFundingCapShouldBeSet()
        {
            var apprenticeships = new[]
            {
                new ApprenticeshipListItemViewModel
                {
                    StartDate = new DateTime(2020,2,2)
                }
            };

            var group = new ApprenticeshipListItemGroupViewModel(apprenticeships, _testTrainingProgramme);

            Assert.AreEqual(TestTrainingProgrammeFundingCap, group.CommonFundingCap);
        }

        [Test]
        public void AndAllApprenticeshipsShareSameFundingCapThenTheCommonFundingCapShouldBeSet()
        {
            var apprenticeships = new[]
            {
                new ApprenticeshipListItemViewModel
                {
                    StartDate = new DateTime(2020,2,2)
                },
                new ApprenticeshipListItemViewModel
                {
                    StartDate = new DateTime(2020,2,3)
                }
            };

            var group = new ApprenticeshipListItemGroupViewModel(apprenticeships, _testTrainingProgramme);

            Assert.AreEqual(TestTrainingProgrammeFundingCap, group.CommonFundingCap);
        }

        [Test]
        public void AndApprenticeshipsHaveDifferentFundingCapsThenThereIsNoCommonFundingCap()
        {
            var apprenticeships = new[]
            {
                new ApprenticeshipListItemViewModel
                {
                    StartDate = new DateTime(2020,1,1)
                },
                new ApprenticeshipListItemViewModel
                {
                    StartDate = new DateTime(2020,2,1)
                }
            };

            _testTrainingProgramme = new Framework
            {
                FundingPeriods = new[]
                {
                    new FundingPeriod
                    {
                        EffectiveFrom = new DateTime(2020, 1, 1),
                        EffectiveTo = new DateTime(2020, 1, 31),
                        FundingCap = 100
                    },
                    new FundingPeriod
                    {
                        EffectiveFrom = new DateTime(2020, 2, 1),
                        EffectiveTo = new DateTime(2020, 2, 28),
                        FundingCap = 200
                    }
                },
                EffectiveFrom = new DateTime(2020, 1, 1),
                EffectiveTo = new DateTime(2020, 2, 28)
            };

            var group = new ApprenticeshipListItemGroupViewModel(apprenticeships, _testTrainingProgramme);

            Assert.AreEqual(null, group.CommonFundingCap);
        }

        #endregion CommonFundingCap

        #region ShowCommonFundingCap

        [Test]
        public void AndNoApprenticeshipsAndNoTrainingProgrammeThenDontShowCommonFundingCap()
        {
            var apprenticeships = new ApprenticeshipListItemViewModel[0];

            var group = new ApprenticeshipListItemGroupViewModel(apprenticeships, null);

            Assert.AreEqual(false, group.ShowCommonFundingCap);
        }

        [Test]
        public void AndNoApprenticeshipsThenDontShowCommonFundingCap()
        {
            var apprenticeships = new ApprenticeshipListItemViewModel[0];

            var group = new ApprenticeshipListItemGroupViewModel(apprenticeships, _testTrainingProgramme);

            Assert.AreEqual(false, group.ShowCommonFundingCap);
        }

        [Test]
        public void AndNoTrainingProgrammeThenDontShowCommonFundingCap()
        {
            var group = new ApprenticeshipListItemGroupViewModel(_singleApprenticeship, null);

            Assert.AreEqual(false, group.ShowCommonFundingCap);
        }

        [Test]
        public void AndSingleApprenticeshipUnderFundingLimitThenDontShowCommonFundingCap()
        {
            _singleApprenticeship.First().Cost = TestTrainingProgrammeFundingCap - 1;

            var group = new ApprenticeshipListItemGroupViewModel(_singleApprenticeship, _testTrainingProgramme);

            Assert.AreEqual(false, group.ShowCommonFundingCap);
        }

        [Test]
        public void AndSingleApprenticeshipWithStartDateOutsideFundingBandsThenDontShowCommonFundingCap()
        {
            var firstApprenticeship = _singleApprenticeship.First();
            firstApprenticeship.StartDate = new DateTime(2000,1,1);
            firstApprenticeship.Cost = TestTrainingProgrammeFundingCap + 1;

            var group = new ApprenticeshipListItemGroupViewModel(_singleApprenticeship, _testTrainingProgramme);

            Assert.AreEqual(false, group.ShowCommonFundingCap);
        }

        [Test]
        public void AndSingleApprenticeshipOverFundingLimitThenShowCommonFundingCap()
        {
            _singleApprenticeship.First().Cost = TestTrainingProgrammeFundingCap + 1;

            var group = new ApprenticeshipListItemGroupViewModel(_singleApprenticeship, _testTrainingProgramme);

            Assert.AreEqual(true, group.ShowCommonFundingCap);
        }

        [TestCase(TestTrainingProgrammeFundingCap - 1, TestTrainingProgrammeFundingCap - 1, false, Description = "BothUnderLimitThenDontShowCommonFundingCap")]
        [TestCase(TestTrainingProgrammeFundingCap + 1, TestTrainingProgrammeFundingCap - 1, false, Description = "FirstOverLimitAndSecondUnderLimitThenDontShowCommonFundingCap")]
        [TestCase(TestTrainingProgrammeFundingCap - 1, TestTrainingProgrammeFundingCap + 1, false, Description = "FirstUnderLimitAndSeondOverLimitThenDontShowCommonFundingCap")]
        [TestCase(TestTrainingProgrammeFundingCap + 1, TestTrainingProgrammeFundingCap + 1, true, Description = "BothOverLimitThenShowCommonFundingCap")]
        [TestCase(null, null, false, Description = "BothNullThenDontShowCommonFundingCap")]
        [TestCase(null, TestTrainingProgrammeFundingCap - 1, false, Description = "FirstNullAndSecondUnderThenDontShowCommonFundingCap")]
        [TestCase(TestTrainingProgrammeFundingCap - 1, null, false, Description = "FirstUnderAndSecondNullThenDontShowCommonFundingCap")]
        [TestCase(null, TestTrainingProgrammeFundingCap + 1, false, Description = "FirstNullAndSecondOverThenDontShowCommonFundingCap")]
        [TestCase(TestTrainingProgrammeFundingCap + 1, null, false, Description = "FirstOverAndSecondNullThenDontShowCommonFundingCap")]
        public void AndTwoApprenticeships(
            int firstApprenticeshipCost, int secondApprenticeshipCost, bool expectedShowCommonFundingCap)
        {
            var apprenticeships = new[]
            {
                new ApprenticeshipListItemViewModel
                {
                    StartDate = new DateTime(2020,2,2),
                    Cost = firstApprenticeshipCost
                },
                new ApprenticeshipListItemViewModel
                {
                    StartDate = new DateTime(2020,2,2),
                    Cost = secondApprenticeshipCost
                },
            };

            var group = new ApprenticeshipListItemGroupViewModel(apprenticeships, _testTrainingProgramme);

            Assert.AreEqual(expectedShowCommonFundingCap, group.ShowCommonFundingCap);
        }

        #endregion ShowCommonFundingCap
    }
}
