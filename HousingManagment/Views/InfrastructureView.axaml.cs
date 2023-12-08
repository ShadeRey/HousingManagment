using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using HousingManagment.ViewModels;

namespace HousingManagment.Views;

public partial class InfrastructureView : UserControl
{
    public InfrastructureView()
    {
        InitializeComponent();
    }
    
    public InfrastructureViewModel ViewModel => (DataContext as InfrastructureViewModel)!;

    private async void InfrastructureSearch_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!InfrastructureWP.IsVisible)
        {
            await StartLoadingAsync();
        }

        if (ViewModel.InfrastructuresPreSearch is null)
        {
            ViewModel.InfrastructuresPreSearch = ViewModel.Infrastructure;
        }

        if (InfrastructureSearch.Text is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(InfrastructureSearch.Text))
        {
            InfrastructureGrid.ItemsSource = ViewModel.InfrastructuresPreSearch;
            return;
        }

        Filter();
    }

    private async Task StartLoadingAsync()
    {
        InfrastructureWP.Value = 0;
        InfrastructureGrid.IsVisible = false;
        InfrastructureWP.IsVisible = true;

        Random random = new Random();
        HashSet<int> generatedNumbers = new HashSet<int>();

        int totalUniqueValues = 101;

        for (int nextNumber = 0; nextNumber < 100; nextNumber = random.Next(nextNumber, totalUniqueValues))
        {
            await Task.Delay(1000);
            generatedNumbers.Add(nextNumber);
            InfrastructureWP.Value = nextNumber;
            Console.WriteLine($"Прогресс: {nextNumber}%");
        }


        InfrastructureWP.IsVisible = false;
        InfrastructureGrid.IsVisible = true;
        InfrastructureWP.Value = 0;

        Console.WriteLine("Загрузка завершена.");
    }

    private void Filter()
    {
        if (InfrastructureSearch.Text is null)
        {
            return;
        }
        else
        {
            if (InfrastructureFilter.SelectedIndex == 0)
            {
                var filtered = ViewModel.InfrastructuresPreSearch.Where(
                    it => it.ID.ToString().Contains(InfrastructureSearch.Text)
                          || it.Type.ToLower().Contains(InfrastructureSearch.Text)
                          || it.State.ToLower().Contains(InfrastructureSearch.Text)
                          || it.WorkDescription.ToLower().Contains(InfrastructureSearch.Text)
                ).ToList();
                filtered = filtered.OrderBy(id => id.ID).ToList();
                InfrastructureGrid.ItemsSource = filtered;
            }
            else if (InfrastructureFilter.SelectedIndex == 1)
            {
                var filtered = ViewModel.InfrastructuresPreSearch
                    .Where(it => it.ID.ToString().Contains(InfrastructureSearch.Text)).ToList();
                filtered = filtered.OrderBy(id => id.ID).ToList();
                InfrastructureGrid.ItemsSource = filtered;
            }
            else if (InfrastructureFilter.SelectedIndex == 2)
            {
                var filtered = ViewModel.InfrastructuresPreSearch
                    .Where(it => it.Type.ToLower().Contains(InfrastructureSearch.Text)).ToList();
                filtered = filtered.OrderBy(type => type.Type).ToList();
                InfrastructureGrid.ItemsSource = filtered;
            }
            else if (InfrastructureFilter.SelectedIndex == 3)
            {
                var filtered = ViewModel.InfrastructuresPreSearch
                    .Where(it => it.State.ToLower().Contains(InfrastructureSearch.Text)).ToList();
                filtered = filtered.OrderBy(state => state.State).ToList();
                InfrastructureGrid.ItemsSource = filtered;
            }
            else if (InfrastructureFilter.SelectedIndex == 4)
            {
                var filtered = ViewModel.InfrastructuresPreSearch
                    .Where(it => it.WorkDescription.ToLower().Contains(InfrastructureSearch.Text)).ToList();
                filtered = filtered.OrderBy(workdescription => workdescription.WorkDescription).ToList();
                InfrastructureGrid.ItemsSource = filtered;
            }
        }
    }

    private void InfrastructureFilter_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) => Filter();
}