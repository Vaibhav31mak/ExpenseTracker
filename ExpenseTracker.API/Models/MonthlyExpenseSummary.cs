namespace ExpenseTracker.API.Models;

// Used record for immutability and value equality. Best practice for DTOs.
public sealed record MonthlyExpenseSummary(int Year, int Month, decimal Total);
