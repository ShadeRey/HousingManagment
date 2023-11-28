using Avalonia.Collections;
using Avalonia.Controls;
using HousingManagment.ViewModels;

namespace HousingManagment.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        HousingTypeTabItem.DataContext = new HousingTypeViewModel();
        MaintenanceWorkTabItem.DataContext = new MaintenanceWorkViewModel();
    }
}