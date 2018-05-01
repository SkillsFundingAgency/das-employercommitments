﻿using System.Threading.Tasks;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IProviderEmailService
    {
        Task SendEmailToAllProviderRecipients(long providerId, string lastUpdateEmailAddress, Email email);
    }
}
