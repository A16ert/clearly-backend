using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using ClearlyApi.Services.Chat.Manager;

namespace ClearlyApi.Services.Chat
{
    public class ChatMessageHandler : WebSocketHandler
    {
        public ChatMessageHandler(ConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
        }

        public override async Task OnConnected(WebSocket socket, string userLogin)
        {

            await base.OnConnected(socket, userLogin);

            var socketId = WebSocketConnectionManager.GetId(socket);
            //await SendMessageToAllAsync($"{socketId} is now connected");
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var socketId = WebSocketConnectionManager.GetId(socket);


            await SendMessageToAllAsync(message);
        }
    }
}
