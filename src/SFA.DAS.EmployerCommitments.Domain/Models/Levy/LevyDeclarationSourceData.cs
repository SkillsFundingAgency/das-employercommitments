using System.Collections.Generic;

namespace SFA.DAS.EmployerCommitments.Domain.Models.Levy
{
    public class LevyDeclarationSourceData
    {
        public long AccountId { get; set; }  

        public List<LevyDeclarationSourceDataItem> Data { get; set; }
    }
}