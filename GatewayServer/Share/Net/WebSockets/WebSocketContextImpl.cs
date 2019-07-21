using System;
using System.Net;
using System.Net.WebSockets;
using System.Security.Principal;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Linq;

namespace Share.Net.WebSockets
{
    public sealed class WebSocketContextImpl : WebSocketContext
    {
        private CookieCollection m_CookieCollection;
        private NameValueCollection m_Headers;
        private bool m_IsAuthenticated;
        private bool m_IsLocal;
        private bool m_IsSecureConnection;
        private string m_Orign;
        private Uri m_RequestUri;
        private string m_SecWebSocketKey;
        private IEnumerable<string> m_SecWebSocketProtocols;
        private string m_SecWebSocketVersion;
        private IPrincipal m_User;
        private WebSocket m_WebSocket;

        private bool m_IsWebSocketRequest;
        private string m_SubProtocol;


        public override CookieCollection CookieCollection { get { return m_CookieCollection; } }
        public override NameValueCollection Headers { get { return m_Headers; } }
        public override bool IsAuthenticated { get { return m_IsAuthenticated; } }
        public override bool IsLocal { get { return m_IsLocal; } }
        public override bool IsSecureConnection { get { return m_IsSecureConnection; } }
        public override string Origin { get { return m_Orign; } }
        public override Uri RequestUri { get { return m_RequestUri; } }
        public override string SecWebSocketKey { get { return m_SecWebSocketKey; } }
        public override IEnumerable<string> SecWebSocketProtocols { get { return m_SecWebSocketProtocols; } }
        public override string SecWebSocketVersion { get { return m_SecWebSocketVersion; } }
        public override IPrincipal User { get { return m_User; } }
        public override WebSocket WebSocket { get { return m_WebSocket; } }
        public bool IsWebSocketRequest { get { return m_IsWebSocketRequest; } }


        public WebSocketContextImpl(string http_header)
        {
            m_Orign = http_header;

            ParseHttpHeader(http_header);

            m_WebSocket = new WebSocketImpl(m_SubProtocol);
        }


        private void ParseHttpHeader(string http_header)
        {
            string value = string.Empty;

            if (WebSocketHttpHelper.ParseHttpHeaderUriField(http_header, out value))
            {
                m_RequestUri = new Uri(value);
            }

            if (WebSocketHttpHelper.ParseHttpHeaderHostField(http_header, out value))
            {
                m_Headers.Add("Host", value);
            }

            if (WebSocketHttpHelper.ParseHttpHeaderUpgradeField(http_header, out value))
            {
                m_Headers.Add("Upgrade", value);
                m_IsWebSocketRequest = ("websocket" == value);
            }

            if (WebSocketHttpHelper.ParseHttpHeaderConnectionField(http_header, out value))
            {
                m_Headers.Add("Connection", value);
            }

            if (WebSocketHttpHelper.ParseHttpHeaderSecWebSocketKeyField(http_header, out value))
            {
                m_Headers.Add("Sec-WebSocket-Key", value);
                m_SecWebSocketKey = value;
            }

            if(WebSocketHttpHelper.ParseHttpHeaderSecWebSocketVersionField(http_header, out value))
            {
                m_Headers.Add("Sec-WebSocket-Version", value);
                m_SecWebSocketVersion = value;
            }

            if (WebSocketHttpHelper.ParseHttpHeaderSecWebSocketProtocolField(http_header, out value))
            {
                m_Headers.Add("Sec-WebSocket-Protocol", value);
                m_SubProtocol = value;
                m_SecWebSocketProtocols = 
                    value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();                 
            }
        }
    }
}
