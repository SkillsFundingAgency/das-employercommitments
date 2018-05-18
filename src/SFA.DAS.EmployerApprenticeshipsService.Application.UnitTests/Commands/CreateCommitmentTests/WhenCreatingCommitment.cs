using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Commands.CreateCommitment;
using SFA.DAS.EmployerCommitments.Application.Commands.SendNotification;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Commands.CreateCommitmentTests
{
    [TestFixture]
    public class WhenCreatingCommitment
    {
        private CreateCommitmentCommandHandler _handler;
        private Mock<IEmployerCommitmentApi> _employerCommitmentApi;
        private Mock<IMediator> _mediator;
        private Mock<IProviderEmailLookupService> _providerEmailLookupService;
        private CommitmentView _commitmentView;
        private CreateCommitmentCommand ValidCommand;

        [SetUp]
        public void Arrange()
        {
            ValidCommand = new CreateCommitmentCommand
            {
                Commitment = new Commitment {CommitmentStatus = CommitmentStatus.Active}
            };

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<SendNotificationCommand>()))
                .ReturnsAsync(new Unit());

            var config = new EmployerCommitmentsServiceConfiguration
            {
                CommitmentNotification = new CommitmentNotificationConfiguration
                {
                    SendEmail = true
                }
            };
            _commitmentView = new CommitmentView {Id = 1};

            _employerCommitmentApi = new Mock<IEmployerCommitmentApi>();
            _employerCommitmentApi
                .Setup(x => x.CreateEmployerCommitment(It.IsAny<long>(), It.IsAny<CommitmentRequest>()))
                .ReturnsAsync(_commitmentView);

            _employerCommitmentApi
                .Setup(x => x.GetEmployerCommitment(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(_commitmentView);

            _providerEmailLookupService = new Mock<IProviderEmailLookupService>();
            _providerEmailLookupService.Setup(x => x.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(new List<string>
                {
                    {"test@testing"}
                });

            _handler = new CreateCommitmentCommandHandler(
                _employerCommitmentApi.Object,
                _mediator.Object,
                Mock.Of<ILog>(),
                config, _providerEmailLookupService.Object);
        }

        [Test]
        public async Task ShouldUseTransferTemplateWhenCommitmentHasATransferSender()
        {
            _commitmentView.TransferSenderId = 1;

            await _handler.Handle(ValidCommand);

            _mediator.Verify(x =>
                x.SendAsync(It.Is<SendNotificationCommand>(command =>
                    command.Email.TemplateId == "CreateTransferCommitmentNotification")));
        }
    }
}
