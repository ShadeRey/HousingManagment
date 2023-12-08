using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using HousingManagment.ViewModels;

namespace HousingManagment.Views;

public partial class UtilityPaymentView : UserControl
{
    public UtilityPaymentView()
    {
        InitializeComponent();
    }
    
    public UtilityPaymentViewModel ViewModel => (DataContext as UtilityPaymentViewModel)!;

    private async void UtilityPaymentSearch_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!UtilityPaymentWP.IsVisible)
        {
            await StartLoadingAsync();
        }

        if (ViewModel.UtilityPaymentsPreSearch is null)
        {
            ViewModel.UtilityPaymentsPreSearch = ViewModel.UtilityPayment;
        }

        if (UtilityPaymentSearch.Text is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(UtilityPaymentSearch.Text))
        {
            UtilityPaymentGrid.ItemsSource = ViewModel.UtilityPaymentsPreSearch;
            return;
        }

        Filter();
    }

    private async Task StartLoadingAsync()
    {
            UtilityPaymentWP.Value = 0;
            UtilityPaymentGrid.IsVisible = false;
            UtilityPaymentWP.IsVisible = true;

            Random random = new Random();
            HashSet<int> generatedNumbers = new HashSet<int>();

            int totalUniqueValues = 101;

            for (int nextNumber = 0; nextNumber < 100; nextNumber = random.Next(nextNumber, totalUniqueValues))
            {
                await Task.Delay(1000);
                generatedNumbers.Add(nextNumber);
                UtilityPaymentWP.Value = nextNumber;
                Console.WriteLine($"Прогресс: {nextNumber}%");
            }
    }

    private void Filter()
    {
        if (UtilityPaymentSearch.Text is null)
        {
            return;
        }
        else
        {
            if (UtilityPaymentFilter.SelectedIndex == 0)
            {
                var filtered = ViewModel.UtilityPaymentsPreSearch.Where(
                    it => it.ID.ToString().Contains(UtilityPaymentSearch.Text)
                          || it.Sum.ToString().Contains(UtilityPaymentSearch.Text)
                          || it.PaymentDate.ToString("dd.MM.yyyy").Contains(UtilityPaymentSearch.Text)
                          || it.PaymentMethod.ToLower().Contains(UtilityPaymentSearch.Text)
                ).ToList();
                filtered = filtered.OrderBy(id => id.ID).ToList();
                UtilityPaymentGrid.ItemsSource = filtered;
            }
            else if (UtilityPaymentFilter.SelectedIndex == 1)
            {
                var filtered = ViewModel.UtilityPaymentsPreSearch
                    .Where(it => it.ID.ToString().Contains(UtilityPaymentSearch.Text)).ToList();
                filtered = filtered.OrderBy(id => id.ID).ToList();
                UtilityPaymentGrid.ItemsSource = filtered;
            }
            else if (UtilityPaymentFilter.SelectedIndex == 2)
            {
                var filtered = ViewModel.UtilityPaymentsPreSearch
                    .Where(it => it.Sum.ToString().Contains(UtilityPaymentSearch.Text)).ToList();
                filtered = filtered.OrderBy(sum => sum.Sum).ToList();
                UtilityPaymentGrid.ItemsSource = filtered;
            }
            else if (UtilityPaymentFilter.SelectedIndex == 3)
            {
                var filtered = ViewModel.UtilityPaymentsPreSearch
                    .Where(it => it.PaymentDate.ToString("dd.MM.yyyy").Contains(UtilityPaymentSearch.Text)).ToList();
                filtered = filtered.OrderBy(paymentdate => paymentdate.PaymentDate).ToList();
                UtilityPaymentGrid.ItemsSource = filtered;
            }
            else if (UtilityPaymentFilter.SelectedIndex == 4)
            {
                var filtered = ViewModel.UtilityPaymentsPreSearch
                    .Where(it => it.PaymentMethod.ToLower().Contains(UtilityPaymentSearch.Text)).ToList();
                filtered = filtered.OrderBy(paymentmethod => paymentmethod.PaymentMethod).ToList();
                UtilityPaymentGrid.ItemsSource = filtered;
            }
        }
    }

    private void UtilityPaymentFilter_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) => Filter();
}