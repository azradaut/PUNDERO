namespace PUNDERO.Services
{
    public class SalesDataService
    {
        private readonly SalesDataRepository _salesDataRepository;

        public SalesDataService(SalesDataRepository salesDataRepository)
        {
            _salesDataRepository = salesDataRepository;
        }

        public List<ProductSalesData> GetProductSalesData(int? productId = null, int? storeId = null)
        {
            var salesData = _salesDataRepository.GetSalesData(productId, storeId);

            return salesData
                .GroupBy(sd => new { sd.ISSUE_DATE.Date, sd.ID_PRODUCT, sd.ID_STORE })
                .Select(g => new ProductSalesData
                {
                    IssueDate = g.Key.Date,
                    OrderQuantity = g.Sum(sd => sd.ORDER_QUANTITY)
                }).ToList();
        }
    }
}
