using System;
using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EmployerCommitments.Application.Extensions;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Extensions.PriceEpisodeExtensions
{
    [TestFixture]
    public class WhenDeterminingCost
    {
        private List<PriceHistory> _episodes;

        [SetUp]
        public void Arrange()
        {
            _episodes = new List<PriceHistory>
            {
                new PriceHistory
                {
                    FromDate = new DateTime(2018,3,1),
                    ToDate = new DateTime(2018,5,1),
                    Cost = 1
                },
                new PriceHistory
                {
                    FromDate = new DateTime(2018,5,2),
                    ToDate = new DateTime(2018,9,1),
                    Cost = 2
                }
            };
        }

        [Test]
        public void ThenIfBeforeFirstPriceEpisodeThenReturnCostOfFirst()
        {
            //Act
            var result = _episodes.GetCostOn(new DateTime(2018, 1, 1));

            //Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void ThenReturnCostOfEffectivePriceEpisode()
        {
            //Act
            var result = _episodes.GetCostOn(new DateTime(2018, 1, 1));

            //Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void ThenIfAfterLastPriceEpisodeThenReturnCostOfLast()
        {
            //Act
            var result = _episodes.GetCostOn(new DateTime(2019, 1, 1));

            //Assert
            Assert.AreEqual(2, result);
        }

        [Test]
        public void ThenReturnCostOfEffectivePriceEpisodeHavingNoDateTo()
        {
            //Arrange
            _episodes.Add(new PriceHistory
            {
                FromDate = new DateTime(2018, 9, 2),
                ToDate = null,
                Cost = 3
            });

            //Act
            var result = _episodes.GetCostOn(new DateTime(2019, 1, 1));

            //Assert
            Assert.AreEqual(3, result);
        }
    }
}
