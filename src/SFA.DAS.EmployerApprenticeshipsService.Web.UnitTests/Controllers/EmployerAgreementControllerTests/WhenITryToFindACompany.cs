﻿using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Controllers;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers.EmployerAgreementControllerTests
{
    class WhenITryToFindACompany
    {
        private EmployerAgreementController _controller;
        private Mock<IEmployerAgreementOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<IEmployerAgreementOrchestrator>();
            _owinWrapper = new Mock<IOwinWrapper>();

            _controller = new EmployerAgreementController(_owinWrapper.Object, _orchestrator.Object);
        }

        [Test]
        public async Task ThenIShouldGetCompanyDetailsBackIfTheyExist()
        {
            //Arrange
            var viewModel = new FindOrganisationViewModel
            {
                AccountId = 1,
                CompanyName = "Test Corp",
                CompanyNumber = "0123456",
                DateOfIncorporation = DateTime.Now,
                RegisteredAddress = "1 Test Road, Test City, TE12 3ST"
            };

            _orchestrator.Setup(x => x.FindLegalEntity(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<FindOrganisationViewModel>
                {
                    Data = viewModel
                });

            //Act
            var result = await _controller.FindLegalEntity(viewModel.AccountId, viewModel.CompanyNumber) as ViewResult;
            
            //Assert
            Assert.IsNotNull(result);

            _orchestrator.Verify(x => x.FindLegalEntity(viewModel.AccountId, viewModel.CompanyNumber), Times.Once);

            var model = result.Model as SelectEmployerViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(viewModel, model);
        }
    }
}
