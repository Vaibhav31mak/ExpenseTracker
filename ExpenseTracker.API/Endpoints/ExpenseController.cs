namespace ExpenseTracker.API.Endpoints;

[ApiController]
[Route("api/[controller]")]
// Used C# 12 primary constructor here.
public class ExpensesController(IExpenseService expenseService) : ControllerBase
{
    private readonly IExpenseService _expenseService = expenseService;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var expenses = await _expenseService.GetAllAsync(cancellationToken);
        return Ok(expenses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest("Expense Id should not be negative or zero");
        }
        var expense = await _expenseService.GetByIdAsync(id, cancellationToken);
        if (expense is null)
        {
            return NotFound();
        }

        return Ok(expense);
    }

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(ExpenseCategory category, CancellationToken cancellationToken)
    {
        var expenses = await _expenseService.GetByCategoryAsync(category, cancellationToken);

        return Ok(expenses);
    }

    [HttpGet("range")]
    public async Task<IActionResult> GetByDateRange(DateOnly start, DateOnly end, CancellationToken cancellationToken)
    {
        if (end < start)
        {
            return BadRequest("End date should grater than start date.");
        }
        var expenses = await _expenseService.GetByDateRangeAsync(start, end, cancellationToken);

        return Ok(expenses);
    }

    [HttpGet("summary/{year}")]
    public async Task<IActionResult> GetMonthlySummary(int year, CancellationToken cancellationToken)
    {
        if (year < 2020 || year > 2026)
        {
            return BadRequest("Year should be in between 2020 and 2026");
        }
        var summary = await _expenseService.GetMonthlySummaryAsync(year, cancellationToken);

        return Ok(summary);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ExpenseRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest("Please enter title.");
        }
        if (request.Amount <= 0)
        {
            return BadRequest("Amount should not be negative or zero");
        }
        var expense = await _expenseService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = expense.Id }, expense);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ExpenseRequest request, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest("Expense id should be non negative.");
        }
        var expense = await _expenseService.UpdateAsync(id, request, cancellationToken);
        if (expense is null)
        {
            return NotFound();
        }

        return Ok(expense);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest("Expense id should be non negative.");
        }
        var deleted = await _expenseService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}