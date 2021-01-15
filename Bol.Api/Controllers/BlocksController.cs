using System;
using Bol.Api.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Bol.Api.Controllers
{
    [Route("api/blocks")]
    [ApiController]
    public class BlocksController : ControllerBase
    {
        private readonly IBlockChainService _blockChainService;

        public BlocksController(IBlockChainService blockChainService)
        {
            _blockChainService = blockChainService ?? throw new ArgumentNullException(nameof(blockChainService));
        }

        [HttpGet]
        public ActionResult GetBlocks()
        {
            var result = _blockChainService.GetBlocks();

            return Ok(result);
        }

        [HttpGet("current")]
        public ActionResult GetCurrentBlock()
        {
            var result = _blockChainService.GetCurrentBlock();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult Get([FromRoute] string id)
        {
            var result = _blockChainService.GetBlock(id);

            return Ok(result);
        }
    }
}
