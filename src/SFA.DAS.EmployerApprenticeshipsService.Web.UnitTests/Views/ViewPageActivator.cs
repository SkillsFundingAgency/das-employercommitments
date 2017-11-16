using System;
using System.Web.Mvc;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Views
{
    internal class ViewPageActivator : IViewPageActivator
    {
        public object Create(ControllerContext controllerContext, Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}