using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Quantum.QsLanguageServer;
using StreamJsonRpc;

namespace ServerRunner
{
    internal class StreamJsonRpcMiddleware
    {
        public StreamJsonRpcMiddleware(RequestDelegate next) { }

        public async Task Invoke(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var jsonRpcMessageHandler = new WebSocketMessageHandler(webSocket);

                QsLanguageServer server = new QsLanguageServer(jsonRpcMessageHandler);
                server.WaitForShutdown();
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}