using Bank.Data;
using Bank.Services.Models;

namespace Bank.Services;

public class TransactionService(BankDbContext dbContext) : ITransactionService
{
    private readonly BankDbContext _dbContext = dbContext;

    public async Task<bool> TransferFundsAsync(TransferRequest request)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var sender = await dbContext.BankAccounts.FindAsync(request.SenderAccountId);
            var recipient = await dbContext.BankAccounts.FindAsync(request.RecipientAccountId);

            if (sender == null || recipient == null)
            {
                return false;
            }

            sender.LockedBalance -= request.TransactionAmount;
            recipient.LockedBalance += request.TransactionAmount;
            recipient.Balance -= request.TransactionAmount;
            sender.Balance += request.TransactionAmount;

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return false;
        }
    }
}