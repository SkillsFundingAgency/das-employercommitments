﻿using System;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.ViewModels
{
    [TestFixture]
    public class WhenCreatedApprenticeshipListItemGroupViewModel
    {
        [TestCase("2020-1-15",   "99", "2020-1-10", "2020-1-20", 100, 0, Description = "StarDatet in band, cost less than cap")]
        [TestCase("2020-1-15",  "100", "2020-1-10", "2020-1-20", 100, 0, Description = "StarDatet in band, cost same as cap")]
        [TestCase("2020-1-15",  "101", "2020-1-10", "2020-1-20", 100, 1, Description = "StartDate in band, cost just over cap")]
        [TestCase("2020-1-15",   null, "2020-1-10", "2020-1-20", 100, 0, Description = "StartDate in band, Apprenticeship has null cost")]
        [TestCase(null,         "101", "2020-1-10", "2020-1-20", 100, 0, Description = "Apprenticeship has no StartDate")]
        //todo: how should we handle cases where no funding band applies? code returns 0 as funding cap in this case, so any cost above 0 is counted as over funding limit
        // how does front end currently handle it? i think it *should* have failed validation and not got this far
        // if out of range of course or funding bands, should we throw an exception?
        //[TestCase("2019-12-31", "101", "2020-1-10", "2020-1-20", 100, 0, Description = "StartDate preceeds band")]
        //[TestCase("2020-2-1",   "101", "2020-1-10", "2020-1-20", 100, 0, Description = "StartDate after band")]
        public void WithSingleTrainingProgramThenApprenticeshipsOverFundingLimitAreCalculated(
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
        public void AndNoTrainingProgrammeThenThereIsNoCommonFundingCap()
        {
            var apprenticeships = new[]
            {
                new ApprenticeshipListItemViewModel
                {
                    StartDate = new DateTime(2020,2,2),
                    Cost = 500
                }
            };

            var group = new ApprenticeshipListItemGroupViewModel(apprenticeships, null);

            Assert.AreEqual(null, group.CommonFundingCap);
        }

        [Test]
        public void AndNoApprenticeshipsThenThereIsNoCommonFundingCap()
        {
            var apprenticeships = new ApprenticeshipListItemViewModel[0];
            var trainingProgram = new Framework();

            var group = new ApprenticeshipListItemGroupViewModel(apprenticeships, trainingProgram);

            Assert.AreEqual(null, group.CommonFundingCap);
        }
    }
}
