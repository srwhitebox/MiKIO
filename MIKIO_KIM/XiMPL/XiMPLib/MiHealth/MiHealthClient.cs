using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using XiMPLib.MiPHS;
using XiMPLib.xBinding;

namespace XiMPLib.MiHealth
{
    public class MiHealthClient
    {
        private WebSocket webSocket;

        public String url
        {
            get
            {
                String url = xcBinder.AppProperties.MiHealthDomain;
                //url = "http://104.199.128.74";
                if (String.IsNullOrEmpty(url))
                    return null;

                UriBuilder uriBuilder = new UriBuilder(url);
                uriBuilder.Scheme = uriBuilder.Scheme.EndsWith("s") ? "wss" : "ws";
                uriBuilder.Path += "/query";
                return uriBuilder.ToString();

            }
        }

        public MiHealthClient()
        {
            if (String.IsNullOrEmpty(url))
                return;

            webSocket = new WebSocket(url);
            
            webSocket.SetCookie(new WebSocketSharp.Net.Cookie("campusId", xcBinder.AppProperties.HospitalID));
            webSocket.SetCookie(new WebSocketSharp.Net.Cookie("mikioId", xcBinder.AppProperties.KioskID));
            webSocket.SetCookie(new WebSocketSharp.Net.Cookie("authkey", xcBinder.AppProperties.HospitalAuthKey));
            webSocket.OnOpen += WebSocket_OnOpen;
            webSocket.OnError += WebSocket_OnError;
            webSocket.OnMessage += WebSocket_OnMessage;
            webSocket.OnClose += WebSocket_OnClose;
        }

        public void open()
        {
            if (webSocket != null)
                webSocket.Connect();
        }

        private void WebSocket_OnOpen(object sender, EventArgs e)
        {
            xcBinder.Mikio.refresh();
        }

        private void WebSocket_OnClose(object sender, CloseEventArgs e)
        {
            Thread.Sleep(5000);
            if (webSocket != null)
                webSocket.ConnectAsync();
        }

        private void WebSocket_OnMessage(object sender, MessageEventArgs e)
        {
            xcMiHealthResponse response = new xcMiHealthResponse(e.Data);
            if (response.Code == 231)
            {
                xcBinder.Mikio.refresh();
            }
        }

        private void WebSocket_OnError(object sender, ErrorEventArgs e)
        {
        }
    }
}
