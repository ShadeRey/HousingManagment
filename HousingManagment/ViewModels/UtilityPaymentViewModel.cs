using System;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using DynamicData;
using HousingManagment.DataBaseCommands;
using HousingManagment.Models;
using MySqlConnector;
using ReactiveUI;
using SukiUI.Controls;

namespace HousingManagment.ViewModels;

public class UtilityPaymentViewModel: ViewModelBase
{
    private static readonly string ConnectionString = DatabaseManagerConnectionString.ConnectionString;

    private AvaloniaList<UtilityPayment> GetUtilityPaymentsFromDb()
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
    
    private AvaloniaList<UtilityPayment> _utilityPaymentsPreSearch;

    public AvaloniaList<UtilityPayment> UtilityPaymentsPreSearch
    {
        get => _utilityPaymentsPreSearch;
        set => this.RaiseAndSetIfChanged(ref _utilityPaymentsPreSearch, value);
    }

    public void AddUtilityPaymentToDB()
    {
        var db = new DatabaseManagerAdd();

        var add = ReactiveCommand.Create<UtilityPayment>((i) =>
        {
            var newId = db.InsertData(
                "UtilityPayment",
                new MySqlParameter("@Sum", MySqlDbType.Decimal)
                {
                    Value = i.Sum
                },
                new MySqlParameter("@PaymentDate", MySqlDbType.Date)
                {
                    Value = i.PaymentDate
                },
                new MySqlParameter("@PaymentMethod", MySqlDbType.String)
                {
                    Value = i.PaymentMethod
                }
            );
            i.ID = newId;
            OnNew(i);
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = new UtilityPayment(),
            Children =
            {
                new TextBox()
                {
                    Watermark = "Sum($):",
                    [!TextBox.TextProperty] = new Binding("Sum"),
                    Classes = { "Prefix" }
                },
                new DatePicker()
                {
                    [!DatePicker.SelectedDateProperty] = new Binding("PaymentDate")
                },
                new TextBox()
                {
                    Watermark = "PaymentMethod",
                    [!TextBox.TextProperty] = new Binding("PaymentMethod")
                },
                new Button()
                {
                    Content = "Добавить",
                    Classes = { "Primary" },
                    Command = add,
                    Foreground = Brushes.White,
                    [!Button.CommandParameterProperty] = new Binding(".")
                },
                new Button()
                {
                    Content = "Закрыть",
                    Command = ReactiveCommand.Create(InteractiveContainer.CloseDialog)
                }
            }
        });
    }

    public void EditUtilityPaymentInDB()
    {
        var db = new DatabaseManagerEdit();
        int utilityPaymentId = UtilityPaymentSelectedItem.ID;
        var edit = ReactiveCommand.Create<UtilityPayment>((i) =>
        {
            db.EditData(
                "UtilityPayment",
                utilityPaymentId,
                new MySqlParameter("@Sum", MySqlDbType.Decimal)
                {
                    Value = i.Sum
                },
                new MySqlParameter("PaymentDate", MySqlDbType.DateTime)
                {
                    Value = i.PaymentMethod
                },
                new MySqlParameter("PaymentMethod", MySqlDbType.String)
                {
                    Value = i.PaymentMethod
                }
            );
            OnEdit(i);
            InteractiveContainer.CloseDialog();
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = new UtilityPayment()
            {
                ID = UtilityPaymentSelectedItem.ID,
                Sum = UtilityPaymentSelectedItem.Sum,
                PaymentDate = UtilityPaymentSelectedItem.PaymentDate,
                PaymentMethod = UtilityPaymentSelectedItem.PaymentMethod
            },
            Children =
            {
                new TextBox()
                {
                    [!TextBox.TextProperty] = new Binding("Sum")
                },
                new DatePicker()
                {
                    [!DatePicker.SelectedDateProperty] = new Binding("PaymentDate")
                },
                new TextBox()
                {
                    [!TextBox.TextProperty] = new Binding("PaymentMethod")
                },
                new Button()
                {
                    Content = "Обновить",
                    Classes = { "Primary" },
                    Command = edit,
                    Foreground = Brushes.White,
                    [!Button.CommandParameterProperty] = new Binding(".")
                },
                new Button()
                {
                    Content = "Закрыть",
                    Command = ReactiveCommand.Create(InteractiveContainer.CloseDialog)
                }
            }
        });
    }

    public void DeleteUtilityPaymentFromDB()
    {
        if (UtilityPaymentSelectedItem is null)
        {
            return;
        }
        var db = new DatabaseManagerDelete();
        int utilityPaymentId = UtilityPaymentSelectedItem.ID;
        var delete = ReactiveCommand.Create<UtilityPayment>((i) =>
        {
            db.DeleteData(
                "UtilityPayment",
                utilityPaymentId
            );
            OnDelete(i);
            InteractiveContainer.CloseDialog();
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = UtilityPaymentSelectedItem,
            Children =
            {
                new TextBlock()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Classes = { "h2" },
                    Text = "Удалить?"
                },
                new Button()
                {
                    Content = "Да",
                    Classes = { "Primary" },
                    Command = delete,
                    Foreground = Brushes.White,
                    [!Button.CommandParameterProperty] = new Binding(".")
                },
                new Button()
                {
                    Content = "Закрыть",
                    Command = ReactiveCommand.Create(InteractiveContainer.CloseDialog)
                }
            }
        });
    }
}