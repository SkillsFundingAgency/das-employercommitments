using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using System.Web.WebPages;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Views
{
    public class ViewRenderer
    {
        private static readonly ConcurrentDictionary<Assembly, ViewRenderer> Cache
            = new ConcurrentDictionary<Assembly, ViewRenderer>();

        /// <summary>
        /// Gets instance of ViewRenderer for executing views in the Assembly of
        /// the specified type T.
        /// </summary>
        /// <returns></returns>
        public static ViewRenderer ForAssemblyOf<T>()
        {
            return ForAssembly(typeof(T).Assembly);
        }

        /// <summary>
        /// Gets instance of ViewRenderer for executing views in the specified Assembly 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static ViewRenderer ForAssembly(Assembly assembly)
        {
            return Cache.GetOrAdd(assembly, a =>
            {
                var viewEngine = new PrecompiledViewEngine(a);
                ViewEngines.Engines.Insert(0, viewEngine);
                return new ViewRenderer(viewEngine);
            });
        }

        private readonly PrecompiledViewEngine _viewEngine;

        public ViewRenderer(PrecompiledViewEngine viewEngine)
        {
            this._viewEngine = viewEngine;
        }

        public string RenderView<T>(object model, Uri baseUri)
            where T : WebViewPage
        {
            var attribute = typeof(T).GetCustomAttribute<PageVirtualPathAttribute>();
            if (attribute == null)
            {
                throw new ArgumentException($"{typeof(T)} does not appear to be a pre-compiled view. The PageVirtualPathAttribute could not be found");
            }
            var virtualPath = attribute.VirtualPath;
            return RenderView(virtualPath, model, baseUri);
        }

        public string RenderView(string virtualPath, object model, Uri baseUri)
        {
            var view = _viewEngine.CreateViewInstance(virtualPath);
            if (view == null)
                throw new ArgumentException("Unable to find view with virtual path " + virtualPath, nameof(virtualPath));

            var writer = new StringWriter();
            var viewContext = CreateViewContext(view, baseUri, virtualPath, model, writer);
            view.Render(viewContext, writer);
            return writer.ToString();
        }

        private static ViewContext CreateViewContext(IView view, Uri baseUri, string virtualPath, object model, StringWriter writer)
        {
            var httpContext = new OfflineHttpContext(baseUri, virtualPath);
            HttpContext.Current = ContextHelper.GetFakeContext();

            var routeData = new RouteData();
            routeData.Values["controller"] = "Placeholder";
            routeData.Values["action"] = "Execute";
            var controller = new PlaceholderController();
            var controllerContext = new ControllerContext(httpContext, routeData, controller);

            var viewData = new ViewDataDictionary(model);
            return new ViewContext(controllerContext, view, viewData, new TempDataDictionary(), writer);
        }

        public class PrecompiledViewEngine : VirtualPathProviderViewEngine, IVirtualPathFactory
        {
            private readonly IDictionary<string, Type> _mappings;
            private readonly string _baseVirtualPath;
            private readonly IViewPageActivator _viewPageActivator;

            public PrecompiledViewEngine(Assembly assembly)
                : this(assembly, null)
            {
            }

            public PrecompiledViewEngine(Assembly assembly, string baseVirtualPath)
            {
                _baseVirtualPath = NormalizeBaseVirtualPath(baseVirtualPath);

                AreaViewLocationFormats = new[] {
                    "~/Areas/{2}/Views/{1}/{0}.cshtml",
                    "~/Areas/{2}/Views/Shared/{0}.cshtml"
                };

                AreaMasterLocationFormats = new[] {
                    "~/Areas/{2}/Views/{1}/{0}.cshtml",
                    "~/Areas/{2}/Views/Shared/{0}.cshtml"
                };

                AreaPartialViewLocationFormats = new[] {
                    "~/Areas/{2}/Views/{1}/{0}.cshtml",
                    "~/Areas/{2}/Views/Shared/{0}.cshtml"
                };
                ViewLocationFormats = new[] {
                    "~/Views/{1}/{0}.cshtml",
                    "~/Views/Shared/{0}.cshtml"
                };
                MasterLocationFormats = new[] {
                    "~/Views/{1}/{0}.cshtml",
                    "~/Views/Shared/{0}.cshtml"
                };
                PartialViewLocationFormats = new[] {
                    "~/Views/{1}/{0}.cshtml",
                    "~/Views/Shared/{0}.cshtml"
                };
                FileExtensions = new[] {
                    "cshtml"
                };

                _mappings = (from type in assembly.GetTypes()
                    where typeof(WebPageRenderingBase).IsAssignableFrom(type)
                    let pageVirtualPath = type.GetCustomAttributes(inherit: false).OfType<PageVirtualPathAttribute>().FirstOrDefault()
                    where pageVirtualPath != null
                    select new KeyValuePair<string, Type>(CombineVirtualPaths(this._baseVirtualPath, pageVirtualPath.VirtualPath), type)
                ).ToDictionary(t => t.Key, t => t.Value, StringComparer.OrdinalIgnoreCase);
                this._viewPageActivator = new ViewPageActivator();
            }

            protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
            {
                return Exists(virtualPath);
            }

            protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
            {
                return CreateViewInternal(partialPath, masterPath: null, runViewStartPages: false);
            }

            protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
            {
                if (masterPath != null)
                    throw new ArgumentException("Using a custom master template path is not supported - see http://razorgenerator.codeplex.com/SourceControl/latest#RazorGenerator.Mvc/PrecompiledMvcView.cs for possible implementation if you need it");
                return CreateViewInternal(viewPath, masterPath, runViewStartPages: true);
            }

            private IView CreateViewInternal(string viewPath, string masterPath, bool runViewStartPages)
            {
                Type type;
                return _mappings.TryGetValue(viewPath, out type) ? new PrecompiledView(viewPath, type, runViewStartPages, FileExtensions, this, _viewPageActivator) : null;
            }

            public object CreateInstance(string virtualPath)
            {
                Type type;
                return _mappings.TryGetValue(virtualPath, out type) ? _viewPageActivator.Create(null, type) : null;
            }

            public IView CreateViewInstance(string virtualPath)
            {
                Type type;
                return _mappings.TryGetValue(virtualPath, out type) ? new PrecompiledView(virtualPath, type, true, FileExtensions, this, _viewPageActivator) : null;
            }

            public bool Exists(string virtualPath)
            {
                return _mappings.ContainsKey(virtualPath);
            }

            private static string NormalizeBaseVirtualPath(string virtualPath)
            {
                if (!string.IsNullOrEmpty(virtualPath))
                {
                    // For a virtual path to combine properly, it needs to start with a ~/ and end with a /.
                    if (!virtualPath.StartsWith("~/", StringComparison.Ordinal))
                    {
                        virtualPath = "~/" + virtualPath;
                    }
                    if (!virtualPath.EndsWith("/", StringComparison.Ordinal))
                    {
                        virtualPath += "/";
                    }
                }
                return virtualPath;
            }

            private static string CombineVirtualPaths(string baseVirtualPath, string virtualPath)
            {
                return !string.IsNullOrEmpty(baseVirtualPath) ? VirtualPathUtility.Combine(baseVirtualPath, virtualPath.Substring(2)) : virtualPath;
            }
        }
    }

    internal class ContextHelper
    {
        public static HttpContext GetFakeContext()
        {
            var httpRequest = new HttpRequest("", "http://stackoverflow/", "");
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponse);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                new HttpStaticObjectsCollection(), 10, true,
                HttpCookieMode.AutoDetect,
                SessionStateMode.InProc, false);

            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null, CallingConventions.Standard,
                    new[] { typeof(HttpSessionStateContainer) },
                    null)
                .Invoke(new object[] { sessionContainer });

            return httpContext;
        }
    }
}