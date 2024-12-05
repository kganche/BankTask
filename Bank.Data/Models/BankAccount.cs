namespace Bank.Data.Models;

public class BankAccount
{
    public int Id { get; set; }

    public decimal Balance { get; set; }

    public decimal LockedBalance { get; set; }
}