
namespace ExpenseTracker.Tests.NUnit;

[TestFixture]
public sealed class ExpenseRepositoryTests
{
    private ExpenseDbContext _dbContext = null!;
    private ExpenseRepository _repository = null!;
    private readonly Mock<ILogger<ExpenseRepository>> _loggerMock = new();

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ExpenseDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ExpenseDbContext(options);
        _repository = new ExpenseRepository(_dbContext, _loggerMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test]
    public async Task GetByCategoryAsync_Should_Return_Only_Food_Expenses()
    {
        // Arrange
        _dbContext.Expenses.AddRange(
            new Expense
            {
                Title = "Pizza",
                Amount = 500,
                Date = new DateOnly(2026, 1, 1),
                Category = ExpenseCategory.Food
            },
            new Expense
            {
                Title = "Bus Ticket",
                Amount = 100,
                Date = new DateOnly(2026, 1, 2),
                Category = ExpenseCategory.Travel
            });
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCategoryAsync(
            ExpenseCategory.Food,
            CancellationToken.None);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(
            result.All(expense => expense.Category == ExpenseCategory.Food),
            Is.True);
    }

    [Test]
    public async Task GetByDateRangeAsync_Should_Return_Expenses_Within_Date_Range()
    {
        // Arrange
        _dbContext.Expenses.AddRange(
            new Expense
            {
                Title = "Expense1",
                Amount = 100,
                Date = new DateOnly(2026, 1, 10),
                Category = ExpenseCategory.Food
            },
            new Expense
            {
                Title = "Expense2",
                Amount = 200,
                Date = new DateOnly(2026, 3, 10),
                Category = ExpenseCategory.Travel
            });
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByDateRangeAsync(
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 1, 31),
            CancellationToken.None);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(
            result[0].Date,
            Is.EqualTo(new DateOnly(2026, 1, 10)));
    }

    [Test]
    public async Task GetMonthlySummaryAsync_Should_Group_Expenses_By_Month()
    {
        // Arrange
        _dbContext.Expenses.AddRange(
            new Expense
            {
                Title = "Expense1",
                Amount = 100,
                Date = new DateOnly(2026, 1, 5),
                Category = ExpenseCategory.Food
            },
            new Expense
            {
                Title = "Expense2",
                Amount = 200,
                Date = new DateOnly(2026, 1, 15),
                Category = ExpenseCategory.Travel
            },
            new Expense
            {
                Title = "Expense3",
                Amount = 300,
                Date = new DateOnly(2026, 2, 10),
                Category = ExpenseCategory.Food
            });
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetMonthlySummaryAsync(
            2026,
            CancellationToken.None);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Month, Is.EqualTo(1));
        Assert.That(result[0].Total, Is.EqualTo(300m));
        Assert.That(result[1].Month, Is.EqualTo(2));
        Assert.That(result[1].Total, Is.EqualTo(300m));
    }
}