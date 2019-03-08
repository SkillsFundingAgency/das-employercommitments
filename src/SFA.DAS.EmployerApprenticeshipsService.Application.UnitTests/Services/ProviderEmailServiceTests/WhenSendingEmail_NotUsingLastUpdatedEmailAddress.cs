using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions.Common;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Services;
using SFA.DAS.EmployerCommitments.Domain.Models.Notification;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;
using SFA.DAS.NLog.Logger;
using SFA.DAS.PAS.Account.Api.Types;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Services.ProviderEmailServiceTests
{
    [TestFixture]
    public class WhenSendingEmail_NotUsingLastUpdatedEmailAddress
    {
        private Mock<IProviderNotifyService> _mockProviderNotifyService;
        private long _expectedProviderId;
        private string _expectedTemplateId;
        private Dictionary<string, string> _expectedTokens;

        [SetUp]
        public async Task WhenSendingEmailToAllProviderRecipients_NotUsingLastUpdatedEmailAddress()
        {
            _mockProviderNotifyService = new Mock<IProviderNotifyService>();

            _expectedProviderId = 112;
            _expectedTemplateId = "UnitTestNotification";
            _expectedTokens = new Dictionary<string, string>
            {
                { "token1", "value1" },
                { "token2", "value2" }
            };

            var service = new ProviderEmailService(Mock.Of<ILog>(), _mockProviderNotifyService.Object);
            await service.SendEmailToAllProviderRecipients(_expectedProviderId, null,
                new EmailMessage { TemplateId = _expectedTemplateId, Tokens = _expectedTokens });
        }

        [Test]
        public void ThenTheProviderNotifyServiceShouldBeCalledWithTheExpectedRequest()
        {
            _mockProviderNotifyService.Verify(x => x.SendProviderEmailNotifications(_expectedProviderId, It.Is<ProviderEmailRequest>(y =>
                y.ExplicitEmailAddresses == null
                && y.TemplateId == _expectedTemplateId
                && y.Tokens.IsSameOrEqualTo(_expectedTokens)
            )));
        }
    }
}