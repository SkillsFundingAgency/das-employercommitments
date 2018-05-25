using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Commands.SendNotification;
using SFA.DAS.EmployerCommitments.Application.Commands.SubmitCommitment;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Commands.SubmitCommitmentTests
{
    [TestFixture]
    public sealed class WhenSubmittingCommitment
    {
        private SubmitCommitmentCommandHandler _handler;
        private Mock<IEmployerCommitmentApi> _mockCommitmentApi;
        private SubmitCommitmentCommand _validCommand;
        private Mock<IMediator> _mockMediator;
        private Mock<IProviderEmailLookupService> _mockEmailLookup;
        private CommitmentView _repositoryCommitment;

        private const string CohortReference = "COREF";

        [SetUp]
        public void Setup()
        {
            _validCommand = new SubmitCommitmentCommand { EmployerAccountId = 12L, CommitmentId = 2L, UserDisplayName = "Test User", UserEmailAddress = "test@test.com", UserId = "externalUserId"};
            _repositoryCommitment = new CommitmentView
            {
                ProviderId = 456L,
                EmployerAccountId = 12L,
                AgreementStatus = AgreementStatus.NotAgreed,
                Reference = CohortReference
            };

            _mockCommitmentApi = new Mock<IEmployerCommitmentApi>();
            _mockCommitmentApi.Setup(x => x.GetEmployerCommitment(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(_repositoryCommitment);

            _mockMediator = new Mock<IMediator>();
            var config = new EmployerCommitmentsServiceConfiguration
                             {
                                 CommitmentNotification = new CommitmentNotificationConfiguration { SendEmail = true }
                             };
            _mockEmailLookup = new Mock<IProviderEmailLookupService>();
            _mockEmailLookup.Setup(m => m.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(new List<string>());

            _handler = new SubmitCommitmentCommandHandler(_mockCommitmentApi.Object, _mockMediator.Object, config, _mockEmailLookup.Object, Mock.Of<ILog>());
        }

        [Test]
        public async Task ThenIfTheEmployerIsAmendingTheCommitmentTheCommitmentShouldBePatched()
        {
            await _handler.Handle(_validCommand);

            _mockCommitmentApi.Verify(x => x.PatchEmployerCommitment(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CommitmentSubmission>()));
        }

        [Test]
        public async Task ThenIfTheEmployerIsApprovingTheCommitmentTheCommitmentShouldBeApproved()
        {
            _validCommand.LastAction = LastAction.Approve;

            await _handler.Handle(_validCommand);

            _mockCommitmentApi.Verify(x => x.ApproveCohort(_validCommand.EmployerAccountId, _validCommand.CommitmentId,
                It.Is<CommitmentSubmission>(y =>
                    y.UserId == _validCommand.UserId && y.Message == _validCommand.Message && y.LastUpdatedByInfo.EmailAddress == _validCommand.UserEmailAddress &&
                    y.LastUpdatedByInfo.Name == _validCommand.UserDisplayName)));
        }

        [Test]
        public async Task NotCallGetProviderEmailQueryRequest()
        {
            _validCommand.LastAction = LastAction.None;
            await _handler.Handle(_validCommand);

            _mockEmailLookup.Verify(x => x.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task NotShouldCallSendNotificationCommand()
        {
            _validCommand.LastAction = LastAction.None;
            await _handler.Handle(_validCommand);

            _mockMediator.Verify(x => x.SendAsync(It.IsAny<SendNotificationCommand>()), Times.Never);
        }

        [Test]
        public async Task CallGetProviderEmailQueryRequest()
        {
            _validCommand.LastAction = LastAction.Amend;
            await _handler.Handle(_validCommand);

            _mockEmailLookup.Verify(x => x.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ShouldCallSendNotificationCommandForCohortReview()
        {
            SendNotificationCommand arg = null;
            _validCommand.LastAction = LastAction.Amend;
            _mockEmailLookup
                .Setup(x => x.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(new List<string> { "test@email.com" });

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<SendNotificationCommand>()))
                .ReturnsAsync(new Unit()).Callback<SendNotificationCommand>(x => arg = x );

            await _handler.Handle(_validCommand);

            _mockMediator.Verify(x => x.SendAsync(It.IsAny<SendNotificationCommand>()));
            arg.Email.TemplateId.Should().Be("ProviderCommitmentNotification");
            arg.Email.Tokens["cohort_reference"].Should().Be(CohortReference);
            arg.Email.Tokens["type"].Should().Be("review");
        }

        [Test]
        public async Task ShouldCallSendNotificationCommandForCohortFirstApproval()
        {
            SendNotificationCommand arg = null;
            _validCommand.LastAction = LastAction.Approve;

            _mockEmailLookup
                .Setup(x => x.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(new List<string> { "test@email.com" });

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<SendNotificationCommand>()))
                .ReturnsAsync(new Unit()).Callback<SendNotificationCommand>(x => arg = x);

            await _handler.Handle(_validCommand);

            _mockMediator.Verify(x => x.SendAsync(It.IsAny<SendNotificationCommand>()));
            arg.Email.TemplateId.Should().Be("ProviderCommitmentNotification");
            arg.Email.Tokens["cohort_reference"].Should().Be(CohortReference);
            arg.Email.Tokens["type"].Should().Be("approval");
        }

        [Test]
        public async Task ShouldCallSendNotificationCommandForCohortSecondApproval()
        {
            SendNotificationCommand arg = null;
            _validCommand.LastAction = LastAction.Approve;
            _repositoryCommitment.AgreementStatus = AgreementStatus.ProviderAgreed;

            _mockEmailLookup
                .Setup(x => x.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(new List<string> { "test@email.com" });

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<SendNotificationCommand>()))
                .ReturnsAsync(new Unit()).Callback<SendNotificationCommand>(x => arg = x);

            await _handler.Handle(_validCommand);

            _mockMediator.Verify(x => x.SendAsync(It.IsAny<SendNotificationCommand>()));
            arg.Email.TemplateId.Should().Be("ProviderCohortApproved");
        }

        [Test]
        public async Task ShouldCallSendNotificationCommandForTransferCohortFirstApproval()
        {
            const string legalEntityName = "Receiving Employer Ltd";

            const string template =
                @"Cohort ((cohort_reference)) is ready to review. ((receiving_employer)) has chosen to use funds transferred from another employer to pay for the training in this cohort.

What you need to know about cohorts funded through transfers

You and the employer will approve the cohort as usual. The cohort will then be sent to the employer who is transferring the funds for final approval. Once the cohort has been approved you will be able to view and manage the apprentices
To review cohort ((cohort_reference)) you will need to sign in to your apprenticeship service account at https://providers.apprenticeships.sfa.bis.gov.uk.

This is an automated message. Please don’t reply to this email.

Kind regards,

Apprenticeship service team";

            string expectedEmailBody = $@"Cohort {CohortReference} is ready to review. {legalEntityName} has chosen to use funds transferred from another employer to pay for the training in this cohort.

What you need to know about cohorts funded through transfers

You and the employer will approve the cohort as usual. The cohort will then be sent to the employer who is transferring the funds for final approval. Once the cohort has been approved you will be able to view and manage the apprentices
To review cohort {CohortReference} you will need to sign in to your apprenticeship service account at https://providers.apprenticeships.sfa.bis.gov.uk.

This is an automated message. Please don’t reply to this email.

Kind regards,

Apprenticeship service team";

            SendNotificationCommand arg = null;
            _validCommand.LastAction = LastAction.Approve;
            _repositoryCommitment.TransferSender = new TransferSender();
            _repositoryCommitment.LegalEntityName = legalEntityName;

            _mockEmailLookup
                .Setup(x => x.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(new List<string> { "test@email.com" });

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<SendNotificationCommand>()))
                .ReturnsAsync(new Unit()).Callback<SendNotificationCommand>(x => arg = x);

            await _handler.Handle(_validCommand);

            _mockMediator.Verify(x => x.SendAsync(It.IsAny<SendNotificationCommand>()));
            arg.Email.TemplateId.Should().Be("TransferProviderCommitmentNotification");
            arg.Email.Tokens["cohort_reference"].Should().Be(CohortReference);
            arg.Email.Tokens["receiving_employer"].Should().Be(legalEntityName);

            var emailBody = PopulateTemplate(template, arg.Email.Tokens);
            TestContext.WriteLine(emailBody);
            Assert.AreEqual(expectedEmailBody, emailBody);
        }

        [Test]
        public async Task ShouldCallSendNotificationCommandForTransferCohortSecondApproval()
        {
            const string legalEntityName = "Receiving Employer Ltd";
            const long providerId = 10000000;

            const string template = @"((receiving_employer)) has approved cohort ((cohort_reference)).

What happens next?
 A transfer request has been sent to the sending employer for approval. You will receive a notification once the sending employer approves or rejects the request.

To view cohort ((cohort_reference)) and its progress, follow the link below.
https://providers.apprenticeships.sfa.bis.gov.uk/((ukprn))/Apprentices/Cohorts
 
This is an automated message. Please do not reply to this email.

Kind regards,

Apprenticeship service team";

            string expectedEmailBody = $@"{legalEntityName} has approved cohort {CohortReference}.

What happens next?
 A transfer request has been sent to the sending employer for approval. You will receive a notification once the sending employer approves or rejects the request.

To view cohort {CohortReference} and its progress, follow the link below.
https://providers.apprenticeships.sfa.bis.gov.uk/{providerId}/Apprentices/Cohorts
 
This is an automated message. Please do not reply to this email.

Kind regards,

Apprenticeship service team";

            SendNotificationCommand arg = null;
            _validCommand.LastAction = LastAction.Approve;
            _repositoryCommitment.AgreementStatus = AgreementStatus.ProviderAgreed;
            _repositoryCommitment.TransferSender = new TransferSender();
            _repositoryCommitment.LegalEntityName = legalEntityName;
            _repositoryCommitment.ProviderId = providerId;

            _mockEmailLookup
                .Setup(x => x.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(new List<string> { "test@email.com" });

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<SendNotificationCommand>()))
                .ReturnsAsync(new Unit()).Callback<SendNotificationCommand>(x => arg = x);

            await _handler.Handle(_validCommand);

            _mockMediator.Verify(x => x.SendAsync(It.IsAny<SendNotificationCommand>()));
            arg.Email.TemplateId.Should().Be("TransferPendingFinalApproval");
            arg.Email.Tokens["cohort_reference"].Should().Be(CohortReference);
            arg.Email.Tokens["receiving_employer"].Should().Be(legalEntityName);
            arg.Email.Tokens["ukprn"].Should().Be(providerId.ToString());

            var emailBody = PopulateTemplate(template, arg.Email.Tokens);
            TestContext.WriteLine(emailBody);
            Assert.AreEqual(expectedEmailBody, emailBody);
        }

        private string PopulateTemplate(string template, Dictionary<string, string> tokens)
        {
            foreach (var token in tokens)
            {
                template = template.Replace($"(({token.Key}))", token.Value);
            }

            return template;
        }

        [Test]
        public async Task ShouldCallSendNotificationCommandOncePerEmailAddress()
        {
            _validCommand.LastAction = LastAction.Amend;
            _mockEmailLookup
                .Setup(x => x.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(new List<string> { "test@email.com", "test2@email.com" });

            await _handler.Handle(_validCommand);

            _mockMediator.Verify(x => x.SendAsync(It.IsAny<SendNotificationCommand>()), Times.Exactly(2));
        }

        [Test]
        public void ThenValidationErrorsShouldThrowAnException()
        {
            _validCommand.EmployerAccountId = 0; // Should fail validation

            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_validCommand));
        }

        [Test]
        public void ThenValidationErrorsShouldThrowAnException2()
        {
            _validCommand.EmployerAccountId = 2;

            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_validCommand));
        }
    }
}

