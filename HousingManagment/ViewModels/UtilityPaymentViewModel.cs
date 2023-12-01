using System;
using Avalonia.Collections;
using HousingManagment.Models;
using MySqlConnector;
using ReactiveUI;

namespace HousingManagment.ViewModels;

public class UtilityPaymentViewModel: ViewModelBase
{
    // private const string _connectionString = "server=10.10.1.24;user=user_01;password=user01pro;database=pro1_23;";
    private const string _connectionString = "Server=localhost;Database=UP;User Id=root;Password=sharaga228;";

    public AvaloniaList<UtilityPayment> GetUtilityPaymentsFromDb()
    {
        AvaloniaList<UtilityPayment> utilityPayments = new AvaloniaList<UtilityPayment>();

        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            try
            {
                connection.Open();
                string selectAllUtilityPayments = "SELECT * FROM UtilityPayment";
                MySqlCommand cmd = new MySqlCommand(selectAllUtilityPayments, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    UtilityPayment utilityPaymentsItem = new UtilityPayment();
                    if (!reader.IsDBNull(reader.GetOrdinal("ID")))
                    {
                        utilityPaymentsItem.ID = reader.GetInt32("ID");
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("Sum")))
                    {
                        utilityPaymentsItem.Sum = reader.GetDecimal("Sum");
                    }
                    
                    if (!reader.IsDBNull(reader.GetOrdinal("PaymentDate")))
                    {
                        utilityPaymentsItem.PaymentDate = reader.GetDateTimeOffset("PaymentDate");
                    }
                    
                    if (!reader.IsDBNull(reader.GetOrdinal("PaymentMethod")))
                    {
                        utilityPaymentsItem.PaymentMethod = reader.GetString("PaymentMethod");
                    }

                    utilityPayments.Add(utilityPaymentsItem);
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Ошибка подключения к БД: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        return utilityPayments;
    }
    
    private AvaloniaList<UtilityPayment> _utilityPayment;

    public AvaloniaList<UtilityPayment> UtilityPayment
    {
        get => _utilityPayment;
        set => this.RaiseAndSetIfChanged(ref _utilityPayment, value);
    }

    public UtilityPaymentViewModel()
    {
        UtilityPayment = GetUtilityPaymentsFromDb();
    }
}