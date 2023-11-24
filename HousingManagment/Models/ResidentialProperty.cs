namespace HousingManagment.Models;

public class ResidentialProperty
{
    public int ID { get; set; }
    public string Address { get; set; }
    public decimal Square { get; set; }
    public int NumberOfRooms { get; set; }
    public int HousingType { get; set; }
    public int Payment { get; set; }
    public int Work { get; set; }
}