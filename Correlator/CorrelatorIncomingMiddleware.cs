using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Correlator
{
    public class CorrelatorIncomingMiddleware : IMiddleware
    {
        private readonly string _correlationHeader;
        private readonly Func<string> _generateCorrelationId;
        private readonly RequestCorrelationId _requestCorrelationId;

        public CorrelatorIncomingMiddleware(string correlationHeader, 
            Func<string> generateCorrelationId,
            RequestCorrelationId requestCorrelationId)
        {
            _correlationHeader = correlationHeader ?? throw new ArgumentNullException(nameof(correlationHeader));
            _generateCorrelationId = generateCorrelationId ?? throw new ArgumentNullException(nameof(generateCorrelationId));
            _requestCorrelationId = requestCorrelationId ?? throw new ArgumentNullException(nameof(requestCorrelationId));
        }
        
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var correlationId = context.Request.Headers[_correlationHeader];

            if (StringValues.IsNullOrEmpty(correlationId))
            {
                correlationId = new StringValues(_generateCorrelationId());
            }
            
            context.Response.Headers[_correlationHeader] = correlationId;
            context.TraceIdentifier = correlationId;
            _requestCorrelationId.CorrelationId = correlationId;

            if (next != null)
            {
                await next(context);
            }
        }
    }
}
