using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NUnit.Framework;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Controllers
{
    public class ControllerTest
    {
        protected void AssertViewResult(ActionResult actionResult, string expectedViewName = "")
        {
            var result = actionResult as ViewResult;

            Assert.NotNull(result, "Not a view result");
            Assert.AreEqual(expectedViewName, result.ViewName);
        }

        protected void AssertRedirectAction(ActionResult actionResult,
            string expectedAction, string expectedController = null, object expectedRouteValues = null)
        {
            var result = actionResult as RedirectToRouteResult;

            Assert.NotNull(result, "Not a redirect result");
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual(expectedAction, result.RouteValues["Action"]);
            if (expectedController == null)
                Assert.IsFalse(result.RouteValues.ContainsKey("Controller"));
            else
                Assert.AreEqual(expectedController, result.RouteValues["Controller"]);

            if (expectedRouteValues == null)
                return;

            var expectedRouteValueDictionarySubset = expectedRouteValues.GetType().GetProperties().Select(v => new KeyValuePair<string, object>(v.Name, v.GetValue(expectedRouteValues)));
            CollectionAssert.IsSubsetOf(expectedRouteValueDictionarySubset, result.RouteValues);
        }
    }
}
