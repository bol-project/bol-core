using Microsoft.AspNetCore.Mvc;
using Neo;
using Neo.Ledger;
using System.Linq;

namespace Bol.Api.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        [HttpGet]
        public ActionResult Get()
        {
            var transactions = Blockchain.Singleton.Store.GetTransactions().Find();
            var count = transactions.Count();
            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            return Ok(Blockchain.Singleton.GetTransaction(UInt256.Parse(id)).ToJson());
        }
    }
}
