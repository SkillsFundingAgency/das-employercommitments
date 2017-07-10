using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Domain.Models.Levy;

namespace SFA.DAS.EmployerCommitments.Domain.Models.HmrcLevy
{
    public class EmployerLevyData
    {
        public EmployerLevyData()
        {
            Declarations = new DasDeclarations {Declarations = new List<DasDeclaration>()};
        }
        public string EmpRef { get; set; }

        public DasDeclarations Declarations { get; set; }
        
    }
}