using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
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

        [SetUp]
        public void Setup()
        {
            _orchestrator = new Mock<IEmployerCommitmentsOrchestrator>();

            _orchestrator.Setup(o => o.AuthorizeRole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Role[]>())).Returns(() => Task.FromResult(true));

            _controller = new EmployerCommitmentsController(_orchestrator.Object, Mock.Of<IOwinWrapper>(), Mock.Of<IMultiVariantTestingService>(),
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>(), Mock.Of<ICookieStorageService<string>>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void WithNullLegalEntitiesThenShouldThrowInvalidStateException(bool isTransfer)
        {
            var transferConnectionCode = GetTransferConnectionCode(isTransfer);

            var response = new OrchestratorResponse<SelectLegalEntityViewModel>
            {
                Data = new SelectLegalEntityViewModel()
            };
            _orchestrator.Setup(o => o.GetLegalEntities(HashedAccountId, transferConnectionCode, "", null))
                .ReturnsAsync(response);

            Assert.ThrowsAsync<InvalidStateException>(() => _controller.SelectLegalEntity(HashedAccountId, transferConnectionCode));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void WithEmptyLegalEntitiesThenShouldThrowInvalidStateException(bool isTransfer)
        {
            var transferConnectionCode = GetTransferConnectionCode(isTransfer);

            var response = new OrchestratorResponse<SelectLegalEntityViewModel>
            {
                Data = new SelectLegalEntityViewModel
                {
                    LegalEntities = Enumerable.Empty<LegalEntity>()
                }
            };
            _orchestrator.Setup(o => o.GetLegalEntities(HashedAccountId, transferConnectionCode, "", null))
                .ReturnsAsync(response);

            Assert.ThrowsAsync<InvalidStateException>(() => _controller.SelectLegalEntity(HashedAccountId, transferConnectionCode));
        }

        public enum ExpectedAction
        {
            AgreementNotSigned,
            SearchProvider
        }

        [TestCase(true, EmployerAgreementStatus.Signed, 1, ExpectedAction.AgreementNotSigned)]
        [TestCase(true, EmployerAgreementStatus.Signed, 2, ExpectedAction.SearchProvider)]
        [TestCase(true, EmployerAgreementStatus.Pending, 0, ExpectedAction.AgreementNotSigned)]
        [TestCase(true, EmployerAgreementStatus.Expired, 0, ExpectedAction.AgreementNotSigned)]
        [TestCase(true, EmployerAgreementStatus.Removed, 0, ExpectedAction.AgreementNotSigned)]
        [TestCase(true, EmployerAgreementStatus.Superseded, 0, ExpectedAction.AgreementNotSigned)]
        [TestCase(false, EmployerAgreementStatus.Signed, 1, ExpectedAction.SearchProvider)]
        [TestCase(false, EmployerAgreementStatus.Pending, 1, ExpectedAction.AgreementNotSigned)]
        [TestCase(false, EmployerAgreementStatus.Expired, 1, ExpectedAction.AgreementNotSigned)]
        [TestCase(false, EmployerAgreementStatus.Removed, 1, ExpectedAction.AgreementNotSigned)]
        [TestCase(false, EmployerAgreementStatus.Expired, 1, ExpectedAction.AgreementNotSigned)]
        public async Task SingleLegalEntityThenX(bool isTransfer, EmployerAgreementStatus status, int templateVersionNumber, ExpectedAction expectedAction )
        {
            var transferConnectionCode = GetTransferConnectionCode(isTransfer);

            var response = new OrchestratorResponse<SelectLegalEntityViewModel>
            {
                Data = new SelectLegalEntityViewModel
                {
                    LegalEntities = new[] { new LegalEntity
                    {
                        Agreements = new List<Agreement> {new Agreement
                        {
                            Status = status,
                            TemplateVersionNumber = templateVersionNumber
                        }}
                    }}
                }
            };
            _orchestrator.Setup(o => o.GetLegalEntities(HashedAccountId, transferConnectionCode, "", null))
                .ReturnsAsync(response);

            var result = await _controller.SelectLegalEntity(HashedAccountId, transferConnectionCode);
//todo: model
            AssertRedirectAction(result, expectedAction.ToString());
        }

        private string GetTransferConnectionCode(bool isTransfer)
        {
            return isTransfer ? "TCODE" : string.Empty;
        }

        private void AssertRedirectAction(ActionResult actionResult, string expectedAction, string expectedController = null)
        {
            var result = actionResult as RedirectToRouteResult;

            Assert.NotNull(result, "Not a redirect result");
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual(expectedAction, result.RouteValues["Action"]);
            if (expectedController == null)
                Assert.IsFalse(result.RouteValues.ContainsKey("Controller"));
            else
                Assert.AreEqual(expectedController, result.RouteValues["Controller"]);
        }
    }
}
