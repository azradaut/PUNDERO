using PUNDERO.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace PUNDERO.Services
{
    public class SalesDataService
    {
        private readonly SalesDataRepository _salesDataRepository;

        public SalesDataService(SalesDataRepository salesDataRepository)
        {
            _salesDataRepository = salesDataRepository;
        }

        public List<ProductSalesData> GetProductSalesData()
        {
            var salesData = _salesDataRepository.GetSalesData();

            return salesData
                .GroupBy(sd => sd.ISSUE_DATE.Date)
                .Select(g => new ProductSalesData
                {
                    IssueDate = g.Key,
                    OrderQuantity = g.Sum(sd => sd.ORDER_QUANTITY)
                }).ToList();
        }
    }
}
