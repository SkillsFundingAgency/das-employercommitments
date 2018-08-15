using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Controllers.EmployerCommitmentsControllerTests
{
    [TestFixture]
    public class WhenSelectingLegalEntity : EmployerCommitmentsControllerTest
    {
        private const string HashedAccountId = "12345";
        private const string LegalEntityCode = "LCODE";
        private const string LegalEntityName = "LNAME";
        private const string CohortRefParam = "";
        private const string CohortRefViewModel = "ViewModelCohortReef";

        [TestCase(true)]
        [TestCase(false)]
        public void WithNullLegalEntitiesThenShouldThrowInvalidStateException(bool isTransfer)
        {
            var transferConnectionCode = GetTransferConnectionCode(isTransfer);

            var response = new OrchestratorResponse<SelectLegalEntityViewModel>
            {
                Data = new SelectLegalEntityViewModel()
            };
            Orchestrator.Setup(o => o.GetLegalEntities(HashedAccountId, transferConnectionCode, CohortRefParam, null))
                .ReturnsAsync(response);

            Assert.ThrowsAsync<InvalidStateException>(() => Controller.SelectLegalEntity(HashedAccountId, transferConnectionCode));
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
            Orchestrator.Setup(o => o.GetLegalEntities(HashedAccountId, transferConnectionCode, CohortRefParam, null))
                .ReturnsAsync(response);

            Assert.ThrowsAsync<InvalidStateException>(() => Controller.SelectLegalEntity(HashedAccountId, transferConnectionCode));
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
        public async Task WithSingleLegalEntityThenRedirectsCorrectly(bool isTransfer, EmployerAgreementStatus status, int templateVersionNumber, ExpectedAction expectedAction )
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
                        }},
                        Code = LegalEntityCode,
                        Name = LegalEntityName
                    }},
                    CohortRef = CohortRefViewModel
                }
            };
            Orchestrator.Setup(o => o.GetLegalEntities(HashedAccountId, transferConnectionCode, CohortRefParam, null))
                .ReturnsAsync(response);

            var result = await Controller.SelectLegalEntity(HashedAccountId, transferConnectionCode);

            object expectedRouteValues;
            switch (expectedAction)
            {
                case ExpectedAction.AgreementNotSigned:
                    expectedRouteValues = new { TransferConnectionCode = transferConnectionCode, HashedAccountId, LegalEntityCode, CohortRef = CohortRefViewModel, HasSignedAgreement = false, LegalEntityName };
                    break;
                case ExpectedAction.SearchProvider:
                    expectedRouteValues = new { TransferConnectionCode = transferConnectionCode, LegalEntityCode, CohortRef = CohortRefViewModel };
                    break;
                default:
                    throw new NotImplementedException();
            }

            AssertRedirectAction(result, expectedAction.ToString(), expectedRouteValues: expectedRouteValues);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task WithMoreThanOneLegalEntityThenShowsExpectedView(bool isTransfer)
        {
            var transferConnectionCode = GetTransferConnectionCode(isTransfer);

            var response = new OrchestratorResponse<SelectLegalEntityViewModel>
            {
                Data = new SelectLegalEntityViewModel
                {
                    LegalEntities = new[] { new LegalEntity(), new LegalEntity() }
                }
            };
            Orchestrator.Setup(o => o.GetLegalEntities(HashedAccountId, transferConnectionCode, CohortRefParam, null))
                .ReturnsAsync(response);

            var result = await Controller.SelectLegalEntity(HashedAccountId, transferConnectionCode);

            AssertViewResult(result);
        }

        private string GetTransferConnectionCode(bool isTransfer)
        {
            return isTransfer ? "TCODE" : string.Empty;
        }
    }
}
