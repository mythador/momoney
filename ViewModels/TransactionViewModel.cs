/*
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoMoney.Models;
using MoMoney.Services;
using MoMoney.Views;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MoMoney.ViewModels
{
    public partial class TransactionViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private string title = "Transactions";
        [ObservableProperty] private ObservableCollection<Transaction> transactions = new();
        [ObservableProperty] private ObservableCollection<string> categories = new();
        [ObservableProperty] private string selectedCategory;

        // Form fields
        [ObservableProperty] private string formAmount = string.Empty;
        [ObservableProperty] private string formNotes = string.Empty;
        [ObservableProperty] private bool isIncome;
        [ObservableProperty] private DateTime date = DateTime.Today;

        public TransactionViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        [RelayCommand]
        public async Task LoadTransactionsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                Transactions.Clear();

                LoggerService.LogInfo<TransactionViewModel>("Loading transactions...");

                var items = await _dataService.GetTransactionsAsync();
                foreach (var t in items)
                    Transactions.Add(t);

                LoggerService.LogInfo<TransactionViewModel>($"Loaded {Transactions.Count} transactions.");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<TransactionViewModel>(ex, "Failed to load transactions.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to load transactions.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task AddTransactionAsync()
        {
            try
            {
                LoggerService.LogInfo<TransactionViewModel>("Adding new transaction...");

                if (!decimal.TryParse(FormAmount, NumberStyles.Currency, CultureInfo.CurrentCulture, out var parsedAmount))
                {
                    LoggerService.LogWarning<TransactionViewModel>("Invalid amount entered in AddTransactionAsync.");
                    await Application.Current.MainPage.DisplayAlert("Invalid Amount", "Please enter a valid amount.", "OK");
                    return;
                }

                var newTransaction = new Transaction
                {
                    Amount = parsedAmount,
                    Category = string.IsNullOrWhiteSpace(SelectedCategory) ? "Uncategorized" : SelectedCategory,
                    Notes = FormNotes,
                    IsIncome = IsIncome,
                    Date = Date
                };

                await _dataService.AddTransactionAsync(newTransaction);

                LoggerService.LogInfo<TransactionViewModel>(
                    $"Added transaction: Amount={parsedAmount:C}, Category='{newTransaction.Category}', IsIncome={IsIncome}");

                // Reset form
                FormAmount = string.Empty;
                SelectedCategory = null;
                FormNotes = string.Empty;
                IsIncome = false;
                Date = DateTime.Today;

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<TransactionViewModel>(ex, "Failed to add transaction.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to add transaction.", "OK");
            }
        }

        [RelayCommand]
        private async Task GoToAddTransaction()
        {
            try
            {
                LoggerService.LogInfo<TransactionViewModel>("Navigating to AddTransactionPage...");
                await Shell.Current.GoToAsync(nameof(AddTransactionPage));
            }
            catch (Exception ex)
            {
                LoggerService.LogError<TransactionViewModel>(ex, "Failed to navigate to AddTransactionPage.");
                await Application.Current.MainPage.DisplayAlert("Error", "Navigation failed.", "OK");
            }
        }

        [RelayCommand]
        private async Task EditTransactionAsync(Transaction transaction)
        {
            if (transaction == null) return;

            try
            {
                LoggerService.LogInfo<TransactionViewModel>($"Navigating to EditTransactionPage for Id={transaction.Id}.");

                var route = $"{nameof(EditTransactionPage)}?TransactionId={transaction.Id}";
                await Shell.Current.GoToAsync(route);
            }
            catch (Exception ex)
            {
                LoggerService.LogError<TransactionViewModel>(ex, $"Failed to navigate to EditTransactionPage for Id={transaction.Id}.");
                await Application.Current.MainPage.DisplayAlert("Error", "Edit failed.", "OK");
            }
        }

        [RelayCommand]
        public async Task DeleteTransactionAsync(Transaction transaction)
        {
            if (transaction == null) return;

            try
            {
                LoggerService.LogInfo<TransactionViewModel>($"Attempting to delete transaction Id={transaction.Id}.");

                bool confirm = await Application.Current.MainPage.DisplayAlert(
                    "Delete Transaction",
                    "Are you sure you want to delete this transaction?",
                    "Yes", "No");

                if (!confirm)
                {
                    LoggerService.LogInfo<TransactionViewModel>($"Delete cancelled for transaction Id={transaction.Id}.");
                    return;
                }

                await _dataService.DeleteTransactionAsync(transaction);
                Transactions.Remove(transaction);

                LoggerService.LogInfo<TransactionViewModel>($"Deleted transaction Id={transaction.Id}.");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<TransactionViewModel>(ex, $"Failed to delete transaction Id={transaction?.Id}.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete transaction.", "OK");
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            try
            {
                LoggerService.LogInfo<TransactionViewModel>("Cancel triggered. Navigating back...");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<TransactionViewModel>(ex, "Failed to navigate back in CancelAsync.");
            }
        }

        [RelayCommand]
        private async Task AddCategory(string newCategory)
        {
            try
            {
                newCategory = newCategory.Trim();

                if (!string.IsNullOrWhiteSpace(newCategory) &&
                    !Categories.Any(c => c.Equals(newCategory, StringComparison.OrdinalIgnoreCase)))
                {
                    LoggerService.LogInfo<TransactionViewModel>($"Adding new category '{newCategory}' from TransactionViewModel.");

                    Categories.Add(newCategory);
                    SelectedCategory = newCategory;

                    await _dataService.AddCategoryAsync(new Category { Name = newCategory });

                    LoggerService.LogInfo<TransactionViewModel>($"Category '{newCategory}' added successfully.");
                }
                else
                {
                    LoggerService.LogWarning<TransactionViewModel>(
                        $"Skipped adding category '{newCategory}' (null/empty or already exists).");
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogError<TransactionViewModel>(ex, $"Failed to add category '{newCategory}'.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to add category.", "OK");
            }
        }

        public async Task LoadCategoriesAsync()
        {
            try
            {
                LoggerService.LogInfo<TransactionViewModel>("Loading categories...");

                Categories.Clear();
                await _dataService.SeedDefaultCategoriesAsync();

                var items = await _dataService.GetCategoriesAsync();
                foreach (var c in items)
                    Categories.Add(c.Name);

                LoggerService.LogInfo<TransactionViewModel>($"Loaded {Categories.Count} categories.");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<TransactionViewModel>(ex, "Failed to load categories.");
            }
        }
    }
}
*/

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
    /// ViewModel for managing transactions, including creation, editing,
    /// deletion, and category management.
    /// </summary>
    public partial class TransactionViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// Gets or sets a value indicating whether the view is busy loading or processing.
        /// </summary>
        [ObservableProperty] private bool isBusy;

        /// <summary>
        /// Gets or sets the title of the transactions page.
        /// </summary>
        [ObservableProperty] private string title = "Transactions";

        /// <summary>
        /// Gets or sets the collection of transactions.
        /// </summary>
        [ObservableProperty] private ObservableCollection<Transaction> transactions = new();

        /// <summary>
        /// Gets or sets the collection of available categories.
        /// </summary>
        [ObservableProperty] private ObservableCollection<string> categories = new();

        /// <summary>
        /// Gets or sets the currently selected category.
        /// </summary>
        [ObservableProperty] private string selectedCategory;

        /// <summary>
        /// Gets or sets the amount field value in the transaction form.
        /// </summary>
        [ObservableProperty] private string formAmount = string.Empty;

        /// <summary>
        /// Gets or sets the notes field value in the transaction form.
        /// </summary>
        [ObservableProperty] private string formNotes = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the transaction is income.
        /// </summary>
        [ObservableProperty] private bool isIncome;

        /// <summary>
        /// Gets or sets the transaction date.
        /// </summary>
        [ObservableProperty] private DateTime date = DateTime.Today;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionViewModel"/> class.
        /// </summary>
        /// <param name="dataService">The data service for accessing transactions and categories.</param>
        public TransactionViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// Loads transactions asynchronously and populates the <see cref="Transactions"/> collection.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        public async Task LoadTransactionsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                Transactions.Clear();

                LoggerService.LogInfo<TransactionViewModel>("Loading transactions...");

                var items = await _dataService.GetTransactionsAsync();
                foreach (var t in items)
                    Transactions.Add(t);

                LoggerService.LogInfo<TransactionViewModel>($"Loaded {Transactions.Count} transactions.");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<TransactionViewModel>(ex, "Failed to load transactions.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to load transactions.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Adds a new transaction asynchronously using form input values.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        public async Task AddTransactionAsync()
        {
            try
            {
                LoggerService.LogInfo<TransactionViewModel>("Adding new transaction...");

                if (!decimal.TryParse(FormAmount, NumberStyles.Currency, CultureInfo.CurrentCulture, out var parsedAmount))
                {
                    LoggerService.LogWarning<TransactionViewModel>("Invalid amount entered in AddTransactionAsync.");
                    await Application.Current.MainPage.DisplayAlert("Invalid Amount", "Please enter a valid amount.", "OK");
                    return;
                }

                var newTransaction = new Transaction
                {
                    Amount = parsedAmount,
                    Category = string.IsNullOrWhiteSpace(SelectedCategory) ? "Uncategorized" : SelectedCategory,
                    Notes = FormNotes,
                    IsIncome = IsIncome,
                    Date = Date
                };

                await _dataService.AddTransactionAsync(newTransaction);

                LoggerService.LogInfo<TransactionViewModel>(
                    $"Added transaction: Amount={parsedAmount:C}, Category='{newTransaction.Category}', IsIncome={IsIncome}");

                // Reset form
                FormAmount = string.Empty;
                SelectedCategory = null;
                FormNotes = string.Empty;
                IsIncome = false;
                Date = DateTime.Today;

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<TransactionViewModel>(ex, "Failed to add transaction.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to add transaction.", "OK");
            }
        }

        /// <summary>
        /// Navigates asynchronously to the AddTransaction page.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task GoToAddTransaction()
        {
            try
            {
                LoggerService.LogInfo<TransactionViewModel>("Navigating to AddTransactionPage...");
                await Shell.Current.GoToAsync(nameof(AddTransactionPage));
            }
            catch (Exception ex)
            {
                LoggerService.LogError<TransactionViewModel>(ex, "Failed to navigate to AddTransactionPage.");
                await Application.Current.MainPage.DisplayAlert("Error", "Navigation failed.", "OK");
            }
        }

        /// <summary>
        /// Navigates asynchronously to the EditTransaction page for the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction to edit.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task EditTransactionAsync(Transaction transaction)
        {
            if (transaction == null) return;

            try
            {
                LoggerService.LogInfo<TransactionViewModel>($"Navigating to EditTransactionPage for Id={transaction.Id}.");

                var route = $"{nameof(EditTransactionPage)}?TransactionId={transaction.Id}";
                await Shell.Current.GoToAsync(route);
            }
            catch (Exception ex)
            {
                LoggerService.LogError<TransactionViewModel>(ex, $"Failed to navigate to EditTransactionPage for Id={transaction.Id}.");
                await Application.Current.MainPage.DisplayAlert("Error", "Edit failed.", "OK");
            }
        }

        /// <summary>
        /// Deletes a transaction asynchronously after user confirmation.
        /// </summary>
        /// <param name="transaction">The transaction to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        public async Task DeleteTransactionAsync(Transaction transaction)
        {
            if (transaction == null) return;

            try
            {
                LoggerService.LogInfo<TransactionViewModel>($"Attempting to delete transaction Id={transaction.Id}.");

                bool confirm = await Application.Current.MainPage.DisplayAlert(
                    "Delete Transaction",
                    "Are you sure you want to delete this transaction?",
                    "Yes", "No");

                if (!confirm)
                {
                    LoggerService.LogInfo<TransactionViewModel>($"Delete cancelled for transaction Id={transaction.Id}.");
                    return;
                }

                await _dataService.DeleteTransactionAsync(transaction);
                Transactions.Remove(transaction);

                LoggerService.LogInfo<TransactionViewModel>($"Deleted transaction Id={transaction.Id}.");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<TransactionViewModel>(ex, $"Failed to delete transaction Id={transaction?.Id}.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete transaction.", "OK");
            }
        }

        /// <summary>
        /// Cancels the current operation and navigates back asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task CancelAsync()
        {
            try
            {
                LoggerService.LogInfo<TransactionViewModel>("Cancel triggered. Navigating back...");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<TransactionViewModel>(ex, "Failed to navigate back in CancelAsync.");
            }
        }

        /// <summary>
        /// Adds a new category asynchronously from the transaction form.
        /// </summary>
        /// <param name="newCategory">The new category name to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task AddCategory(string newCategory)
        {
            try
            {
                newCategory = newCategory.Trim();

                if (!string.IsNullOrWhiteSpace(newCategory) &&
                    !Categories.Any(c => c.Equals(newCategory, StringComparison.OrdinalIgnoreCase)))
                {
                    LoggerService.LogInfo<TransactionViewModel>($"Adding new category '{newCategory}' from TransactionViewModel.");

                    Categories.Add(newCategory);
                    SelectedCategory = newCategory;

                    await _dataService.AddCategoryAsync(new Category { Name = newCategory });

                    LoggerService.LogInfo<TransactionViewModel>($"Category '{newCategory}' added successfully.");
                }
                else
                {
                    LoggerService.LogWarning<TransactionViewModel>(
                        $"Skipped adding category '{newCategory}' (null/empty or already exists).");
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogError<TransactionViewModel>(ex, $"Failed to add category '{newCategory}'.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to add category.", "OK");
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
                LoggerService.LogInfo<TransactionViewModel>("Loading categories...");

                Categories.Clear();
                await _dataService.SeedDefaultCategoriesAsync();

                var items = await _dataService.GetCategoriesAsync();
                foreach (var c in items)
                    Categories.Add(c.Name);

                LoggerService.LogInfo<TransactionViewModel>($"Loaded {Categories.Count} categories.");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<TransactionViewModel>(ex, "Failed to load categories.");
            }
        }
    }
}