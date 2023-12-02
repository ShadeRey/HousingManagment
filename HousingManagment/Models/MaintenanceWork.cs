using System;

namespace HousingManagment.Models;

public class MaintenanceWork
{
    public int ID { get; set; }
    public string Description { get; set; }
    public DateTimeOffset StartDate { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset EndDate { get; set; } = DateTimeOffset.Now;
    public decimal Price { get; set; }
}