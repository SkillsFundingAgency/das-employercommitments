using System.ComponentModel;

namespace SFA.DAS.EmployerCommitments.Domain.Models.Organisation
{
    public enum OrganisationType: short
    {
        [Description("Listed on Companies House")]
        CompaniesHouse = 1,
        [Description("Charities")]
        Charities = 2,
        [Description("Public Bodies")]
        PublicBodies = 3,
        [Description("Other")]
        Other = 4
    }
}
