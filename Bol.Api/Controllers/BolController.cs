using Bol.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Bol.Api.Controllers
{
    [Route("api/bol")]
    public class BolController : ControllerBase
    {
        private IBolService _bolService;

        public BolController(IBolService bolService)
        {
            _bolService = bolService ?? throw new ArgumentNullException(nameof(bolService));
        }

        [HttpPost("register")]
        public ActionResult Register()
        {
            var result = _bolService.Register();
            return Ok(result);
        }

        [HttpPost("claim")]
        public ActionResult Claim()
        {
            var result = _bolService.Claim();
            return Ok(result);
        }

        [HttpGet("decimals")]
        public ActionResult Decimals()
        {
            var result = _bolService.Decimals();
            return Ok(result);
        }
    }
}
