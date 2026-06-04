namespace ExpenseTracker.API.Models;

// Used record for immutability and value equality. Best practice for DTOs.
public sealed record ExpenseRequest(string Title, decimal Amount, DateOnly Date, 
    ExpenseCategory Category, string? Notes);