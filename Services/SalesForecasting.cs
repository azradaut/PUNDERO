using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

namespace PUNDERO.Services
{
    public class SalesForecasting
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;

        public SalesForecasting()
        {
            _mlContext = new MLContext();
        }

        public void TrainModel(List<ProductSalesData> salesData)
        {
            if (salesData.Count <= 3)
            {
                throw new ArgumentException("The series length should be greater than the window size (5).");
            }

            var data = _mlContext.Data.LoadFromEnumerable(salesData);

            var windowSize = Math.Min(3, salesData.Count - 1);
            var horizon = Math.Min(10, salesData.Count - windowSize);

            var pipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(SalesForecastingPrediction.ForecastedOrderQuantity),
                inputColumnName: nameof(ProductSalesData.OrderQuantity),
                windowSize: windowSize,
                seriesLength: salesData.Count,
                trainSize: salesData.Count,
                horizon: horizon);

            _model = pipeline.Fit(data);
        }

        public List<float> Forecast(int horizon, List<ProductSalesData> salesData)
        {
            var forecastEngine = _model.CreateTimeSeriesEngine<ProductSalesData, SalesForecastingPrediction>(_mlContext);
            var prediction = forecastEngine.Predict();
            return prediction.ForecastedOrderQuantity.Take(horizon).ToList();
        }
    }

    public class SalesForecastingPrediction
    {
        public float[] ForecastedOrderQuantity { get; set; }
    }
}

