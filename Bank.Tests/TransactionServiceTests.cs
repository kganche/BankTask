using Bank.Data;
using Bank.Data.Models;
using Bank.Services;
using Bank.Services.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Bank.Tests;

public class TransactionServiceTests
{
    private readonly TransactionService _sut;
    private readonly BankDbContext _dbContext;
    private readonly DbSet<BankAccount> _bankAccounts;

    public TransactionServiceTests()
    {
        var options = new DbContextOptionsBuilder<BankDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _dbContext = A.Fake<BankDbContext>(x => x.WithArgumentsForConstructor(() => new BankDbContext(options)));
        _bankAccounts = A.Fake< DbSet<BankAccount>>();
        _sut = new TransactionService(_dbContext);
    }
    
    [Fact]
    public async Task TransferFundsAsync_ShouldRollbackTransaction_OnException()
    {
        var sender = new BankAccount { Id = 1, Balance = 1000, LockedBalance = 500 };
        var recipient = new BankAccount { Id = 2, Balance = 1000, LockedBalance = 500 };
        
        _bankAccounts.AddRange(sender, recipient);
        await _dbContext.SaveChangesAsync();

        A.CallTo(() => _dbContext.BankAccounts).Returns(_bankAccounts);
        A.CallTo(() => _bankAccounts.FindAsync(1)).Returns(sender);
        A.CallTo(() => _bankAccounts.FindAsync(2)).Returns(recipient);
        A.CallTo(() => _dbContext.SaveChangesAsync(default)).Throws<DbUpdateException>();

        var model = new TransferRequest() { SenderAccountId = 1, RecipientAccountId = 2, TransactionAmount = 100 };

        var result = await _sut.TransferFundsAsync(model);

        result.Should().BeFalse();
    }
    
    [Fact]
    public async Task TransferFundsAsync_ShouldCompleteTransaction_Successfully()
    {
        var sender = new BankAccount { Id = 1, Balance = 1000, LockedBalance = 500 };
        var recipient = new BankAccount { Id = 2, Balance = 1000, LockedBalance = 500 };
        
        _bankAccounts.AddRange(sender, recipient);
        await _dbContext.SaveChangesAsync();
        
        A.CallTo(() => _dbContext.BankAccounts).Returns(_bankAccounts);
        A.CallTo(() => _bankAccounts.FindAsync(1)).Returns(sender);
        A.CallTo(() => _bankAccounts.FindAsync(2)).Returns(recipient);

        var model = new TransferRequest { SenderAccountId = 1, RecipientAccountId = 2, TransactionAmount = 100 };

        var result = await _sut.TransferFundsAsync(model);

        result.Should().BeTrue();
        sender.Balance.Should().Be(1100);
        sender.LockedBalance.Should().Be(400);
        recipient.Balance.Should().Be(900);
        recipient.LockedBalance.Should().Be(600);
    }
}