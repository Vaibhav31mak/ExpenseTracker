namespace ExpenseTracker.API.Extensions;

// Extension members introduced in C# 14 used here.
// Used extension members instead of automappers.
public static class ExpenseExtensions
{
    extension(Expense expense)
    {
        public ExpenseResponse ToResponse() 
        { 
            return new (expense.Id, expense.Title, expense.Amount, expense.Date,
                expense.Category, expense.Notes);
        }
    }

    extension(ExpenseRequest request)
    {
        public Expense ToEntity(int id = 0)
        {
            return new()
            {
                Id = id,
                Title = request.Title,
                Amount = request.Amount,
                Date = request.Date,
                Category = request.Category,
                Notes = request.Notes
            };
        }
    }

    extension(IEnumerable<Expense> expenses)
    {
        public decimal TotalAmount => expenses.Sum(x => x.Amount);
    }
}