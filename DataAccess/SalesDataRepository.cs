using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

public class SalesData
{
    public int ID_INVOICE { get; set; }
    public DateTime ISSUE_DATE { get; set; }
    public int ORDER_QUANTITY { get; set; }
    public int ID_PRODUCT { get; set; }
    public int ID_STORE { get; set; }
}

public class SalesDataRepository
{
    private readonly string _connectionString;

    
public SalesDataRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<SalesData> GetSalesData()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var sql = @"
            SELECT 
                i.ID_INVOICE,
                i.ISSUE_DATE,
                ip.ORDER_QUANTITY,
                ip.ID_PRODUCT,
                i.ID_STORE
            FROM 
                INVOICE i
            JOIN 
                INVOICE_PRODUCT ip ON i.ID_INVOICE = ip.ID_INVOICE";

            var data = connection.Query<SalesData>(sql).ToList();
            return data;
        }
    }
}