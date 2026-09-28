using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Personal;

[ApiController]
[Route("personal/[controller]")]
public class CompromisosController : Controller
{
    private readonly ICommitmentService _commitmentService;
    private readonly IAccountService _accountService;

    public CompromisosController(ICommitmentService commitmentService, IAccountService accountService)
    {
        _commitmentService = commitmentService;
        _accountService = accountService;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new CompromisosViewModel
        {
            Commitments = await _commitmentService.GetCurrentPeriodAsync(),
            Accounts = await _accountService.GetAccountsAsync()
        };
        return View(vm);
    }

    public class MarkPaidRequest
    {
        public Guid AccountId { get; set; }
    }

    [HttpPost("{id:guid}/pagar")]
    public async Task<IActionResult> Pagar(Guid id, [FromBody] MarkPaidRequest request)
    {
        if (request.AccountId == Guid.Empty)
        {
            return BadRequest(new { error = "Elegí de qué cuenta sale la plata." });
        }

        var result = await _commitmentService.MarkPaidAsync(id, request.AccountId);
        if (!result.Success)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok();
    }
}
