using Microsoft.AspNetCore.Mvc;
using PUNDERO.Services;
using System.Collections.Generic;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesForecastingController : ControllerBase
    {
        private readonly SalesDataService _salesDataService;
        private readonly SalesForecasting _salesForecasting;

        public SalesForecastingController(SalesDataService salesDataService, SalesForecasting salesForecasting)
        {
            _salesDataService = salesDataService;
            _salesForecasting = salesForecasting;
        }

        [HttpGet("train")]
        public IActionResult TrainModel()
        {
            var salesData = _salesDataService.GetProductSalesData();
            _salesForecasting.TrainModel(salesData);
            return Ok("Model trained successfully.");
        }

        [HttpGet("forecast")]
        public ActionResult<List<float>> Forecast(int horizon = 10)
        {
            var forecast = _salesForecasting.Forecast(horizon);
            return Ok(forecast);
        }

    }
}
