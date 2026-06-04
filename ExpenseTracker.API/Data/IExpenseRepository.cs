namespace ExpenseTracker.API.Data;

public interface IExpenseRepository
{
    Task<IReadOnlyList<Expense>> GetAllAsync(CancellationToken cancellationToken);
    Task<Expense?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Expense>> GetByCategoryAsync(ExpenseCategory category, CancellationToken cancellationToken);
    Task<IReadOnlyList<Expense>> GetByDateRangeAsync(DateOnly start, DateOnly end, CancellationToken cancellationToken);
    Task<IReadOnlyList<MonthlyExpenseSummary>> GetMonthlySummaryAsync(int year, CancellationToken cancellationToken);
    Task<Expense> AddAsync(Expense expense, CancellationToken cancellationToken);
    Task<Expense?> UpdateAsync(Expense expense, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
