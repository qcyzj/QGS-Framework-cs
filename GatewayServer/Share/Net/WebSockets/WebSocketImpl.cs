//using System;
//using System.Threading;
//using System.Net.WebSockets;
//using System.Threading.Tasks;

//namespace Share.Net.WebSockets
//{
//    public sealed class WebSocketImpl : WebSocket
//    {
//        private WebSocketState m_State;
//        private string m_CloseStatusDesc;
//        private WebSocketCloseStatus? m_CloseStatus;
//        private string m_SubProtocol;
//        private CancellationTokenSource m_Source;


//        public override WebSocketState State { get { return m_State; } }
//        public override string CloseStatusDescription { get { return m_CloseStatusDesc; } }
//        public override WebSocketCloseStatus? CloseStatus { get { return m_CloseStatus; } }
//        public override string SubProtocol { get { return m_SubProtocol; } }


//        public WebSocketImpl(string sub_protocol, CancellationTokenSource source)
//        {
//            m_State = WebSocketState.Connecting;
//            m_CloseStatusDesc = string.Empty;
//            m_CloseStatus = null;
//            m_SubProtocol = sub_protocol;
//            m_Source = source;
//        }


//        public override void Abort()
//        {
//            m_State = WebSocketState.Aborted;
//            m_Source.Cancel();
//        }

//        public override async Task CloseAsync(WebSocketCloseStatus closeStatus, 
//                                              string statusDescription, 
//                                              CancellationToken cancellationToken)
//        {
//            if (WebSocketState.Open == m_State)
//            {

//                m_State = WebSocketState.CloseSent;
//            }
//        }

//        public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, 
//                                              string statusDescription, 
//                                              CancellationToken cancellationToken)
//        {
//            throw new System.NotImplementedException();
//        }

//        public override void Dispose()
//        {
//            if (WebSocketState.Open == m_State)
//            {

//            }
//        }


//        public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }
//        public override Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//    }
//}
