using System;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;

namespace SFA.DAS.EmployerCommitments.Domain.Models.ReferenceData
{
    public class Organisation
    {
        public string Name { get; set; }
        public SFA.DAS.Common.Domain.Types.OrganisationType Type { get; set; }
        public SFA.DAS.Common.Domain.Types.OrganisationSubType SubType { get; set; }
        public string Code { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public Address Address { get; set; }
        public string Sector { get; set; }
    }
}