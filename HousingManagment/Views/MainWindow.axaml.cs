using Avalonia.Controls;
using HousingManagment.ViewModels;

namespace HousingManagment.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new HousingTypeViewModel();
    }
}