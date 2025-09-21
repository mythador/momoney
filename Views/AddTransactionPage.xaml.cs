using MoMoney.ViewModels;

namespace MoMoney.Views
{
    /// <summary>
    /// Represents the page for adding a new transaction.
    /// Provides UI bindings to the <see cref="TransactionViewModel"/>.
    /// </summary>
    public partial class AddTransactionPage : ContentPage
    {
        private readonly TransactionViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddTransactionPage"/> class.
        /// </summary>
        /// <param name="vm">The view model used for managing transaction data and logic.</param>
        public AddTransactionPage(TransactionViewModel vm)
        {
            InitializeComponent();
            BindingContext = _viewModel = vm;
        }

        /// <summary>
        /// Called when the page appears. Reloads categories to ensure the latest data is available.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Always reload categories when the page appears
            await _viewModel.LoadCategoriesAsync();
        }
    }
}
