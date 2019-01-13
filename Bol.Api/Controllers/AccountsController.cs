using Microsoft.AspNetCore.Mvc;
using Neo;
using Neo.Ledger;
using System.Linq;

namespace Bol.Api.Controllers
{
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        [HttpGet]
        public ActionResult GetAccounts()
        {
            var accounts = Blockchain.Singleton.Store.GetAccounts().Find();
            var count = accounts.Count();
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public ActionResult GetAccount(string id)
        {
            var account = Blockchain.Singleton.Store.GetAccounts().TryGet(UInt160.Parse(id));
            return Ok(account);
        }
    }
}
