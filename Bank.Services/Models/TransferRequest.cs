namespace Bank.Services.Models;

public class TransferRequest
{
    public int SenderAccountId { get; set; }
    
    public int RecipientAccountId { get; set; }
    
    public decimal TransactionAmount { get; set; }
}