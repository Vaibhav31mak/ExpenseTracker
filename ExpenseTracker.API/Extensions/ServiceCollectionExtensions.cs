namespace ExpenseTracker.API.Extensions;

// Extension members introduced in C# 14 used here.
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddExpenseTracker(
            IConfiguration configuration)
        {
            var connectionString =
                configuration.GetConnectionString("Expenses") ?? "Data Source=expenses.db";
            services.AddDbContext<ExpenseDbContext>(
                options => options.UseSqlServer(connectionString));
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddScoped<IExpenseRepository, ExpenseRepository>();

            // Used Keyed DI introduced in .NET 8.
            services.AddKeyedSingleton<TimeProvider>("system", TimeProvider.System);
            services.AddScoped<IExpenseService>(sp =>
                new ExpenseService(
                    sp.GetRequiredService<IExpenseRepository>(),
                    sp.GetRequiredService<ILogger<ExpenseService>>()));
            
            // Used .NET 7 TimeProvider.
            services.TryAddSingleton(TimeProvider.System);
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("fixed", limiter =>
                {
                    limiter.Window = TimeSpan.FromSeconds(10);
                    limiter.PermitLimit = 30;
                    limiter.QueueLimit = 10;
                });
            });
            services.AddOpenApi();
            services.AddEndpointsApiExplorer();

            return services;
        }
    }
}