namespace ExpenseTracker.API.Models;

// Used record for immutability and value equality. Best practice for DTOs.
// Records were introduced in C# 8.
public sealed record ExpenseResponse(int Id, string Title, decimal Amount, DateOnly Date,
    ExpenseCategory Category, string? Notes);
