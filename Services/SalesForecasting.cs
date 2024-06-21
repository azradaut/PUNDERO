using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using PUNDERO.DataAccess;
using System.Collections.Generic;

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

            // Adjusting windowSize and horizon based on available data size
            var windowSize = Math.Min(3, salesData.Count - 1);  // Adjust windowSize to fit available data
            var horizon = Math.Min(6, salesData.Count - windowSize);  // Adjust horizon based on windowSize and available data

            var pipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(SalesForecastingPrediction.ForecastedOrderQuantity),
                inputColumnName: nameof(ProductSalesData.OrderQuantity),
                windowSize: windowSize,
                seriesLength: salesData.Count,
                trainSize: salesData.Count,
                horizon: horizon);

            _model = pipeline.Fit(data);
        }



        public List<float> Forecast(int horizon)
        {
            var forecastEngine = _model.CreateTimeSeriesEngine<ProductSalesData, SalesForecastingPrediction>(_mlContext);
            var prediction = forecastEngine.Predict();
            return prediction.ForecastedOrderQuantity.ToList();
        }
    }

    public class SalesForecastingPrediction
    {
        public float[] ForecastedOrderQuantity { get; set; }
    }
}
