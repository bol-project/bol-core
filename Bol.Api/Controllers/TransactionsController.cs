using System;
using Bol.Api.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Bol.Api.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
        }

        [HttpGet]
        public ActionResult Get()
        {
            var result = _transactionService.GetTransactions();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult Get([FromRoute]string id)
        {
            var result = _transactionService.GetTransaction(id);

            return Ok(result);
        }
    }
}
