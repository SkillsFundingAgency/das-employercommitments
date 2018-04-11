using System.Web.Mvc;
using NLog;
using SFA.DAS.EmployerCommitments.Application.Exceptions;

namespace SFA.DAS.EmployerCommitments.Web.Exceptions
{
    public class InvalidStateExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception.GetType() == typeof(InvalidStateException))
            {
                LogManager.GetCurrentClassLogger().Info(filterContext.Exception, "Invalid state exception");

                filterContext.ExceptionHandled = true;
            }
        }
    }
}