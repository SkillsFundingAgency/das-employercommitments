using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using HtmlAgilityPack;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Views
{
    public static class HtmlAgilityPackExtensions
    {
        public static IHtmlDocument ToAngleSharp(this HtmlDocument document)
        {
            var html = document?.DocumentNode?.OuterHtml;
            return new HtmlParser().Parse(html ?? string.Empty);
        }
    }
}