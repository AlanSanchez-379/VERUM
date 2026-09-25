using Microsoft.AspNetCore.Mvc;
using Verum.Application.DTOs.Negocio;
using Verum.Application.Interfaces;

namespace Verum.Web.Controllers.Negocio;

[ApiController]
public abstract class NegocioBaseController : Controller
{
    private readonly IBusinessService _businessService;
    private readonly ICurrentBusinessService _currentBusiness;

    protected NegocioBaseController(IBusinessService businessService, ICurrentBusinessService currentBusiness)
    {
        _businessService = businessService;
        _currentBusiness = currentBusiness;
    }

    protected async Task<BusinessDto?> GetActiveBusinessAsync()
    {
        var businesses = await _businessService.GetAllAsync();
        if (businesses.Count == 0)
        {
            return null;
        }

        return businesses.FirstOrDefault(b => b.Id == _currentBusiness.BusinessId) ?? businesses[0];
    }
}
