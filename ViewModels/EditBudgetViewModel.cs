using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoMoney.Models;
using MoMoney.Services;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MoMoney.ViewModels
{
    /// <summary>
    /// ViewModel for editing an existing budget, including updating, deleting, and navigation.
    /// </summary>
    public partial class EditBudgetViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// Gets or sets the identifier of the budget being edited.
        /// </summary>
        [ObservableProperty] private int id;

        /// <summary>
        /// Gets or sets the category field value in the edit form.
        /// </summary>
        [ObservableProperty] private string formCategory;

        /// <summary>
        /// Gets or sets the amount field value in the edit form.
        /// </summary>
        [ObservableProperty] private string formAmount;

        /// <summary>
        /// Gets or sets the period field value in the edit form.
        /// </summary>
        [ObservableProperty] private string formPeriod;

        /// <summary>
        /// Gets or sets the total spent amount associated with this budget.
        /// </summary>
        [ObservableProperty] private decimal spent;

        /// <summary>
        /// Gets the collection of available budget categories.
        /// </summary>
        public ObservableCollection<string> Categories { get; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="EditBudgetViewModel"/> class.
        /// </summary>
        /// <param name="dataService">The data service used to access and update budgets.</param>
        public EditBudgetViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// Loads a budget asynchronously for editing by its identifier.
        /// </summary>
        /// <param name="budgetId">The identifier of the budget to load.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task LoadBudgetAsync(int budgetId)
        {
            try
            {
                LoggerService.LogInfo<EditBudgetViewModel>($"Loading budget Id={budgetId}...");

                var budget = (await _dataService.GetBudgetsAsync())
                             .FirstOrDefault(b => b.Id == budgetId);

                if (budget != null)
                {
                    Id = budget.Id;
                    FormAmount = budget.Amount.ToString("C");
                    FormPeriod = budget.Period;
                    Spent = 0; // TODO: calculate from transactions

                    await LoadCategoriesAsync();

                    // Ensure FormCategory matches something in Categories
                    FormCategory = Categories.FirstOrDefault(c =>
                        c.Equals(budget.Category, StringComparison.OrdinalIgnoreCase));

                    LoggerService.LogInfo<EditBudgetViewModel>(
                         $"Loaded budget Id={Id}, Category='{FormCategory}', Amount={FormAmount}, Period={FormPeriod}");
                }
                else
                {
                    LoggerService.LogWarning<EditBudgetViewModel>(
                        $"No budget found for Id={budgetId}.");
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogError<EditBudgetViewModel>(ex, $"Failed to load budget Id={budgetId}.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to load budget.", "OK");
            }
        }

        /// <summary>
        /// Saves the currently edited budget asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task SaveBudgetAsync()
        {
            try
            {
                LoggerService.LogInfo<EditBudgetViewModel>($"Saving budget Id={Id}...");

                if (!decimal.TryParse(FormAmount, NumberStyles.Currency, CultureInfo.CurrentCulture, out var parsedAmount))
                {
                    LoggerService.LogWarning<EditBudgetViewModel>("Invalid amount entered in SaveBudgetAsync.");
                    await Application.Current.MainPage.DisplayAlert("Invalid Amount", "Please enter a valid amount.", "OK");
                    return;
                }

                var updated = new Budget
                {
                    Id = Id,
                    Category = FormCategory,
                    Amount = parsedAmount,
                    Period = FormPeriod
                };

                await _dataService.UpdateBudgetAsync(updated);

                LoggerService.LogInfo<EditBudgetViewModel>(
                    $"Updated budget Id={Id}, Category='{FormCategory}', Amount={parsedAmount:C}, Period={FormPeriod}");

                await Shell.Current.GoToAsync(".."); // navigate back
            }
            catch (Exception ex)
            {
                LoggerService.LogError<EditBudgetViewModel>(ex, $"Failed to save budget Id={Id}.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to save budget.", "OK");
            }
        }

        /// <summary>
        /// Deletes the currently loaded budget asynchronously after user confirmation.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task DeleteBudgetAsync()
        {
            try
            {
                LoggerService.LogInfo<EditBudgetViewModel>($"Attempting to delete budget Id={Id}...");

                bool confirm = await Application.Current.MainPage.DisplayAlert(
                    "Delete Budget",
                    "Are you sure you want to delete this budget?",
                    "Yes", "No");

                if (!confirm)
                {
                    LoggerService.LogInfo<EditBudgetViewModel>($"Delete cancelled for budget Id={Id}.");
                    return;
                }

                await _dataService.DeleteBudgetAsync(new Budget { Id = Id });

                LoggerService.LogInfo<EditBudgetViewModel>($"Deleted budget Id={Id}.");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<EditBudgetViewModel>(ex, $"Failed to delete budget Id={Id}.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete budget.", "OK");
            }
        }

        /// <summary>
        /// Cancels the current edit operation and navigates back asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task CancelAsync()
        {
            {
                try
                {
                    LoggerService.LogInfo<EditBudgetViewModel>($"Cancel triggered for budget Id={Id}, navigating back.");
                    await Shell.Current.GoToAsync("..");
                }
                catch (Exception ex)
                {
                    LoggerService.LogError<EditBudgetViewModel>(ex, $"Failed to navigate back in CancelAsync for budget Id={Id}.");
                }
            }
        }

        /// <summary>
        /// Loads available categories asynchronously and ensures the form category is valid.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task LoadCategoriesAsync()
        {
            try
            {
                LoggerService.LogInfo<EditBudgetViewModel>("Loading categories...");

                await _dataService.SeedDefaultCategoriesAsync();

                Categories.Clear();
                var items = await _dataService.GetCategoriesAsync();
                foreach (var c in items)
                    Categories.Add(c.Name);

                // Ensure current budget’s category is in the list
                if (!string.IsNullOrWhiteSpace(FormCategory) &&
                    !Categories.Contains(FormCategory))
                {
                    Categories.Add(FormCategory);
                }

                LoggerService.LogInfo<EditBudgetViewModel>($"Loaded {Categories.Count} categories.");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<EditBudgetViewModel>(ex, "Failed to load categories.");
            }
        }
    }
}
