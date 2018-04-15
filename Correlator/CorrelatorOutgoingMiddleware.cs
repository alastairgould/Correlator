using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Correlator
{
    public class CorrelatorOutgoingMiddleware : DelegatingHandler
    {
        private readonly string _correlationHeader;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CorrelatorOutgoingMiddleware(string correlationHeader, 
            IHttpContextAccessor httpContextAccessor)
        {
            _correlationHeader = correlationHeader ?? throw new ArgumentNullException(nameof(correlationHeader));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestCorrelationId =
               (RequestCorrelationId) _httpContextAccessor.HttpContext.RequestServices.GetService(typeof(RequestCorrelationId));

            if (!String.IsNullOrWhiteSpace(requestCorrelationId.CorrelationId))
            {
                request.Headers.Add(_correlationHeader, requestCorrelationId.CorrelationId);
            }
            
            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}