using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Web.WebPages;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Views
{
    public class PrecompiledView : IView
    {
        private readonly string _virtualPath;
        private readonly Type _type;
        private readonly IVirtualPathFactory _virtualPathFactory;
        private readonly IViewPageActivator _viewPageActivator;

        public PrecompiledView(
            string virtualPath,
            Type type,
            bool runViewStartPages,
            IEnumerable<string> fileExtensions,
            IVirtualPathFactory virtualPathFactory,
            IViewPageActivator viewPageActivator)
        {
            this._type = type;
            this._virtualPathFactory = virtualPathFactory;
            this._virtualPath = virtualPath;
            RunViewStartPages = runViewStartPages;
            ViewStartFileExtensions = fileExtensions;
            this._viewPageActivator = viewPageActivator;
        }

        public bool RunViewStartPages
        {
            get;
        }

        public IEnumerable<string> ViewStartFileExtensions
        {
            get;
        }

        public string VirtualPath => _virtualPath;

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            var webViewPage = _viewPageActivator.Create(viewContext.Controller.ControllerContext, _type) as WebViewPage;

            if (webViewPage == null)
            {
                throw new InvalidOperationException("Invalid view type");
            }

            webViewPage.VirtualPath = _virtualPath;
            webViewPage.ViewContext = viewContext;
            webViewPage.ViewData = viewContext.ViewData;
            webViewPage.InitHelpers();
            webViewPage.VirtualPathFactory = _virtualPathFactory;

            WebPageRenderingBase startPage = null;
            if (this.RunViewStartPages)
            {
                startPage = StartPageHelper.GetStartPage(webViewPage, "_ViewStart", ViewStartFileExtensions);
            }

            var pageContext = new WebPageContext(viewContext.HttpContext, webViewPage, startPage);
            webViewPage.ExecutePageHierarchy(pageContext, writer, startPage);
        }
    }
}