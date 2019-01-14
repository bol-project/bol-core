using Microsoft.AspNetCore.Mvc;
using Neo;
using Neo.Ledger;
using System.Linq;

namespace Bol.Api.Controllers
{
    [Route("api/blocks")]
    [ApiController]
    public class BlocksController : ControllerBase
    {
        [HttpGet]
        public ActionResult GetBlocks()
        {
            var blocks = Blockchain.Singleton.Store.GetBlocks().Find();
            var count = blocks.Count();
            return Ok(blocks);
        }

        [HttpGet("current")]
        public ActionResult GetCurrentBlock()
        {
            return Ok(Blockchain.Singleton.CurrentBlockHash.ToString());
        }

        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            return Ok(Blockchain.Singleton.GetBlock(UInt256.Parse(id)).ToJson());
        }
    }
}
