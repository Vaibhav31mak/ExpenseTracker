namespace ExpenseTracker.API.Data;

// Seeded data for testing.
public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ExpenseDbContext>();
        await context.Database.MigrateAsync(cancellationToken);
        if (await context.Expenses.AnyAsync(cancellationToken))
        {
            return;
        }
        // Latest TimeProvider and DateOnly used here.
        var today = DateOnly.FromDateTime(TimeProvider.System.GetLocalNow().DateTime);
        Expense[] seedExpenses =
        [
            new Expense
            {
                Title = "Lunch",
                Amount = 120m,
                Date = today.AddDays(-2),
                Category = ExpenseCategory.Food,
                Notes = "Lunch time!"
            },
            new Expense
            {
                Title = "Train Travel",
                Amount = 450m,
                Date = today.AddDays(-10),
                Category = ExpenseCategory.Travel,
                Notes = "Train time!"
            },
            new Expense
            {
                Title = "Electricity bill",
                Amount = 800m,
                Date = today.AddDays(-15),
                Category = ExpenseCategory.Bills,
                Notes = "I hate bills!"
            }
        ];

        context.Expenses.AddRange(seedExpenses);
        await context.SaveChangesAsync(cancellationToken);
    }
}
