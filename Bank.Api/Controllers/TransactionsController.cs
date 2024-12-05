using Bank.Services;
using Bank.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class TransactionsController(ITransactionService transactionService) : ControllerBase
{
    private readonly ITransactionService _transactionService = transactionService;

    [HttpPost]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(string), 500)]
    public async Task<IActionResult> TransferFunds([FromBody] TransferRequest model)
    {
        var result = await _transactionService.TransferFundsAsync(model);
        return result ? Ok("Transaction successful.") : StatusCode(500, "Transaction failed.");
    }
}