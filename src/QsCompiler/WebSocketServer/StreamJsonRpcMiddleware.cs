using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Quantum.QsLanguageServer;
using StreamJsonRpc;

namespace WebSocketServer
{
    internal class StreamJsonRpcMiddleware
    {
        public StreamJsonRpcMiddleware(RequestDelegate _) { }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var jsonRpcMessageHandler = new WebSocketMessageHandler(webSocket);

                var server = new QsLanguageServer(jsonRpcMessageHandler);
                server.WaitForShutdown();
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
