using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Domain.Models.PAYE;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public class EmployerAccountPayeListViewModel
    {
        public string HashedId { get; set; }
                    
        public List<PayeView> PayeSchemes { get; set; }
    }
}