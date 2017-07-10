using System.ComponentModel;
using System.Linq;
using SFA.DAS.EmployerCommitments.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EmployerCommitments.Domain.Extensions
{
    public static class EmploymentStatusAgreementExtension
    {
        public static string GetDescription(this EmployerAgreementStatus status)
        {
            var type = typeof(EmployerAgreementStatus);
            var memInfo = type.GetMember(status.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            var descriptionAttribute = attributes.OfType<DescriptionAttribute>().FirstOrDefault();

            return descriptionAttribute?.Description ?? string.Empty;
        }
    }
}
