using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoMoney.Models;
using MoMoney.Services;
using MoMoney.Views;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MoMoney.ViewModels
{
    /// <summary>
    /// ViewModel for managing budgets, including CRUD operations and navigation.
    /// </summary>
    public partial class BudgetViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// Gets or sets a value indicating whether the view is busy loading or processing.
        /// </summary>
        [ObservableProperty] private bool isBusy;

        /// <summary>
        /// Gets or sets the title of the budgets view.
        /// </summary>
        [ObservableProperty] private string title = "Budgets";

        /// <summary>
        /// Gets or sets the collection of budgets displayed in the UI.
        /// </summary>
        [ObservableProperty] private ObservableCollection<BudgetItemViewModel> budgets = new();

        /// <summary>
        /// Gets or sets the list of available budget categories.
        /// </summary>
        [ObservableProperty] private ObservableCollection<string> categories = new();

        /// <summary>
        /// Gets or sets the currently selected category when creating or editing a budget.
        /// </summary>
        [ObservableProperty] private string selectedCategory;

        /// <summary>
        /// Gets or sets the form input value for the budget amount.
        /// </summary>
        [ObservableProperty] private string formAmount = string.Empty;

        /// <summary>
        /// Gets or sets the form input value for the budget period.
        /// </summary>
        [ObservableProperty] private string formPeriod = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="BudgetViewModel"/> class.
        /// </summary>
        /// <param name="dataService">The data service used to manage budgets and categories.</param>
        public BudgetViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// Loads budgets and related transactions asynchronously, seeding defaults if necessary.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        public async Task LoadBudgetsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                Budgets.Clear();

                LoggerService.LogInfo<BudgetViewModel>("Loading budgets...");

                // Seed defaults if empty
                await _dataService.SeedDefaultBudgetsAsync();
                await _dataService.SeedDefaultCategoriesAsync();

                // Load from DB
                var budgets = await _dataService.GetBudgetsAsync();
                var transactions = await _dataService.GetTransactionsAsync();

                foreach (var b in budgets)
                    Budgets.Add(new BudgetItemViewModel(b, transactions));

                LoggerService.LogInfo<BudgetViewModel>($"Loaded {Budgets.Count} budgets.");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<BudgetViewModel>(ex, "Failed to load budgets.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to load budgets.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }


        /*
        /// <summary>
        /// Adds a new budget asynchronously from form input values.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        public async Task AddBudgetAsync()
        {
            try
            {
                LoggerService.LogInfo<BudgetViewModel>($"Adding new budget for category '{SelectedCategory}'.");

                // Parse FormAmount safely
                if (!decimal.TryParse(FormAmount, NumberStyles.Currency, CultureInfo.CurrentCulture, out var parsedAmount))
                {
                    LoggerService.LogWarning<BudgetViewModel>("Invalid amount entered in AddBudgetAsync.");
                    await Application.Current.MainPage.DisplayAlert("Invalid Amount", "Please enter a valid amount.", "OK");
                    return;
                }

                var newBudget = new Budget
                {
                    Category = SelectedCategory,
                    Amount = parsedAmount,
                    Period = FormPeriod
                };

                await _dataService.AddBudgetAsync(newBudget);

                Budgets.Add(new BudgetItemViewModel(newBudget));

                LoggerService.LogInfo<BudgetViewModel>(
                    $"Added budget: {newBudget.Category}, Amount={newBudget.Amount:C}, Period={newBudget.Period}");

                // reset form
                SelectedCategory = null;
                FormAmount = string.Empty;
                FormPeriod = string.Empty;

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<BudgetViewModel>(ex, "Failed to add new budget.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to add budget.", "OK");
            }
        }
        */

        /// <summary>
        /// Adds a new budget asynchronously from form input values.
        /// Prevents duplicates with the same Category and Period.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        public async Task AddBudgetAsync()
        {
            try
            {
                LoggerService.LogInfo<BudgetViewModel>($"Adding new budget for category '{SelectedCategory}'.");

                // Parse FormAmount safely
                if (!decimal.TryParse(FormAmount, NumberStyles.Currency, CultureInfo.CurrentCulture, out var parsedAmount))
                {
                    LoggerService.LogWarning<BudgetViewModel>("Invalid amount entered in AddBudgetAsync.");
                    await Application.Current.MainPage.DisplayAlert("Invalid Amount", "Please enter a valid amount.", "OK");
                    return;
                }

                // Check for duplicates (same Category + Period)
                var existingBudgets = await _dataService.GetBudgetsAsync();
                bool duplicateExists = existingBudgets.Any(b =>
                    string.Equals(b.Category, SelectedCategory, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(b.Period, FormPeriod, StringComparison.OrdinalIgnoreCase));

                if (duplicateExists)
                {
                    LoggerService.LogWarning<BudgetViewModel>(
                        $"Duplicate budget prevented for Category='{SelectedCategory}', Period='{FormPeriod}'.");
                    await Application.Current.MainPage.DisplayAlert(
                        "Duplicate Budget",
                        $"A budget for '{SelectedCategory}' already exists for the period '{FormPeriod}'.",
                        "OK");
                    return;
                }

                var newBudget = new Budget
                {
                    Category = SelectedCategory,
                    Amount = parsedAmount,
                    Period = FormPeriod
                };

                await _dataService.AddBudgetAsync(newBudget);
                Budgets.Add(new BudgetItemViewModel(newBudget));

                LoggerService.LogInfo<BudgetViewModel>(
                    $"Added budget: {newBudget.Category}, Amount={newBudget.Amount:C}, Period={newBudget.Period}");

                // reset form
                SelectedCategory = null;
                FormAmount = string.Empty;
                FormPeriod = string.Empty;

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<BudgetViewModel>(ex, "Failed to add new budget.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to add budget.", "OK");
            }
        }


        /// <summary>
        /// Navigates to the AddBudgetPage asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task GoToAddBudget()
        {
            try
            {
                LoggerService.LogInfo<BudgetViewModel>("Navigating to AddBudgetPage...");

                await LoadCategoriesAsync();
                await Shell.Current.GoToAsync(nameof(AddBudgetPage));
            }
            catch (Exception ex)
            {
                LoggerService.LogError<BudgetViewModel>(ex, "Failed to navigate to AddBudgetPage.");
                await Application.Current.MainPage.DisplayAlert("Error", "Navigation failed.", "OK");
            }
        }

        /// <summary>
        /// Navigates to the EditBudgetPage for a specific budget asynchronously.
        /// </summary>
        /// <param name="budget">The budget item to edit.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task EditBudgetAsync(BudgetItemViewModel budget)
        {
            if (budget == null) return;

            try
            {
                LoggerService.LogInfo<BudgetViewModel>($"Navigating to EditBudgetPage for budget Id={budget.Id}.");

                var route = $"{nameof(EditBudgetPage)}?BudgetId={budget.Id}";
                await Shell.Current.GoToAsync(route);
            }
            catch (Exception ex)
            {
                LoggerService.LogError<BudgetViewModel>(ex, $"Failed to navigate to EditBudgetPage for Id={budget.Id}.");
                await Application.Current.MainPage.DisplayAlert("Error", "Edit failed.", "OK");
            }
        }

        /// <summary>
        /// Deletes a budget asynchronously after user confirmation.
        /// </summary>
        /// <param name="budget">The budget item to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        public async Task DeleteBudgetAsync(BudgetItemViewModel budget)
        {
            if (budget == null) return;

            try
            {
                LoggerService.LogInfo<BudgetViewModel>($"Attempting to delete budget Id={budget.Id}, Category='{budget.Category}'.");

                bool confirm = await Application.Current.MainPage.DisplayAlert(
                    "Delete Budget",
                    "Are you sure you want to delete this budget?",
                    "Yes", "No");

                if (!confirm)
                {
                    LoggerService.LogInfo<BudgetViewModel>("Delete cancelled by user.");
                    return;
                }

                await _dataService.DeleteBudgetAsync(new Budget
                {
                    Id = budget.Id,
                    Category = budget.Category,
                    Amount = budget.Amount,
                    Period = budget.Period
                });

                Budgets.Remove(budget);

                LoggerService.LogInfo<BudgetViewModel>($"Deleted budget Id={budget.Id}, Category='{budget.Category}'.");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<BudgetViewModel>(ex, $"Failed to delete budget Id={budget?.Id}.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete budget.", "OK");
            }
        }

        /// <summary>
        /// Cancels the current action and navigates back asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task CancelAsync()
        {
            try
            {
                LoggerService.LogInfo<BudgetViewModel>("Cancel action triggered. Navigating back...");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<BudgetViewModel>(ex, "Failed to navigate back in CancelAsync.");
            }
        }

        /// <summary>
        /// Loads categories asynchronously and populates the <see cref="Categories"/> collection.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task LoadCategoriesAsync()
        {
            try
            {
                LoggerService.LogInfo<BudgetViewModel>("Loading categories...");

                Categories.Clear();
                await _dataService.SeedDefaultCategoriesAsync();

                var items = await _dataService.GetCategoriesAsync();
                foreach (var c in items)
                    Categories.Add(c.Name);

                LoggerService.LogInfo<BudgetViewModel>($"Loaded {Categories.Count} categories.");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<BudgetViewModel>(ex, "Failed to load categories.");
            }
        }

        /// <summary>
        /// Adds a new category asynchronously if it does not already exist.
        /// Updates the <see cref="Categories"/> collection and persists the category.
        /// </summary>
        /// <param name="newCategory">The name of the new category to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task AddCategoryAsync(string newCategory)
        {
            try
            {
                newCategory = newCategory?.Trim();

                if (!string.IsNullOrWhiteSpace(newCategory) &&
                    !Categories.Any(c => c.Equals(newCategory, StringComparison.OrdinalIgnoreCase)))
                {
                    LoggerService.LogInfo<BudgetViewModel>($"Adding new category '{newCategory}' from BudgetViewModel.");

                    Categories.Add(newCategory);
                    SelectedCategory = newCategory;

                    await _dataService.AddCategoryAsync(new Category { Name = newCategory });

                    LoggerService.LogInfo<BudgetViewModel>($"New category '{newCategory}' added successfully.");
                }
                else
                {
                    LoggerService.LogWarning<BudgetViewModel>(
                        $"Skipped adding category '{newCategory}' (null/empty or already exists).");
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogError<BudgetViewModel>(ex, $"Failed to add category '{newCategory}'.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to add category.", "OK");
            }
        }
    }
}