﻿using System.Collections.Generic;
using System.Linq;

using FluentAssertions;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types.ProviderPayment;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.NLog.Logger;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.Mappers
{
    [TestFixture]
    public class WhenMappingProviderPaymentPriority : ApprenticeshipMapperBase
    {
        [Test]
        public void ShouldMapToViewModel()
        {
            var inputData = new List<ProviderPaymentPriorityItem>
                                {
                                    new ProviderPaymentPriorityItem
                                        { ProviderId = 111L, PriorityOrder = 1, ProviderName = "Provider 1"},
                                    new ProviderPaymentPriorityItem
                                        { ProviderId = 222L, PriorityOrder = 2, ProviderName = "Provider 2"},
                                    new ProviderPaymentPriorityItem
                                        { ProviderId = 333L, PriorityOrder = 3, ProviderName = "Provider 3"}
                                };
            var mapped = Sut.MapPayment(inputData);

            mapped.Items.Count().Should().Be(3);
            var first = mapped.Items.Single(m => m.ProviderId == 111);
            first.ProviderName.Should().Be("Provider 1");
            first.Priority.Should().Be(1);
        }
    }
}
