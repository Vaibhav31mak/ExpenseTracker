namespace ExpenseTracker.API.Data;

// Used LINQ. Also used .NET 9 CountBy.
public sealed class ExpenseRepository(ExpenseDbContext dbContext, ILogger<ExpenseRepository> logger) : IExpenseRepository
{
    // Using Lock instead of an object introduced in c# 13.
    private readonly Lock _sync = new();

    public async Task<IReadOnlyList<Expense>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Expenses.AsNoTracking().OrderBy(expense => expense.Date).ToListAsync(cancellationToken);
    }

    public async Task<Expense?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Expenses.AsNoTracking().FirstOrDefaultAsync(expense => expense.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Expense>> GetByCategoryAsync(ExpenseCategory category, CancellationToken cancellationToken)
    {
        return await dbContext.Expenses.AsNoTracking()
            .Where(expense => expense.Category == category)
            .OrderBy(expense => expense.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Expense>> GetByDateRangeAsync(DateOnly start, DateOnly end, CancellationToken cancellationToken)
    {
        return await dbContext.Expenses.AsNoTracking()
            .Where(expense => expense.Date >= start && expense.Date <= end)
            .OrderBy(expense => expense.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MonthlyExpenseSummary>> GetMonthlySummaryAsync(
    int year,
    CancellationToken cancellationToken)
    {
        var expenses = await dbContext.Expenses
            .AsNoTracking()
            .Where(expense => expense.Date.Year == year)
            .ToListAsync(cancellationToken);

        // .NET 9 CountBy.
        var MonthlyExpensesCount = expenses.CountBy(e => e.Date.Month);
        foreach (var month in MonthlyExpensesCount)
        {
            logger.LogInformation("Month {Month} contains {Count} expenses", month.Key, month.Value);
        }

        return [.. expenses
            .GroupBy(expense => new
            {
                expense.Date.Year,
                expense.Date.Month
            })
            .Select(group => new MonthlyExpenseSummary(
                group.Key.Year,
                group.Key.Month,
                group.Sum(expense => expense.Amount))
            )
            .OrderBy(summary => summary.Month)];
    }

    public async Task<Expense> AddAsync(Expense expense, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            dbContext.Expenses.Add(expense);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return expense;
    }

    public async Task<Expense?> UpdateAsync(Expense expense, CancellationToken cancellationToken)
    {
        var existing = await dbContext.Expenses.FirstOrDefaultAsync(item => item.Id == expense.Id, cancellationToken);
        if (existing is null)
        {
            return null;
        }
        existing.Title = expense.Title;
        existing.Amount = expense.Amount;
        existing.Date = expense.Date;
        existing.Category = expense.Category;
        existing.Notes = expense.Notes;

        await dbContext.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var existing = await dbContext.Expenses.FirstOrDefaultAsync(expense => expense.Id == id, cancellationToken);
        if (existing is null)
        {
            return false;
        }
        dbContext.Expenses.Remove(existing);

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
