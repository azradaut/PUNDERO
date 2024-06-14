public class StoreViewModel
{
    public int IdStore { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string? ClientName { get; set; }
    public string? Qr { get; set; } = null!;
}
