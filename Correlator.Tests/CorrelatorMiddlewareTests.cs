using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Correlator.Tests
{
    public class CorrelatorMiddlewareTests
    {
        private const string CorrelationId = "6676e3d4-ddf7-4fc0-9c06-3e25a6d0270d";
        
        [Fact]
        public async Task Given_Another_Request_Handler_In_The_Pipeline_When_Request_Is_Made_Then_The_Next_Handler_Is_Called()
        {
            var incomingMiddleware = new CorrelatorIncomingMiddlewareBuilder().Create();
            var httpContext = CreateHttpContext();
            var nextHandlerCalled = false;
            
            await incomingMiddleware.InvokeAsync(httpContext, (context) =>
            {
                nextHandlerCalled = true;
                return Task.CompletedTask;
            });
            
            Assert.True(nextHandlerCalled);
        }

        [Fact]
        public async Task Given_No_Next_Request_Handler_In_The_Pipeline_When_Request_Is_Made_Then_No_Exception_Is_Thrown()
        {
            var incomingMiddleware = new CorrelatorIncomingMiddlewareBuilder().Create();
            var httpContext = CreateHttpContext();

            var exception = await Record.ExceptionAsync(() => incomingMiddleware.InvokeAsync(httpContext, null));

            Assert.Null(exception);
        }
        
        [Fact]
        public async Task Given_A_Request_With_A_CorrelationId_When_Request_Is_Made_Then_The_Response_CorrelationId_Header_Is_Set()
        {
            var middlewareBuilder = new CorrelatorIncomingMiddlewareBuilder();
            var incomingMiddleware = middlewareBuilder.Create();
            var httpContext = CreateHttpContext();

            await incomingMiddleware.InvokeAsync(httpContext, null);
            
            Assert.Equal(CorrelationId, httpContext.Response.Headers[middlewareBuilder.CorrelationHeader]);
        }
        
        [Fact]
        public async Task Given_A_Request_With_A_CorrelationId_When_Request_Is_Made_Then_The_TraceIdentifier_Is_Set_To_CorrelationId()
        {
            var incomingMiddleware = new CorrelatorIncomingMiddlewareBuilder().Create();
            var httpContext = CreateHttpContext();

            await incomingMiddleware.InvokeAsync(httpContext, null);
            
            Assert.Equal(CorrelationId, httpContext.TraceIdentifier);
        }
        
        [Fact]
        public async Task Given_A_Request_With_No_CorrelationId_When_Request_Is_Made_Then_A_CorrelationId_Is_Generated()
        {
            const string generatedId = "9b9e68d7-32c4-49ad-8681-b186ae3e6d38";
            
            var middlewareBuilder = new CorrelatorIncomingMiddlewareBuilder()
                .WithCorrelatorIdGenerator(() => generatedId);
            
            var incomingMiddleware = middlewareBuilder.Create();
            var httpContext = CreateHttpContext(null);

            await incomingMiddleware.InvokeAsync(httpContext, null);
            
            Assert.Equal(generatedId, httpContext.Response.Headers[middlewareBuilder.CorrelationHeader]);
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