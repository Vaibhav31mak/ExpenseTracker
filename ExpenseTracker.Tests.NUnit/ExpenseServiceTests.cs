namespace ExpenseTracker.Tests.NUnit;

[TestFixture]
public sealed class ExpenseServiceTests
{
    private readonly Mock<IExpenseRepository> _repositoryMock = new();
    private readonly Mock<ILogger<ExpenseService>> _loggerMock = new();
    private ExpenseService _service;

    [SetUp]
    public void Setup()
    {
        _service = new ExpenseService(
            _repositoryMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task GetAllAsync_Should_Return_All_Expenses()
    {
        // Arrange
        List<Expense> expenses =
        [
            new()
            {
                Id = 1,
                Title = "Food",
                Amount = 100,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Category = ExpenseCategory.Food
            }
        ];
        _repositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expenses);

        // Act
        var result = await _service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.That(result, Has.Exactly(1).Items);
        Assert.That(result[0].Title, Is.EqualTo("Food"));
    }

    [Test]
    public async Task GetByIdAsync_Should_Return_Expense_When_Found()
    {
        // Arrange
        Expense expense = new()
        {
            Id = 1,
            Title = "Travel",
            Amount = 500
        };
        _repositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expense);

        // Act 
        var result = await _service.GetByIdAsync(1, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
    }

    [Test]
    public async Task GetByIdAsync_Should_Return_Null_When_NotFound()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.GetByIdAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expense?)null);

        // Act
        var result =
            await _service.GetByIdAsync(
                100,
                CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByDateRangeAsync_Should_Throw_When_End_Is_Before_Start()
    {
        // Arrange
        DateOnly start = new(2026, 12, 31);
        DateOnly end = new(2026, 1, 1);

        // Act and Assert
        Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _service.GetByDateRangeAsync(
                    start,
                    end,
                    CancellationToken.None));
    }

    [Test]
    public async Task CreateAsync_Should_Return_Created_Expense()
    {
        // Arrange
        ExpenseRequest request =
            new(
                "Laptop",
                50000,
                DateOnly.FromDateTime(DateTime.Today),
                ExpenseCategory.Education,
                "College");
        Expense created = request.ToEntity(1);
        _repositoryMock
            .Setup(x => x.AddAsync(
                It.IsAny<Expense>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        // Act
        var result =
            await _service.CreateAsync(
                request,
                CancellationToken.None);

        // Arrange
        Assert.That(result.Id, Is.EqualTo(1));
    }

    [Test]
    public async Task UpdateAsync_Should_Return_Updated_Expense()
    {
        // Arrange
        ExpenseRequest request =
            new(
                "Updated",
                200,
                DateOnly.FromDateTime(DateTime.Today),
                ExpenseCategory.Food,
                null);
        Expense updated = request.ToEntity(1);
        _repositoryMock
            .Setup(x => x.UpdateAsync(
                It.IsAny<Expense>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(updated);

        // Act
        var result =
            await _service.UpdateAsync(
                1,
                request,
                CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task UpdateAsync_Should_Return_Null_When_NotFound()
    {
        // Arrange
        ExpenseRequest request =
            new(
                "Updated",
                200,
                DateOnly.FromDateTime(DateTime.Today),
                ExpenseCategory.Food,
                null);
        _repositoryMock
            .Setup(x => x.UpdateAsync(
                It.IsAny<Expense>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expense?)null);

        // Act
        var result =
            await _service.UpdateAsync(
                1,
                request,
                CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_Should_Return_True_When_Deleted()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.DeleteAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result =
            await _service.DeleteAsync(
                1,
                CancellationToken.None);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task DeleteAsync_Should_Return_False_When_NotFound()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.DeleteAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result =
            await _service.DeleteAsync(
                1,
                CancellationToken.None);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task GetMonthlySummaryAsync_Should_Return_Summary()
    {
        // Arrange
        IReadOnlyList<MonthlyExpenseSummary> summaries =
        [
            new(2026, 1, 1000)
        ];
        _repositoryMock
            .Setup(x => x.GetMonthlySummaryAsync(
                2026,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(summaries);

        // Act
        var result =
            await _service.GetMonthlySummaryAsync(
                2026,
                CancellationToken.None);

        // Assert
        Assert.That(result, Has.Exactly(1).Items);
        Assert.That(result[0].Total, Is.EqualTo(1000));
    }

    [Test]
    public void ToResponse_Should_Map_All_Fields()
    {
        // Arrange
        Expense expense = new()
        {
            Id = 1,
            Title = "Food",
            Amount = 100,
            Date = new DateOnly(2026, 1, 1),
            Category = ExpenseCategory.Food
        };

        // Act
        var response = expense.ToResponse();

        // Assert
        Assert.That(expense.Id, Is.EqualTo(response.Id));
    }
}
