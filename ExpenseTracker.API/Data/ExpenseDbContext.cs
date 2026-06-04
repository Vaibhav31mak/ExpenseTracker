namespace ExpenseTracker.API.Data;

public sealed class ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : DbContext(options)
{
    public DbSet<Expense> Expenses => Set<Expense>();

    // Adding fluent APIs.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(expense => expense.Id);
            entity.HasIndex(expense => expense.Category);
            entity.Property(expense => expense.Title).HasMaxLength(200).IsRequired();
            entity.Property(expense => expense.Amount).HasPrecision(18, 2);
            entity.Property(expense => expense.Category).HasConversion<string>().HasMaxLength(50);
            entity.Property(expense => expense.Date).HasConversion(date => date.ToDateTime(TimeOnly.MinValue),
                    value => DateOnly.FromDateTime(value));

            entity.ToTable(table =>
            {
                table.HasCheckConstraint("Amount_NonNegative", "[Amount] > 0");
            });
        });
    }
}
