using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerCommitments.Domain.Models.Organisation
{
    public class LegalEntity
    {
        public string Name { get; set; }
        public string RegisteredAddress { get; set; }
        public short Source { get; set; }
        public EmployerAgreementStatus AgreementStatus { get; set; }
        public long Id { get; set; }
        public string Code { get; set; }
    }
}
