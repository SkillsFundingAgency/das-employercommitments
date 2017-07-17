using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Domain.Models.UserView;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IMultiVariantTestingService
    {
        MultiVariantViewLookup GetMultiVariantViews();
        string GetRandomViewNameToShow(List<ViewAccess> views);
    }
}