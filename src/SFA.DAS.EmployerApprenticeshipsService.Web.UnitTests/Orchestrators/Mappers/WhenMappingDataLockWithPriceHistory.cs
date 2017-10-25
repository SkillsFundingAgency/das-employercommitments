﻿using System;
using System.Collections.Generic;

using FluentAssertions;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.EmployerCommitments.Web.Extensions;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.NLog.Logger;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.Mappers
{
    [TestFixture]
    public class WhenMappingDataLockWithPriceHistory : ApprenticeshipMapperBase
    {
        [Test]
        public void WithEmptyLists()
        {
            var dls = new List<DataLockStatus>();
            var h = new List<PriceHistory>();
            var result = Sut.MapPriceChanges(dls, h);

            result.Count.Should().Be(0);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void MapWith1An1(int addDays)
        {
            var now = DateTime.Now.AddMonths(1);

            var dls = new List<DataLockStatus>
                          {
                              new DataLockStatus
                                  {
                                      IlrEffectiveFromDate = now.AddDays(addDays),
                                      IlrTotalCost = 500
                                  }
                          };
            var h = new List<PriceHistory>
                        {
                            new PriceHistory
                                {
                                    ApprenticeshipId = 2, 
                                    Cost = 100,
                                    FromDate = now,
                                    ToDate = now
                                }
                        };

            var result = Sut.MapPriceChanges(dls, h);

            result.Count.Should().Be(1);
            result[0].CurrentCost.Should().Be(100);
            result[0].IlrCost.Should().Be(500);
            result[0].CurrentStartDate.Should().Be(now);
            result[0].IlrStartDate.Should().Be(now.AddDays(addDays));
        }

        [Test]
        public void OnlyPrice()
        {
            var date = new DateTime(2018, 6, 1);
            
            var h = new List<PriceHistory>
                        {
                            new PriceHistory { ApprenticeshipId = 2, Cost = 1000, FromDate = date.AddMonths(0), ToDate = date.AddMonths(6) }, // June
                            new PriceHistory { ApprenticeshipId = 2, Cost = 2000, FromDate = date.AddMonths(3), ToDate = date.AddMonths(6) }, // Sep
                            new PriceHistory { ApprenticeshipId = 2, Cost = 3000, FromDate = date.AddMonths(5), ToDate = date.AddMonths(6) } // Nov
                        };

            var dls = new List<DataLockStatus>
                          {
                              new DataLockStatus { IlrEffectiveFromDate = date.AddMonths(0), IlrTotalCost = 1001 },
                              new DataLockStatus { IlrEffectiveFromDate = date.AddMonths(3), IlrTotalCost = 2002 },
                              new DataLockStatus { IlrEffectiveFromDate = date.AddMonths(5), IlrTotalCost = 3003 }
                          };

            var result = Sut.MapPriceChanges(dls, h);

            result.Count.Should().Be(3);
            FormatPrice(result[0]).Should().Be("1000 1001 - 1 Jun 2018 1 Jun 2018");
            FormatPrice(result[1]).Should().Be("2000 2002 - 1 Sep 2018 1 Sep 2018");
            FormatPrice(result[2]).Should().Be("3000 3003 - 1 Nov 2018 1 Nov 2018");
        }

        [Test]
        public void PriceAndOneDate()
        {
            var date = new DateTime(2018, 6, 1);

            var h = new List<PriceHistory>
                        {
                            new PriceHistory { ApprenticeshipId = 2, Cost = 1000, FromDate = date.AddMonths(0), ToDate = date.AddMonths(6) }, // June
                            new PriceHistory { ApprenticeshipId = 2, Cost = 2000, FromDate = date.AddMonths(3), ToDate = date.AddMonths(6) }, // Sep
                            new PriceHistory { ApprenticeshipId = 2, Cost = 3000, FromDate = date.AddMonths(5), ToDate = date.AddMonths(6) } // Nov
                        };

            var dls = new List<DataLockStatus>
                          {
                              new DataLockStatus { IlrEffectiveFromDate = date.AddMonths(0), IlrTotalCost = 1001 },
                              new DataLockStatus { IlrEffectiveFromDate = date.AddMonths(3), IlrTotalCost = 2002 },
                              new DataLockStatus { IlrEffectiveFromDate = date.AddMonths(4), IlrTotalCost = 3003 } // Oct
                          };

            var result = Sut.MapPriceChanges(dls, h);

            result.Count.Should().Be(3);
            FormatPrice(result[0]).Should().Be("1000 1001 - 1 Jun 2018 1 Jun 2018");
            FormatPrice(result[1]).Should().Be("2000 2002 - 1 Sep 2018 1 Sep 2018");
            FormatPrice(result[2]).Should().Be("2000 3003 - 1 Sep 2018 1 Oct 2018");
        }

        [Test]
        public void OneMorePriceHistory()
        {
            var date = new DateTime(2018, 6, 1);

            var h = new List<PriceHistory>
                        {
                            new PriceHistory { ApprenticeshipId = 2, Cost = 1000, FromDate = date.AddMonths(0), ToDate = date.AddMonths(6) }, // June
                            new PriceHistory { ApprenticeshipId = 2, Cost = 2000, FromDate = date.AddMonths(3), ToDate = date.AddMonths(6) }, // Sep
                            new PriceHistory { ApprenticeshipId = 2, Cost = 3000, FromDate = date.AddMonths(5), ToDate = date.AddMonths(6) } // Nov
                        };

            var dls = new List<DataLockStatus>
                          {
                              new DataLockStatus { IlrEffectiveFromDate = date.AddMonths(3), IlrTotalCost = 2002 },
                              new DataLockStatus { IlrEffectiveFromDate = date.AddMonths(5), IlrTotalCost = 3003 }
                          };

            var result = Sut.MapPriceChanges(dls, h);

            result.Count.Should().Be(2);
            FormatPrice(result[0]).Should().Be("2000 2002 - 1 Sep 2018 1 Sep 2018");
            FormatPrice(result[1]).Should().Be("3000 3003 - 1 Nov 2018 1 Nov 2018");
        }

        [Test]
        public void OneMoreDataLock()
        {
            var date = new DateTime(2018, 6, 1);

            var h = new List<PriceHistory>
                        {
                            //new PriceHistory { ApprenticeshipId = 2, Cost = 1000, FromDate = date.AddMonths(0), ToDate = date.AddMonths(6) }, // June
                            new PriceHistory { ApprenticeshipId = 2, Cost = 2000, FromDate = date.AddMonths(3), ToDate = date.AddMonths(6) }, // Sep
                            new PriceHistory { ApprenticeshipId = 2, Cost = 3000, FromDate = date.AddMonths(5), ToDate = date.AddMonths(6) } // Nov
                        };

            var dls = new List<DataLockStatus>
                          {
                              new DataLockStatus { IlrEffectiveFromDate = date.AddMonths(0), IlrTotalCost = 1001 },
                              new DataLockStatus { IlrEffectiveFromDate = date.AddMonths(3), IlrTotalCost = 2002 },
                              new DataLockStatus { IlrEffectiveFromDate = date.AddMonths(4), IlrTotalCost = 3003 } // Oct
                          };

            var result = Sut.MapPriceChanges(dls, h);

            result.Count.Should().Be(3);
            FormatPrice(result[0]).Should().Be("0 1001 - 1 Jan 0001 1 Jun 2018"); // Missing History
            result[0].MissingPriceHistory.Should().BeTrue();
            FormatPrice(result[1]).Should().Be("2000 2002 - 1 Sep 2018 1 Sep 2018");
            FormatPrice(result[2]).Should().Be("2000 3003 - 1 Sep 2018 1 Oct 2018");
        }

        private string FormatPrice(PriceChange price)
        {
            return $"{price.CurrentCost} {price.IlrCost} - {price.CurrentStartDate.ToGdsFormat()} {price.IlrStartDate.ToGdsFormat()}";
        }
    }
}
