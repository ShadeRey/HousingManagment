using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using HousingManagment.DataBaseCommands;
using HousingManagment.Models;
using HousingManagment.ViewModels;
using MySqlConnector;
using ReactiveUI;
using SukiUI.Controls;

namespace HousingManagment.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }

    private void HousingTypeAdd_OnClick(object? sender, RoutedEventArgs e) {
        var db = new DatabaseManagerAdd();

        var add = ReactiveCommand.Create<HousingType>((i) => {
            var newId = db.InsertData(
                "HousingType",
                new MySqlParameter("@Name", MySqlDbType.String) {
                    Value = i.Name
                }
                );
            i.ID = newId;
            (DataContext as MainWindowViewModel)!.HousingTypeViewModel.OnNew(i);
        });
        
        InteractiveContainer.ShowDialog(new StackPanel() {
            DataContext = new HousingType(),
            Children = {
                new TextBox() {
                    [!TextBox.TextProperty] = new Binding("Name")
                },
                new Button() {
                    Content = "Добавить",
                    Classes = { "Primary" },
                    Command = add,
                    [!Button.CommandParameterProperty] = new Binding(".")
                },
                new Button() {
                    Content = "Закрыть",
                    Command = ReactiveCommand.Create(InteractiveContainer.CloseDialog)
                }
            }
        });
    }
}