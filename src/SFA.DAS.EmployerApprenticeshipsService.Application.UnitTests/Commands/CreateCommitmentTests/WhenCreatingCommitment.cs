using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions.Common;
using KellermanSoftware.CompareNetObjects;
using MediatR;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Commands.CreateCommitment;
using SFA.DAS.EmployerCommitments.Application.Commands.SendNotification;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Commands.CreateCommitmentTests
{
    [TestFixture]
    public class WhenCreatingCommitment
    {
        private CreateCommitmentCommandHandler _commandHandler;

        private Mock<IEmployerCommitmentApi> _employerCommitmentApi;
        //private Mock<IProviderEmailLookupService> _mockProviderEmailLookupService; //this will change to notifc service
        private Mock<IMediator> _mediator;
        private Mock<IProviderEmailLookupService> _providerEmailLookupService;

        private CommitmentView _commitment;

        private CreateCommitmentCommand _payload;
        private CreateCommitmentCommand _validCommand;

        private CommitmentRequest _capturedCommitmentRequest;

        private Func<Task> _act;

        [SetUp]
        public void Arrange()
        {
            _employerCommitmentApi = new Mock<IEmployerCommitmentApi>();

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<SendNotificationCommand>()))
                .ReturnsAsync(new Unit());

            _employerCommitmentApi.Setup(x => x.CreateEmployerCommitment(It.IsAny<long>(),It.IsAny<CommitmentRequest>()))
                .ReturnsAsync(new CommitmentView())
                .Callback<long, CommitmentRequest>((id, req) => _capturedCommitmentRequest = req);

            _commitment = new CommitmentView();

            _employerCommitmentApi.Setup(x => x.GetEmployerCommitment(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(_commitment);

            _providerEmailLookupService = new Mock<IProviderEmailLookupService>();
            _providerEmailLookupService.Setup(x => x.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(new List<string>{ "test@email.com"});
            //_mockProviderEmailLookupService = new Mock<IProviderEmailLookupService>();          

            _payload = new CreateCommitmentCommand
            {
                Commitment = new Commitment
                {
                    EmployerAccountId = 99
                },
                Message = string.Empty,
                UserId = string.Empty
            };

            //Store a copy of the original payload for assertions, to guard against handler modification
            _validCommand =
                JsonConvert.DeserializeObject<CreateCommitmentCommand>(JsonConvert.SerializeObject(_payload));

            _commandHandler = new CreateCommitmentCommandHandler(
                _employerCommitmentApi.Object,
                _mediator.Object, //this will go
                Mock.Of<ILog>(),
                new EmployerCommitmentsServiceConfiguration
                {
                    CommitmentNotification  = new CommitmentNotificationConfiguration
                    {
                        SendEmail = true
                    }
                }, //this will go
                Mock.Of<IHashingService>(), //this will go
                _providerEmailLookupService.Object //this will be notif service
            );

            _act = async () => await _commandHandler.Handle(_payload);
        }

        [Test]
        public async Task ThenCommitmentsApiIsCalledToCreateCommitment()
        {
            await _act.Invoke();

            _employerCommitmentApi.Verify(x => x.CreateEmployerCommitment(
                It.Is<long>(employeraccountId => employeraccountId == _validCommand.Commitment.EmployerAccountId),
                It.IsAny<CommitmentRequest>()
                ),Times.Once);

            _capturedCommitmentRequest.Commitment.ShouldCompare(_validCommand.Commitment);
        }

        [TestCase(CommitmentStatus.Active, true)]
        [TestCase(CommitmentStatus.New, false)]
        public async Task ThenProviderIsSentANotificationIfCommitmentIsAssignedToThem(CommitmentStatus status, bool expectSendNotification)
        {
            //Arrange
            _payload.Commitment.CommitmentStatus = status;

            //Act
            await _act.Invoke();

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<SendNotificationCommand>()),
                Times.Exactly(expectSendNotification ? 1 : 0));
        }
    }
}
