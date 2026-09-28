using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Domain.Entities;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Negocio;

[Route("negocio/[controller]")]
public class TransferenciasController : NegocioBaseController
{
    private readonly IAccountService _personalAccountService;
    private readonly IBusinessAccountService _businessAccountService;
    private readonly ITransferService _transferService;

    public TransferenciasController(
        IBusinessService businessService,
        ICurrentBusinessService currentBusiness,
        IAccountService personalAccountService,
        IBusinessAccountService businessAccountService,
        ITransferService transferService)
        : base(businessService, currentBusiness)
    {
        _personalAccountService = personalAccountService;
        _businessAccountService = businessAccountService;
        _transferService = transferService;
    }

    public async Task<IActionResult> Index()
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return Redirect("/negocio");
        }

        var vm = new NegocioTransferenciasViewModel
        {
            Business = business,
            PersonalAccounts = await _personalAccountService.GetAccountsAsync(),
            BusinessAccounts = await _businessAccountService.GetAllAsync(business.Id),
            RecentTransfers = await _transferService.GetRecentAsync(business.Id)
        };

        return View(vm);
    }

    public class TransferRequest
    {
        public Guid PersonalAccountId { get; set; }
        public Guid BusinessAccountId { get; set; }
        public string Direction { get; set; } = "to_business";
        public decimal Amount { get; set; }
        public string Note { get; set; } = string.Empty;
    }

    [HttpPost("transferir")]
    public async Task<IActionResult> Transferir([FromBody] TransferRequest request)
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return BadRequest(new { error = "No hay un negocio activo." });
        }

        if (request.PersonalAccountId == Guid.Empty || request.BusinessAccountId == Guid.Empty)
        {
            return BadRequest(new { error = "Elegí una cuenta personal y una cuenta del negocio." });
        }

        var direction = request.Direction == "to_personal" ? TransferDirection.ToPersonal : TransferDirection.ToBusiness;

        var result = await _transferService.TransferAsync(
            business.Id,
            request.PersonalAccountId,
            request.BusinessAccountId,
            direction,
            request.Amount,
            request.Note?.Trim() ?? string.Empty);

        if (!result.Success)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok();
    }
}
