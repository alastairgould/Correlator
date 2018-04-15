using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Correlator.IntergrationTests
{
    [Route("Test")] 
    public class TestController : Controller
    {
        public static string CorrelationHeaderValue = "";
        
        private HttpClient _client;

        public TestController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("secondcall");
        }
        
        public async Task<IActionResult> FirstCall()
        {
            await _client.GetAsync("/Test/SecondCall");
            return Ok();
        }
        
        [Route("secondcall")]
        public async Task<IActionResult> SecondCall()
        {
            CorrelationHeaderValue = Request.Headers["X-Correlation-ID"];
            return Ok();
        }
    }
}