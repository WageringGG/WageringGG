using Microsoft.AspNetCore.Mvc;
using stellar = stellar_dotnet_sdk;

namespace WageringGG.Server.Handlers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StellarController : ControllerBase
    {
        private readonly stellar.Server _server;
        public StellarController(stellar.Server server)
        {
            _server = server;
        }

        [HttpGet]
        public IActionResult RequestChallenge([FromQuery] string account)
        {
            //need server keys here
            stellar.WebAuthentication.BuildChallengeTransaction(null, account, "Wagering.GG");
            return Ok();
        }
    }
}