namespace ExpenseTracker.API.Services;

// Using primary constructor introduced in C# 12 for readability.
public sealed class ExpenseService(IExpenseRepository repository, ILogger<ExpenseService> logger) : IExpenseService
{
    public async Task<IReadOnlyList<ExpenseResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var expenses = await repository.GetAllAsync(cancellationToken);

        // C# 12 collection expression and spread operator used here.
        return [.. expenses.Select(expense => expense.ToResponse())];
    }

    public async Task<ExpenseResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var expense = await repository.GetByIdAsync(id, cancellationToken);
        // Used C# 14 Extension members here.
        return expense?.ToResponse();
    }

    public async Task<IReadOnlyList<ExpenseResponse>> GetByCategoryAsync(ExpenseCategory category, CancellationToken cancellationToken)
    {
        var expenses = await repository.GetByCategoryAsync(category, cancellationToken);
        return [.. expenses.Select(expense => expense.ToResponse())];
    }

    public async Task<IReadOnlyList<ExpenseResponse>> GetByDateRangeAsync(DateOnly start, DateOnly end, CancellationToken cancellationToken)
    {
        if (end < start)
        {
            throw new ArgumentException("End date should after start date.");
        }

        var expenses = await repository.GetByDateRangeAsync(start, end, cancellationToken);
        return [.. expenses.Select(expense => expense.ToResponse())];
    }

    public async Task<IReadOnlyList<MonthlyExpenseSummary>> GetMonthlySummaryAsync(int year, CancellationToken cancellationToken)
    {
        var summaries = await repository.GetMonthlySummaryAsync(year, cancellationToken);
        logger.LogInformation("Monthly summary for year {Year}", year);
        return summaries;
    }

    public async Task<ExpenseResponse> CreateAsync(ExpenseRequest request, CancellationToken cancellationToken)
    {
        var expense = await repository.AddAsync(request.ToEntity(), cancellationToken);
        logger.LogInformation("Created expense {ExpenseId}", expense.Id);
        return expense.ToResponse();
    }

    public async Task<ExpenseResponse?> UpdateAsync(int id, ExpenseRequest request, CancellationToken cancellationToken)
    {
        var updated = await repository.UpdateAsync(request.ToEntity(id), cancellationToken);
        if (updated is null)
        {
            return null;
        }

        logger.LogInformation("Updated expense {ExpenseId}", id);
        return updated.ToResponse();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var deleted = await repository.DeleteAsync(id, cancellationToken);
        if (deleted)
        {
            logger.LogInformation("Deleted expense {ExpenseId}", id);
        }

        return deleted;
    }
}
