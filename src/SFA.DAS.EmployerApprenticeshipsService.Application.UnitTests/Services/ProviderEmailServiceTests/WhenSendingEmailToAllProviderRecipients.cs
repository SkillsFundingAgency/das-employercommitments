using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions.Common;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Services;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Notification;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.PAS.Account.Api.Types;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Services.ProviderEmailServiceTests
{
    [TestFixture]
    public class WhenSendingEmailToAllProviderRecipients
    {
        private EmployerCommitmentsServiceConfiguration _configuration;
        private Mock<IProviderNotifyService> _mockProviderNotifyService;
        private List<string> _expectedProviderTestEmails;
        private long _expectedProviderId;
        private string _expectedTemplateId;
        private Dictionary<string, string> _expectedTokens;

        [SetUp]
        public async Task WhenSendingEmailToAllProviderRecipients_UsingProviderTestEmails()
        {
            _configuration = new EmployerCommitmentsServiceConfiguration();
            _mockProviderNotifyService = new Mock<IProviderNotifyService>();

            _expectedProviderTestEmails = new List<string> {"providertest1@example.com", "providertest2@example.com"};
            _expectedProviderId = 112;
            _expectedTemplateId = "UnitTestNotification";
            _expectedTokens = new Dictionary<string, string>
            {
                { "token1", "value1" },
                { "token2", "value2" }
            };

            _configuration = new EmployerCommitmentsServiceConfiguration
            {
                CommitmentNotification = new CommitmentNotificationConfiguration
                {
                    UseProviderEmail = false,
                    ProviderTestEmails = _expectedProviderTestEmails
                }
            };

            var service = new ProviderEmailService(Mock.Of<ILog>(), _configuration, _mockProviderNotifyService.Object);
            await service.SendEmailToAllProviderRecipients(_expectedProviderId, null,
                new EmailMessage {TemplateId = _expectedTemplateId, Tokens = _expectedTokens});
        }

        [Test]
        public void ThenTheProviderNotifyServiceShouldBeCalledWithTheExpectedRequest()
        {
            _mockProviderNotifyService.Verify(x => x.SendProviderEmailNotifications(_expectedProviderId, It.Is<ProviderEmailRequest>(y =>
                _expectedProviderTestEmails.All(em => y.ExplicitEmailAddresses.Any(ex => ex == em))
                && y.TemplateId == _expectedTemplateId
                && y.Tokens.IsSameOrEqualTo(_expectedTokens)
                )));
        }
    }
}
