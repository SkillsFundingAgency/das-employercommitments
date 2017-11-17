using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Views
{
    public abstract class ViewTestBase
    {
        protected string GetPartial(IHtmlDocument html, string selector, int index = 1)
        {
            return GetTextContent(html?.QuerySelectorAll(selector), index)
                   ?? string.Empty;
        }

        protected string GetPartial(IElement html, string selector, int index = 1)
        {
            return GetTextContent(html?.QuerySelectorAll(selector), index)
                   ?? string.Empty;
        }

        protected string GetAttribute(IHtmlDocument html, string selector, string attribute, int index = 1)
        {
            return html?.QuerySelectorAll(selector)[index - 1]?.GetAttribute(attribute);
        }

        protected IElement GetHtmlElement(IHtmlDocument html, string selector, int index = 1)
        {
            var element = html?.QuerySelectorAll(selector);

            if (element == null)
            {
                return null;
            }

            return element.Any() ? element[index - 1] : null;
        }

        protected string GetPartialWhere(IHtmlDocument html, string selector, string textContent)
        {
            return
                html?.QuerySelectorAll(selector)?
                    .First(m => m.TextContent.Contains(textContent))?
                    .TextContent?.Replace("\r", string.Empty)
                    .Trim() ?? string.Empty;
        }

        private static string GetTextContent(IHtmlCollection<IElement> querySelectorAll, int index)
        {
            return !querySelectorAll.Any() || querySelectorAll.Length < index ?
                null :
                querySelectorAll[index - 1]?.TextContent?.Replace("\r", string.Empty).Trim();
        }
    }
}