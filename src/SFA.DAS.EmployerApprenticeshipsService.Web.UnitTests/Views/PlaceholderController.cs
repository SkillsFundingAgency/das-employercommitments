using System.Web.Http.Results;
using System.Web.Mvc;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Views
{
    internal class PlaceholderController : Controller
    {
        public ActionResult Index()
        {
            return new EmptyResult();
        }
    }
}