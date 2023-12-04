using System;
using Avalonia.Collections;
using DynamicData;
using HousingManagment.DataBaseCommands;
using HousingManagment.Models;
using MySqlConnector;
using ReactiveUI;

namespace HousingManagment.ViewModels;

public class UtilityPaymentViewModel: ViewModelBase
{
    public static readonly string ConnectionString = DatabaseManagerConnectionString.ConnectionString;

    public AvaloniaList<UtilityPayment> GetUtilityPaymentsFromDb()
    {
        AvaloniaList<UtilityPayment> utilityPayments = new AvaloniaList<UtilityPayment>();

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
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
    
    public void OnNew(UtilityPayment utilityPayment) {
        UtilityPayment.Add(utilityPayment);
    }
    
    private UtilityPayment _utilityPaymentSelectedItem;

    public UtilityPayment UtilityPaymentSelectedItem {
        get => _utilityPaymentSelectedItem;
        set => this.RaiseAndSetIfChanged(ref _utilityPaymentSelectedItem, value);
    }
    
    public void OnDelete(UtilityPayment utilityPayment) {
        UtilityPayment.Remove(utilityPayment);
    }
    
    public void OnEdit(UtilityPayment utilityPayment) {
        UtilityPayment.Replace(UtilityPaymentSelectedItem, utilityPayment);
    }
}