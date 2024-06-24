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
            var salesData = _salesDataService.GetProductSalesData();
            var forecast = _salesForecasting.Forecast(horizon, salesData);
            return Ok(forecast);
        }

        [HttpGet("forecast-sum")]
        public ActionResult<List<float>> ForecastSum(int horizon = 10)
        {
            var salesData = _salesDataService.GetProductSalesData();
            _salesForecasting.TrainModel(salesData);
            var forecast = _salesForecasting.Forecast(horizon, salesData);
            return Ok(forecast);
        }

        [HttpGet("forecast-product")]
        public ActionResult<List<float>> ForecastProduct(int productId, int horizon = 10)
        {
            var salesData = _salesDataService.GetProductSalesData(productId: productId);
            _salesForecasting.TrainModel(salesData);
            var forecast = _salesForecasting.Forecast(horizon, salesData);
            return Ok(forecast);
        }

        [HttpGet("forecast-store")]
        public ActionResult<List<float>> ForecastStore(int storeId, int horizon = 10)
        {
            var salesData = _salesDataService.GetProductSalesData(storeId: storeId);
            _salesForecasting.TrainModel(salesData);
            var forecast = _salesForecasting.Forecast(horizon, salesData);
            return Ok(forecast);
        }

        [HttpGet("forecast-store-product")]
        public ActionResult<List<float>> ForecastStoreProduct(int storeId, int productId, int horizon = 10)
        {
            var salesData = _salesDataService.GetProductSalesData(productId: productId, storeId: storeId);
            _salesForecasting.TrainModel(salesData);
            var forecast = _salesForecasting.Forecast(horizon, salesData);
            return Ok(forecast);
        }
    }
}
