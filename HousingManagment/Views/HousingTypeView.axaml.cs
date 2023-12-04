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

public partial class HousingTypeView : UserControl
{
    public HousingTypeView()
    {
        InitializeComponent();
    }
    
    public HousingTypeViewModel ViewModel => (DataContext as HousingTypeViewModel)!;
    
    private void HousingTypeAdd_OnClick(object? sender, RoutedEventArgs e)
    {
        var db = new DatabaseManagerAdd();

        var add = ReactiveCommand.Create<HousingType>((i) =>
        {
            var newId = db.InsertData(
                "HousingType",
                new MySqlParameter("@Name", MySqlDbType.String)
                {
                    Value = i.Name
                }
            );
            i.ID = newId;
            ViewModel.OnNew(i);
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = new HousingType(),
            Children =
            {
                new TextBox()
                {
                    Watermark = "Name",
                    [!TextBox.TextProperty] = new Binding("Name")
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
    
    private void HousingTypeEdit_OnClick(object? sender, RoutedEventArgs e) {
        var db = new DatabaseManagerEdit();
        int housingTypeId = ViewModel.HousingTypeSelectedItem.ID;
        var edit = ReactiveCommand.Create<HousingType>((i) =>
        {
            db.EditData(
                "HousingType",
                housingTypeId,
                new MySqlParameter("@Name", MySqlDbType.String)
                {
                    Value = i.Name
                }
            );
            ViewModel.OnEdit(i);
            InteractiveContainer.CloseDialog();
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = new HousingType()
            {
                ID = ViewModel.HousingTypeSelectedItem.ID, 
                Name = ViewModel.HousingTypeSelectedItem.Name
            },
            Children =
            {
                new TextBox()
                {
                    [!TextBox.TextProperty] = new Binding("Name")
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
    
    private void HousingTypeDelete_OnClick(object? sender, RoutedEventArgs e) {
        if (ViewModel.HousingTypeSelectedItem is null)
        {
            return;
        }
        var db = new DatabaseManagerDelete();
        int housingTypeId = ViewModel.HousingTypeSelectedItem.ID;
        var delete = ReactiveCommand.Create<HousingType>((i) =>
        {
            db.DeleteData(
                "HousingType",
                housingTypeId
            );
            ViewModel.OnDelete(i);
            InteractiveContainer.CloseDialog();
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = ViewModel.HousingTypeSelectedItem,
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