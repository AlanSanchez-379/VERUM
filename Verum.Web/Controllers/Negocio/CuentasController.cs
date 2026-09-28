using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Negocio;

[Route("negocio/[controller]")]
public class CuentasController : NegocioBaseController
{
    private readonly IBusinessAccountService _accountService;

    public CuentasController(IBusinessService businessService, ICurrentBusinessService currentBusiness, IBusinessAccountService accountService)
        : base(businessService, currentBusiness)
    {
        _accountService = accountService;
    }

    public async Task<IActionResult> Index()
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return Redirect("/negocio");
        }

        var vm = new NegocioCuentasViewModel
        {
            Business = business,
            Accounts = await _accountService.GetAllAsync(business.Id)
        };

        return View(vm);
    }

    public class RegisterAccountRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegisterAccountRequest request)
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return BadRequest(new { error = "No hay un negocio activo." });
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { error = "Ingresá un nombre." });
        }

        await _accountService.CreateAsync(business.Id, request.Name.Trim(), request.Subtitle?.Trim() ?? string.Empty);
        return Ok();
    }
}
