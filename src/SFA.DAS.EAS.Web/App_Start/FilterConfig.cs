using System.Web;
using System.Web.Mvc;
using SFA.DAS.EmployerCommitments.Web.Exceptions;
using SFA.DAS.EmployerCommitments.Web.Plumbing.Mvc;
using SFA.DAS.NLog.Logger.Web;

namespace SFA.DAS.EmployerCommitments.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LogAndHandleErrorAttribute());
            filters.Add(new InvalidStateExceptionFilter());

            filters.Add(new RequestIdActionFilter());
            filters.Add(new SessionIdActionFilter(HttpContext.Current));
        }
    }
}
