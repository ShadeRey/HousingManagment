using System;

namespace HousingManagment.Models;

public class MaintenanceWork
{
    public int ID { get; set; }
    public string Description { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public decimal Price { get; set; }
}