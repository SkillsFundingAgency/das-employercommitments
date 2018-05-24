using System;
using System.Collections.Generic;
using System.Linq;
using FeatureToggle;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.FeatureToggles;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.EmployerCommitments.Web.PublicHashingService;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.Mappers.CommitmentMapperTests
{
    [TestFixture]
    public class WhenMappingToTransferConnectionsViewModel
    {
        private CommitmentMapper _sut;
        private Mock<IPublicHashingService> _publicHashingService;


        [SetUp]
        public void Arrange()
        {
            _publicHashingService = new Mock<IPublicHashingService>();
            _publicHashingService.Setup(x => x.HashValue(It.IsAny<long>())).Returns((long p) => $"XYZ{p}");

            _sut = new CommitmentMapper(Mock.Of<IHashingService>(), Mock.Of<IFeatureToggleService>(), _publicHashingService.Object);
        }

        [Test]
        public void ThenMapsToTransferConnectionsViewModelCorrectly()
        {
            var list = new List<TransferConnection>
            {
                new TransferConnection {AccountName = "AccountName", AccountId = 123}
            };

            var result = _sut.MapToTransferConnectionsViewModel(list).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual($"XYZ{123}", result[0].TransferConnectionCode);
            Assert.AreEqual("AccountName", result[0].TransferConnectionName);
        }

    }
}