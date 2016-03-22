using NLog;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotOPDS.Utils
{
    public abstract class MessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var corrId = string.Format("{0}{1}", DateTime.Now.Ticks, Thread.CurrentThread.ManagedThreadId);
            var requestInfo = string.Format("{0} {1}", request.Method, request.RequestUri);

            var requestMessage = Encoding.UTF8.GetString(await request.Content.ReadAsByteArrayAsync());
            requestMessage = request.Headers + requestMessage;

            await IncommingMessageAsync(corrId, requestInfo, requestMessage);

            var response = await base.SendAsync(request, cancellationToken);

            string responseMessage;

            if (response.IsSuccessStatusCode)
            {
                var respBytes = await response.Content.ReadAsByteArrayAsync();
                if (response.Content.Headers.ContentType?.MediaType == "application/atom+xml")
                    responseMessage = Encoding.UTF8.GetString(respBytes);
                else
                    responseMessage = string.Format("[BLOB SIZE: {0}]", respBytes.LongLength);
            }
            else
                responseMessage = response.ReasonPhrase;
            responseMessage = response.Content?.Headers + responseMessage;

            await OutgoingMessageAsync(corrId, requestInfo, responseMessage);

            return response;
        }

        protected abstract Task IncommingMessageAsync(string correlationId, string requestInfo, string message);
        protected abstract Task OutgoingMessageAsync(string correlationId, string requestInfo, string message);
    }

    public class MessageLoggingHandler : MessageHandler
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected override async Task IncommingMessageAsync(string correlationId, string requestInfo, string message)
        {
            await Task.Run(() =>
                logger.Trace("{0} - Request: {1}\r\n{2}", correlationId, requestInfo, message));
        }

        protected override async Task OutgoingMessageAsync(string correlationId, string requestInfo, string message)
        {
            await Task.Run(() =>
                logger.Trace("{0} - Response: {1}\r\n{2}", correlationId, requestInfo, message));
        }
    }
}
