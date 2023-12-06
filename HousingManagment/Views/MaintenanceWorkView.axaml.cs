using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

public partial class MaintenanceWorkView : UserControl
{
    public MaintenanceWorkView()
    {
        InitializeComponent();
    }
    
    public MaintenanceWorkViewModel ViewModel => (DataContext as MaintenanceWorkViewModel)!;
    
    private void MaintenanceWorkAdd_OnClick(object? sender, RoutedEventArgs e)
    {
        var db = new DatabaseManagerAdd();

        var add = ReactiveCommand.Create<MaintenanceWork>((i) =>
        {
            var newId = db.InsertData(
                "MaintenanceWork",
                new MySqlParameter("@Description", MySqlDbType.String)
                {
                    Value = i.Description
                },
                new MySqlParameter("@StartDate", MySqlDbType.Date)
                {
                    Value = i.StartDate
                },
                new MySqlParameter("@EndDate", MySqlDbType.Date)
                {
                    Value = i.EndDate
                },
                new MySqlParameter("@Price", MySqlDbType.Decimal)
                {
                    Value = i.Price
                }
            );
            i.ID = newId;
            ViewModel.OnNew(i);
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = new MaintenanceWork(),
            Children =
            {
                new TextBox()
                {
                    Watermark = "Description",
                    [!TextBox.TextProperty] = new Binding("Description")
                },
                new DatePicker()
                {
                    [!DatePicker.SelectedDateProperty] = new Binding("StartDate"),
                },
                new DatePicker()
                {
                    [!DatePicker.SelectedDateProperty] = new Binding("EndDate"),
                },
                new TextBox()
                {
                    Watermark = "Price($):",
                    [!TextBox.TextProperty] = new Binding("Price"),
                    Classes = { "Prefix" }
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
    
    private void MaintenanceWorkEdit_OnClick(object? sender, RoutedEventArgs e)
    {
        var db = new DatabaseManagerEdit();
        int maintenanceWorkId = ViewModel.MaintenanceWorkSelectedItem.ID;
        var edit = ReactiveCommand.Create<MaintenanceWork>((i) =>
        {
            db.EditData(
                "MaintenanceWork",
                maintenanceWorkId,
                new MySqlParameter("@Description", MySqlDbType.String)
                {
                    Value = i.Description
                },
                new MySqlParameter("StartDate", MySqlDbType.DateTime)
                {
                    Value = i.StartDate
                },
                new MySqlParameter("EndDate", MySqlDbType.DateTime)
                {
                    Value = i.EndDate
                },
                new MySqlParameter("Price", MySqlDbType.Decimal)
                {
                    Value = i.Price
                }
            );
            ViewModel.OnEdit(i);
            InteractiveContainer.CloseDialog();
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = new MaintenanceWork()
            {
                ID = ViewModel.MaintenanceWorkSelectedItem.ID,
                Description = ViewModel.MaintenanceWorkSelectedItem.Description,
                StartDate = ViewModel.MaintenanceWorkSelectedItem.StartDate,
                EndDate = ViewModel.MaintenanceWorkSelectedItem.EndDate,
                Price = ViewModel.MaintenanceWorkSelectedItem.Price
            },
            Children =
            {
                new TextBox()
                {
                    [!TextBox.TextProperty] = new Binding("Description")
                },
                new DatePicker()
                {
                    [!DatePicker.SelectedDateProperty] = new Binding("StartDate")
                },
                new DatePicker()
                {
                    [!DatePicker.SelectedDateProperty] = new Binding("EndDate")
                },
                new TextBox()
                {
                    [!TextBox.TextProperty] = new Binding("Price")
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

    private void MaintenanceWorkDelete_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.MaintenanceWorkSelectedItem is null)
        {
            return;
        }
        var db = new DatabaseManagerDelete();
        int maintenanceWorkId = ViewModel.MaintenanceWorkSelectedItem.ID;
        var delete = ReactiveCommand.Create<MaintenanceWork>((i) =>
        {
            db.DeleteData(
                "MaintenanceWork",
                maintenanceWorkId
            );
            ViewModel.OnDelete(i);
            InteractiveContainer.CloseDialog();
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = ViewModel.MaintenanceWorkSelectedItem,
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

    private async void MaintenanceWorkSearch_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!MaintenanceWorkWP.IsVisible)
        {
            await StartLoadingAsync();
        }
        if (ViewModel.MaintenanceWorksPreSearch is null)
        {
            ViewModel.MaintenanceWorksPreSearch = ViewModel.MaintenanceWork;
        }

        if (MaintenanceWorkSearch.Text is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(MaintenanceWorkSearch.Text))
        {
            MaintenanceWorkGrid.ItemsSource = ViewModel.MaintenanceWorksPreSearch;
            return;
        }

        Filter();
    }

    private void Filter()
    {
        if (MaintenanceWorkSearch.Text is null)
        {
            return;
        }
        else
        {
            if (MaintenanceWorkFilter.SelectedIndex == 0)
            {
                var filtered = ViewModel.MaintenanceWorksPreSearch.Where(
                    it => it.ID.ToString().Contains(MaintenanceWorkSearch.Text)
                          || it.Description.ToLower().Contains(MaintenanceWorkSearch.Text)
                          || it.StartDate.ToString("dd.MM.yyyy").Contains(MaintenanceWorkSearch.Text)
                          || it.EndDate.ToString("dd.MM.yyyy").Contains(MaintenanceWorkSearch.Text)
                          || it.Price.ToString().Contains(MaintenanceWorkSearch.Text)
                ).ToList();
                filtered = filtered.OrderBy(id => id.ID).ToList();
                MaintenanceWorkGrid.ItemsSource = filtered;
            }
            else if (MaintenanceWorkFilter.SelectedIndex == 1)
            {
                var filtered = ViewModel.MaintenanceWorksPreSearch
                    .Where(it => it.ID.ToString().Contains(MaintenanceWorkSearch.Text)).ToList();
                filtered = filtered.OrderBy(id => id.ID).ToList();
                MaintenanceWorkGrid.ItemsSource = filtered;
            }
            else if (MaintenanceWorkFilter.SelectedIndex == 2)
            {
                var filtered = ViewModel.MaintenanceWorksPreSearch
                    .Where(it => it.Description.ToLower().Contains(MaintenanceWorkSearch.Text)).ToList();
                filtered = filtered.OrderBy(description => description.Description).ToList();
                MaintenanceWorkGrid.ItemsSource = filtered;
            }
            else if (MaintenanceWorkFilter.SelectedIndex == 3)
            {
                var filtered = ViewModel.MaintenanceWorksPreSearch
                    .Where(it => it.StartDate.ToString("dd.MM.yyyy").Contains(MaintenanceWorkSearch.Text)).ToList();
                filtered = filtered.OrderBy(startdate => startdate.StartDate).ToList();
                MaintenanceWorkGrid.ItemsSource = filtered;
            }
            else if (MaintenanceWorkFilter.SelectedIndex == 4)
            {
                var filtered = ViewModel.MaintenanceWorksPreSearch
                    .Where(it => it.EndDate.ToString("dd.MM.yyyy").Contains(MaintenanceWorkSearch.Text)).ToList();
                filtered = filtered.OrderBy(enddate => enddate.EndDate).ToList();
                MaintenanceWorkGrid.ItemsSource = filtered;
            }
            else if (MaintenanceWorkFilter.SelectedIndex == 5)
            {
                var filtered = ViewModel.MaintenanceWorksPreSearch
                    .Where(it => it.Price.ToString().Contains(MaintenanceWorkSearch.Text)).ToList();
                filtered = filtered.OrderBy(price => price.Price).ToList();
                MaintenanceWorkGrid.ItemsSource = filtered;
            }
        }
    }
    
    private async Task StartLoadingAsync()
    {
        MaintenanceWorkWP.Value = 0;
        MaintenanceWorkGrid.IsVisible = false;
        MaintenanceWorkWP.IsVisible = true;

        Random random = new Random();
        HashSet<int> generatedNumbers = new HashSet<int>();

        int totalUniqueValues = 101;

        for (int nextNumber = 0; nextNumber < 100; nextNumber = random.Next(nextNumber, totalUniqueValues))
        {
            await Task.Delay(1000);
            generatedNumbers.Add(nextNumber);
            MaintenanceWorkWP.Value = nextNumber;
            Console.WriteLine($"Прогресс: {nextNumber}%");
        }


        MaintenanceWorkWP.IsVisible = false;
        MaintenanceWorkGrid.IsVisible = true;
        MaintenanceWorkWP.Value = 0;

        Console.WriteLine("Загрузка завершена.");
    }

    private void MaintenanceWorkFilter_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) => Filter();
}