namespace ExpenseTracker.API.Services;

public interface IExpenseService
{
    Task<IReadOnlyList<ExpenseResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<ExpenseResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<ExpenseResponse>> GetByCategoryAsync(ExpenseCategory category, CancellationToken cancellationToken);
    Task<IReadOnlyList<ExpenseResponse>> GetByDateRangeAsync(DateOnly start, DateOnly end, CancellationToken cancellationToken);
    Task<IReadOnlyList<MonthlyExpenseSummary>> GetMonthlySummaryAsync(int year, CancellationToken cancellationToken);
    Task<ExpenseResponse> CreateAsync(ExpenseRequest request, CancellationToken cancellationToken);
    Task<ExpenseResponse?> UpdateAsync(int id, ExpenseRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
