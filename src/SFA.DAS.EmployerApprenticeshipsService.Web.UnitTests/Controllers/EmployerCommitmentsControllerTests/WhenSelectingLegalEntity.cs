﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;
using SFA.DAS.EmployerCommitments.Domain.Models.UserProfile;
using SFA.DAS.EmployerCommitments.Web.Authentication;
using SFA.DAS.EmployerCommitments.Web.Controllers;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Controllers.EmployerCommitmentsControllerTests
{
    [TestFixture]
    public class WhenSelectingLegalEntity
    {
        private Mock<IEmployerCommitmentsOrchestrator> _orchestrator;
        private EmployerCommitmentsController _controller;

        private const string HashedAccountId = "12345";
        private const string NonTransferTransferConnectionCode = "";

        [SetUp]
        public void Setup()
        {
            _orchestrator = new Mock<IEmployerCommitmentsOrchestrator>();

            _orchestrator.Setup(o => o.AuthorizeRole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Role[]>())).Returns(() => Task.FromResult(true));

            _controller = new EmployerCommitmentsController(_orchestrator.Object, Mock.Of<IOwinWrapper>(), Mock.Of<IMultiVariantTestingService>(),
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>(), Mock.Of<ICookieStorageService<string>>());
        }

        [Test]
        public void WithNullLegalEntitiesThenShouldThrowInvalidStateException()
        {
            var response = new OrchestratorResponse<SelectLegalEntityViewModel>
            {
                Data = new SelectLegalEntityViewModel()
            };
            _orchestrator.Setup(o => o.GetLegalEntities(HashedAccountId, NonTransferTransferConnectionCode, "", null))
                .ReturnsAsync(response);

            Assert.ThrowsAsync<InvalidStateException>(() => _controller.SelectLegalEntity(HashedAccountId, NonTransferTransferConnectionCode));
        }

        [Test]
        public void WithEmptyLegalEntitiesThenShouldThrowInvalidStateException()
        {
            var response = new OrchestratorResponse<SelectLegalEntityViewModel>
            {
                Data = new SelectLegalEntityViewModel
                {
                    LegalEntities = Enumerable.Empty<LegalEntity>()
                }
            };
            _orchestrator.Setup(o => o.GetLegalEntities(HashedAccountId, NonTransferTransferConnectionCode, "", null))
                .ReturnsAsync(response);

            Assert.ThrowsAsync<InvalidStateException>(() => _controller.SelectLegalEntity(HashedAccountId, NonTransferTransferConnectionCode));
        }

        [Test]
        public async Task NonTransferWithSingleLegalEntityThenX()
        {
            var response = new OrchestratorResponse<SelectLegalEntityViewModel>
            {
                Data = new SelectLegalEntityViewModel
                {
                    LegalEntities = new[] {new LegalEntity()}
                }
            };
            _orchestrator.Setup(o => o.GetLegalEntities(HashedAccountId, NonTransferTransferConnectionCode, "", null))
                .ReturnsAsync(response);

            await _controller.SelectLegalEntity(HashedAccountId, NonTransferTransferConnectionCode);
        }
    }
}
