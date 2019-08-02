using System;
using System.Threading;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Share.Net.WebSockets
{
    public sealed class WebSocketImpl : WebSocket
    {
        private WebSocketState m_State;
        private string m_CloseStatusDesc;
        private WebSocketCloseStatus? m_CloseStatus;
        private string m_SubProtocol;


        public override WebSocketState State { get { return m_State; } }
        public override string CloseStatusDescription { get { return m_CloseStatusDesc; } }
        public override WebSocketCloseStatus? CloseStatus { get { return m_CloseStatus; } }
        public override string SubProtocol { get { return m_SubProtocol; } }


        public WebSocketImpl()
        {
            m_State = WebSocketState.Closed;
            m_CloseStatusDesc = string.Empty;
            m_CloseStatus = null;
            m_SubProtocol = string.Empty;
        }


        public override void Abort()
        {
            throw new System.NotImplementedException();
        }

        public override Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }


        public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public override Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

    }
}
