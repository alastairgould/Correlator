using System;

namespace Correlator.Tests
{
    public class CorrelatorIncomingMiddlewareBuilder
    {
        public string CorrelationHeader = "X-Correlation-ID";
        
        private Func<string> _generateCorrelatorId = () => null;
        private readonly RequestCorrelationId _requestCorrelationId = new RequestCorrelationId();  
            
        public CorrelatorIncomingMiddlewareBuilder WithCorrelatorIdGenerator(Func<string> generateCorrelatorId)
        {
            _generateCorrelatorId = generateCorrelatorId;
            return this;
        }

        public CorrelatorIncomingMiddleware Create()
        {
            return new CorrelatorIncomingMiddleware(CorrelationHeader, _generateCorrelatorId, _requestCorrelationId);
        }
    }
}