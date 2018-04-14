using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Correlator
{
    public class CorrelatorMiddleware : IMiddleware
    {
        private readonly string _correlationHeader;
        private readonly Func<string> _generateCorrelationId;

        public CorrelatorMiddleware(string correlationHeader, Func<string> generateCorrelationId)
        {
            _correlationHeader = correlationHeader ?? throw new ArgumentNullException(nameof(correlationHeader));
            _generateCorrelationId = generateCorrelationId ?? throw new ArgumentNullException(nameof(generateCorrelationId));
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

            if (next != null)
            {
                await next(context);
            }
        }
    }
}
