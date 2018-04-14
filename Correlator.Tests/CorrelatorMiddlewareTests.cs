using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Correlator.Tests
{
    public class CorrelatorMiddlewareTests
    {
        private const string CorrelationId = "6676e3d4-ddf7-4fc0-9c06-3e25a6d0270d";
        private const string CorrelationHeader = "X-Correlation-ID";
        
        [Fact]
        public async Task Given_Another_Request_Handler_In_The_Pipeline_When_Request_Is_Made_Then_The_Next_Handler_Is_Called()
        {
            var middleware = new CorrelatorMiddleware(CorrelationHeader, () => null);
            var httpContext = CreateHttpContext();
            var nextHandlerCalled = false;
            
            await middleware.InvokeAsync(httpContext, (context) =>
            {
                nextHandlerCalled = true;
                return Task.CompletedTask;
            });
            
            Assert.Equal(true, nextHandlerCalled);
        }
        
        [Fact]
        public async Task Given_No_Next_Request_Handler_In_The_Pipeline_When_Request_Is_Made_Then_No_Exception_Is_Thrown()
        {
            var middleware = new CorrelatorMiddleware(CorrelationHeader, () => null);
            var httpContext = CreateHttpContext();

            var exception = await Record.ExceptionAsync(() => middleware.InvokeAsync(httpContext, null));

            Assert.Null(exception);
        }
        
        [Fact]
        public async Task Given_A_Request_With_A_CorrelationId_When_Request_Is_Made_Then_The_Response_CorrelationId_Header_Is_Set()
        {
            var middleware = new CorrelatorMiddleware(CorrelationHeader, () => null);
            var httpContext = CreateHttpContext();

            await middleware.InvokeAsync(httpContext, null);
            
            Assert.Equal(CorrelationId, httpContext.Response.Headers[CorrelationHeader]);
        }
        
        [Fact]
        public async Task Given_A_Request_With_A_CorrelationId_When_Request_Is_Made_Then_The_TraceIdentifier_Is_Set_To_CorrelationId()
        {
            var middleware = new CorrelatorMiddleware(CorrelationHeader, () => null);
            var httpContext = CreateHttpContext();

            await middleware.InvokeAsync(httpContext, null);
            
            Assert.Equal(CorrelationId, httpContext.TraceIdentifier);
        }
        
        [Fact]
        public async Task Given_A_Request_With_No_CorrelationId_When_Request_Is_Made_Then_A_CorrelationId_Is_Generated()
        {
            var generatedId = "9b9e68d7-32c4-49ad-8681-b186ae3e6d38";
            var middleware = new CorrelatorMiddleware(CorrelationHeader, () => generatedId);
            var httpContext = CreateHttpContext(null);

            await middleware.InvokeAsync(httpContext, null);
            
            Assert.Equal(generatedId, httpContext.Response.Headers[CorrelationHeader]);
        }
        
        private static DefaultHttpContext CreateHttpContext(string correlationId = CorrelationId)
        {
            var httpContext = new DefaultHttpContext();

            if (correlationId != null)
            {
                httpContext.Request.Headers["X-Correlation-ID"] = correlationId;
            }
            
            return httpContext;
        }
    }
}