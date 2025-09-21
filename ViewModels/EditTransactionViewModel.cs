using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoMoney.Models;
using MoMoney.Services;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MoMoney.ViewModels
{
    /// <summary>
    /// ViewModel for editing an existing transaction, including updating, deleting, and category management.
    /// </summary>
    public partial class EditTransactionViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// Gets or sets the identifier of the transaction being edited.
        /// </summary>
        [ObservableProperty] private int id;

        /// <summary>
        /// Gets or sets the amount field value in the edit form.
        /// </summary>
        [ObservableProperty] private string formAmount;

        /// <summary>
        /// Gets or sets the currently selected category.
        /// </summary>
        [ObservableProperty] private string selectedCategory;

        /// <summary>
        /// Gets or sets the notes field value in the edit form.
        /// </summary>
        [ObservableProperty] private string formNotes;

        /// <summary>
        /// Gets or sets a value indicating whether the transaction is income.
        /// </summary>
        [ObservableProperty] private bool isIncome;

        /// <summary>
        /// Gets or sets the date of the transaction.
        /// </summary>
        [ObservableProperty] private DateTime date = DateTime.Today;

        /// <summary>
        /// Gets the collection of available categories for the transaction.
        /// </summary>
        public ObservableCollection<string> Categories { get; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="EditTransactionViewModel"/> class.
        /// </summary>
        /// <param name="dataService">The data service used to access and update transactions.</param>
        public EditTransactionViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// Loads a transaction asynchronously for editing by its identifier.
        /// </summary>
        /// <param name="transactionId">The identifier of the transaction to load.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task LoadTransactionAsync(int transactionId)
        {
            try
            {
                LoggerService.LogInfo<EditTransactionViewModel>($"Loading transaction Id={transactionId}...");

                var tx = (await _dataService.GetTransactionsAsync())
                            .FirstOrDefault(t => t.Id == transactionId);

                if (tx != null)
                {
                    Id = tx.Id;
                    FormAmount = tx.Amount.ToString("C");
                    FormNotes = tx.Notes ?? string.Empty;
                    IsIncome = tx.IsIncome;
                    Date = tx.Date;

                    await LoadCategoriesAsync();

                    SelectedCategory = Categories.FirstOrDefault(c =>
                        c.Equals(tx.Category, StringComparison.OrdinalIgnoreCase));

                    LoggerService.LogInfo<EditTransactionViewModel>(
                        $"Loaded transaction Id={Id}, Amount={FormAmount}, Category='{SelectedCategory}', IsIncome={IsIncome}");
                }
                else
                {
                    LoggerService.LogWarning<EditTransactionViewModel>($"No transaction found for Id={transactionId}.");
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogError<EditTransactionViewModel>(ex, $"Failed to load transaction Id={transactionId}.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to load transaction.", "OK");
            }
        }

        /// <summary>
        /// Loads available categories asynchronously and ensures the selected category is included.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task LoadCategoriesAsync()
        {
            try
            {
                LoggerService.LogInfo<EditTransactionViewModel>("Loading categories...");

                await _dataService.SeedDefaultCategoriesAsync();

                Categories.Clear();
                var items = await _dataService.GetCategoriesAsync();
                foreach (var c in items)
                    Categories.Add(c.Name);

                if (!string.IsNullOrWhiteSpace(SelectedCategory) &&
                    !Categories.Contains(SelectedCategory))
                {
                    Categories.Add(SelectedCategory);
                }

                LoggerService.LogInfo<EditTransactionViewModel>($"Loaded {Categories.Count} categories.");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<EditTransactionViewModel>(ex, "Failed to load categories.");
            }
        }

        /// <summary>
        /// Saves the currently edited transaction asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task SaveTransactionAsync()
        {
            try
            {
                LoggerService.LogInfo<EditTransactionViewModel>($"Saving transaction Id={Id}...");

                if (!decimal.TryParse(FormAmount, NumberStyles.Currency, CultureInfo.CurrentCulture, out var parsedAmount))
                {
                    LoggerService.LogWarning<EditTransactionViewModel>("Invalid amount entered in SaveTransactionAsync.");
                    await Application.Current.MainPage.DisplayAlert("Invalid Amount", "Please enter a valid amount.", "OK");
                    return;
                }

                var updated = new Transaction
                {
                    Id = Id,
                    Amount = parsedAmount,
                    Category = string.IsNullOrWhiteSpace(SelectedCategory) ? "Uncategorized" : SelectedCategory,
                    Notes = FormNotes,
                    IsIncome = IsIncome,
                    Date = Date
                };

                await _dataService.UpdateTransactionAsync(updated);

                LoggerService.LogInfo<EditTransactionViewModel>(
                    $"Updated transaction Id={Id}, Amount={parsedAmount:C}, Category='{updated.Category}', IsIncome={IsIncome}");

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<EditTransactionViewModel>(ex, $"Failed to save transaction Id={Id}.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to save transaction.", "OK");
            }
        }

        /// <summary>
        /// Deletes the currently loaded transaction asynchronously after user confirmation.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task DeleteTransactionAsync()
        {
            try
            {
                LoggerService.LogInfo<EditTransactionViewModel>($"Attempting to delete transaction Id={Id}...");

                bool confirm = await Application.Current.MainPage.DisplayAlert(
                    "Delete Transaction",
                    "Are you sure you want to delete this transaction?",
                    "Yes", "No");

                if (!confirm)
                {
                    LoggerService.LogInfo<EditTransactionViewModel>($"Delete cancelled for transaction Id={Id}.");
                    return;
                }

                await _dataService.DeleteTransactionAsync(new Transaction { Id = Id });

                LoggerService.LogInfo<EditTransactionViewModel>($"Deleted transaction Id={Id}.");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<EditTransactionViewModel>(ex, $"Failed to delete transaction Id={Id}.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete transaction.", "OK");
            }
        }

        /// <summary>
        /// Cancels the current edit operation and navigates back asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task CancelAsync()
        {
            try
            {
                LoggerService.LogInfo<EditTransactionViewModel>($"Cancel triggered for transaction Id={Id}, navigating back.");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<EditTransactionViewModel>(ex, $"Failed to navigate back in CancelAsync for transaction Id={Id}.");
            }
        }

        /// <summary>
        /// Adds a new category asynchronously from the edit screen.
        /// </summary>
        /// <param name="newCategory">The new category name to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task AddCategoryAsync(string newCategory)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(newCategory) && !Categories.Contains(newCategory))
                {
                    LoggerService.LogInfo<EditTransactionViewModel>($"Adding new category '{newCategory}' from EditTransactionViewModel.");

                    Categories.Add(newCategory);
                    SelectedCategory = newCategory;

                    await _dataService.AddCategoryAsync(new Category { Name = newCategory });

                    LoggerService.LogInfo<EditTransactionViewModel>($"New category '{newCategory}' added successfully.");
                }
                else
                {
                    LoggerService.LogWarning<EditTransactionViewModel>(
                        $"Skipped adding category '{newCategory}' (null/empty or already exists).");
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogError<EditTransactionViewModel>(ex, $"Failed to add category '{newCategory}'.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to add category.", "OK");
            }
        }
    }
}
