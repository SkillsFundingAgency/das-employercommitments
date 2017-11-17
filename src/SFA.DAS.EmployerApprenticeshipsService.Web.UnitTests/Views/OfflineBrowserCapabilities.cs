using System.Web;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Views
{
    internal class OfflineBrowserCapabilities : HttpBrowserCapabilitiesBase
    {
        public override bool IsMobileDevice => false;

        public override bool Cookies => false;
    }
}