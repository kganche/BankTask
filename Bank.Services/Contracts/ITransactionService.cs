using Bank.Services.Models;

namespace Bank.Services;

public interface ITransactionService
{
    Task<bool> TransferFundsAsync(TransferRequest request);
}