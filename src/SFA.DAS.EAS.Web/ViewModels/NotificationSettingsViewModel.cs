using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Domain.Models.Settings;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public class NotificationSettingsViewModel : ViewModelBase
    {
        public string HashedId { get; set; }

        public List<UserNotificationSetting> NotificationSettings { get; set; }
    }
}