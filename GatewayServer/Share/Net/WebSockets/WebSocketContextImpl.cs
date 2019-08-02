using System;
using System.Net;
using System.Net.WebSockets;
using System.Security.Principal;
using System.Collections.Generic;
using System.Collections.Specialized;

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


        public WebSocketContextImpl()
        {
            m_IsWebSocketRequest = false;
        }

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
    }
}
