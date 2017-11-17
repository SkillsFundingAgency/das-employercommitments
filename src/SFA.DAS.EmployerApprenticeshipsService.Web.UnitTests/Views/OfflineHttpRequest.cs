using System;
using System.Collections.Specialized;
using System.IO;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Views
{
    internal class OfflineHttpRequest : HttpRequestBase
    {
        private readonly Uri _uri;

        private readonly HttpCookieCollection _cookies = new HttpCookieCollection();

        private readonly NameValueCollection _serverVariables = new NameValueCollection();

        private readonly HttpBrowserCapabilitiesBase _browserCapabilities = new OfflineBrowserCapabilities();

        public OfflineHttpRequest(Uri uri)
        {
            this._uri = uri;
        }

        public override bool IsLocal => false;

        public override string ApplicationPath => "/";

        public override NameValueCollection ServerVariables => _serverVariables;

        public override string RawUrl => Url.ToString();

        public override Uri Url => _uri;

        public override HttpCookieCollection Cookies => _cookies;

        public override HttpBrowserCapabilitiesBase Browser => _browserCapabilities;

        public override string UserAgent => "background";

        #region Not supported members

        private const string NotSupportedMessage =
            "This implementation of HttpRequestBase is used to execute precompiled MVC views outside of the context of an active HttpRuntime. All members that are known to be required while executing templates have been implemented. If the view being executed needs to access other members, consider adding implementation to BackgroundHttpRequest";

        public override string this[string key]
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override NameValueCollection QueryString
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override NameValueCollection Headers
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override string UserHostName => string.Empty;

        public override string UserHostAddress => string.Empty;

        public override string[] UserLanguages
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override Uri UrlReferrer => this.Url;

        public override UnvalidatedRequestValuesBase Unvalidated
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override void Abort()
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        public override byte[] BinaryRead(int count)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        public override Stream GetBufferedInputStream()
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        public override Stream GetBufferlessInputStream()
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        public override Stream GetBufferlessInputStream(bool disableMaxRequestLength)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        public override void InsertEntityBody()
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        public override void InsertEntityBody(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        public override int[] MapImageCoordinates(string imageFieldName)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        public override double[] MapRawImageCoordinates(string imageFieldName)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        public override string MapPath(string virtualPath)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        public override string MapPath(string virtualPath, string baseVirtualDir, bool allowCrossAppMapping)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        public override void ValidateInput()
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        public override void SaveAs(string filename, bool includeHeaders)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        public override string[] AcceptTypes
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override string AnonymousID
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override string AppRelativeCurrentExecutionFilePath
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override ChannelBinding HttpChannelBinding
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override HttpClientCertificate ClientCertificate
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override Encoding ContentEncoding
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
            set { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override string ContentType
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
            set { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override int ContentLength
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override string CurrentExecutionFilePath
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override string CurrentExecutionFilePathExtension
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override string FilePath
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override HttpFileCollectionBase Files
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override Stream Filter
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
            set { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override NameValueCollection Form
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override string HttpMethod => "GET";

        public override Stream InputStream
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override bool IsAuthenticated
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override WindowsIdentity LogonUserIdentity
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override bool IsSecureConnection
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override NameValueCollection Params
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override string Path
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override string PathInfo
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override string PhysicalApplicationPath
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override string PhysicalPath
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override ReadEntityBodyMode ReadEntityBodyMode
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override string RequestType
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
            set { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override System.Web.Routing.RequestContext RequestContext
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
            set { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override CancellationToken TimedOutToken
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        public override int TotalBytes
        {
            get { throw new NotSupportedException(NotSupportedMessage); }
        }

        #endregion
    }
}