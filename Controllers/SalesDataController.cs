using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[Route("api/[controller]")]
[ApiController]
public class SalesDataController : ControllerBase
{
    private readonly SalesDataRepository _salesDataRepository;

public SalesDataController(SalesDataRepository salesDataRepository)
    {
        _salesDataRepository = salesDataRepository;
    }

    [HttpGet]
    public ActionResult<List<SalesData>> GetSalesData()
    {
        var data = _salesDataRepository.GetSalesData();
        return Ok(data);
    }
}