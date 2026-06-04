namespace ExpenseTracker.API.Models;

public sealed class Expense
{
    public int Id { get; set; }
    public required string Title { get => field; set => field = value.Trim(); }
    public decimal Amount {
        get;
        set
        {
            if(value < 0)
            {
                throw new ArgumentException("Amount should not be less than 0");
            }
            // Used field keyword here introduced in C# 14 for encapsulation.
            field = value;
        } 
    }
    public DateOnly Date { get; set; }
    public string? Notes { get; set; }
    public ExpenseCategory Category { get; set; }
}
