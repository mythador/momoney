using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoMoney.Services;

namespace MoMoney.ViewModels
{
    /// <summary>
    /// ViewModel for the main application navigation, including navigation to
    /// transactions, budgets, and reports pages.
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        /// <summary>
        /// Navigates asynchronously to the Transactions page.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task GoToTransactions()
        {
            try
            {
                LoggerService.LogInfo<MainViewModel>("Navigating to TransactionsPage...");
                await Shell.Current.GoToAsync("//TransactionsPage");
                LoggerService.LogInfo<MainViewModel>("Navigation to TransactionsPage successful.");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<MainViewModel>(ex, "Failed to navigate to TransactionsPage.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to navigate to Transactions.", "OK");
            }
        }

        /// <summary>
        /// Navigates asynchronously to the Budgets page.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task GoToBudgets()
        {
            try
            {
                LoggerService.LogInfo<MainViewModel>("Navigating to BudgetsPage...");
                await Shell.Current.GoToAsync("//BudgetsPage");
                LoggerService.LogInfo<MainViewModel>("Navigation to BudgetsPage successful.");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<MainViewModel>(ex, "Failed to navigate to BudgetsPage.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to navigate to Budgets.", "OK");
            }
        }

        /// <summary>
        /// Navigates asynchronously to the Reports page.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task GoToReports()
        {
            try
            {
                LoggerService.LogInfo<MainViewModel>("Navigating to ReportsPage...");
                await Shell.Current.GoToAsync("//ReportsPage");
                LoggerService.LogInfo<MainViewModel>("Navigation to ReportsPage successful.");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<MainViewModel>(ex, "Failed to navigate to ReportsPage.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to navigate to Reports.", "OK");
            }
        }
    }
}
