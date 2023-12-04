using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using HousingManagment.DataBaseCommands;
using HousingManagment.Models;
using HousingManagment.ViewModels;
using MySqlConnector;
using ReactiveUI;
using SukiUI.Controls;

namespace HousingManagment.Views;

public partial class UtilityPaymentView : UserControl
{
    public UtilityPaymentView()
    {
        InitializeComponent();
    }
    
    public UtilityPaymentViewModel ViewModel => (DataContext as UtilityPaymentViewModel)!;
    
    private void UtilityPaymentAdd_OnClick(object? sender, RoutedEventArgs e)
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
            ViewModel.OnNew(i);
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
    
    private void UtilityPaymentEdit_OnClick(object? sender, RoutedEventArgs e)
    {
        var db = new DatabaseManagerEdit();
        int utilityPaymentId = ViewModel.UtilityPaymentSelectedItem.ID;
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
            ViewModel.OnEdit(i);
            InteractiveContainer.CloseDialog();
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = new UtilityPayment()
            {
                ID = ViewModel.UtilityPaymentSelectedItem.ID,
                Sum = ViewModel.UtilityPaymentSelectedItem.Sum,
                PaymentDate = ViewModel.UtilityPaymentSelectedItem.PaymentDate,
                PaymentMethod = ViewModel.UtilityPaymentSelectedItem.PaymentMethod
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

    private void UtilityPaymentDelete_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.UtilityPaymentSelectedItem is null)
        {
            return;
        }
        var db = new DatabaseManagerDelete();
        int utilityPaymentId = ViewModel.UtilityPaymentSelectedItem.ID;
        var delete = ReactiveCommand.Create<UtilityPayment>((i) =>
        {
            db.DeleteData(
                "UtilityPayment",
                utilityPaymentId
            );
            ViewModel.OnDelete(i);
            InteractiveContainer.CloseDialog();
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = ViewModel.UtilityPaymentSelectedItem,
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