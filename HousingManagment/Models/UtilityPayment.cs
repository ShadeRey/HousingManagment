using System;

namespace HousingManagment.Models;

public class UtilityPayment
{
    public int ID { get; set; }
    public decimal Sum { get; set; }
    public DateTimeOffset PaymentDate { get; set; }
    public string PaymentMethod { get; set; }
}