using System.Web.Mvc;
using SFA.DAS.EmployerCommitments.Web.Exceptions;
using SFA.DAS.EmployerCommitments.Web.Plumbing.Mvc;

namespace SFA.DAS.EmployerCommitments.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LogAndHandleErrorAttribute());
            filters.Add(new InvalidStateExceptionFilter());
        }
    }
}
