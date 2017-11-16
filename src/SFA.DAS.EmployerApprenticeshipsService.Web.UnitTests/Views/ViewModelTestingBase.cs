using System;
using System.Web.Mvc;
using FluentAssertions;
using HtmlAgilityPack;
using NUnit.Framework;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Views
{
    public abstract class ViewModelTestingBase<TModel, TView> : ViewTestBase where TView: WebViewPage<OrchestratorResponse<TModel>> where TModel : class, new()
    {
        private ViewRenderer _viewRenderer;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _viewRenderer = ViewRenderer.ForAssemblyOf<TView>();
        }

        protected void AssertParsedContent(TModel model, string cssTag, string expectedValue)
        {
            var response = new OrchestratorResponse<TModel> {Data = model};
            var context = _viewRenderer.RenderView<TView>(response, new Uri("http://127.0.0.1"));

            var doc = new HtmlDocument();

            doc.LoadHtml(context);

            var parsedContent = doc.ToAngleSharp();

            if (string.IsNullOrEmpty(expectedValue))
            {
                GetPartial(parsedContent, cssTag).Should().BeEmpty();
            }
            else
            {
                GetPartial(parsedContent, cssTag).Should().Contain(expectedValue);
            }
        }
    }
}