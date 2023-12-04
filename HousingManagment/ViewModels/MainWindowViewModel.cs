namespace HousingManagment.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MaintenanceWorkViewModel MaintenanceWorkViewModel { get; set; } = new();
    public HousingTypeViewModel HousingTypeViewModel { get; set; } = new();
    public UtilityPaymentViewModel UtilityPaymentViewModel { get; set; } = new();
    public InfrastructureViewModel InfrastructureViewModel { get; set; } = new();
    public ResidentialPropertyViewModel ResidentialPropertyViewModel { get; set; } = new();
}