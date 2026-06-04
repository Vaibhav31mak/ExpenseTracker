namespace ExpenseTracker.Tests.XUnit;

public sealed class ExpenseRepositoryTests
{
    private readonly ExpenseDbContext _dbContext;
    private readonly ExpenseRepository _repository;
    private readonly Mock<ILogger<ExpenseRepository>> _loggerMock = new();

    public ExpenseRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ExpenseDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new ExpenseDbContext(options);
        _repository = new ExpenseRepository(_dbContext, _loggerMock.Object);
    }

    [Fact]
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
        Assert.Single(result);
        Assert.All(result,
            expense => Assert.Equal(
                ExpenseCategory.Food,
                expense.Category));
    }

    [Fact]
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
        Assert.Single(result);
        Assert.Equal(
            new DateOnly(2026, 1, 10),
            result[0].Date);
    }

    [Fact]
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
            2026, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Month);
        Assert.Equal(300m, result[0].Total);
        Assert.Equal(2, result[1].Month);
        Assert.Equal(300m, result[1].Total);
    }
}