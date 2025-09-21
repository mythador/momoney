using MoMoney.ViewModels;

namespace MoMoney.Views
{
    /// <summary>
    /// Represents the page that displays and manages transactions.
    /// Provides UI bindings to the <see cref="TransactionViewModel"/>.
    /// </summary>
    public partial class TransactionsPage : ContentPage
    {
        private readonly TransactionViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionsPage"/> class.
        /// </summary>
        /// <param name="vm">The view model used for managing transactions.</param>
        public TransactionsPage(TransactionViewModel vm)
        {
            InitializeComponent();
            BindingContext = _viewModel = vm;
        }

        /// <summary>
        /// Called when the page appears. Refreshes transactions to ensure the latest data is loaded.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Always refresh transactions on page load
            await _viewModel.LoadTransactionsAsync();
        }
    }
}
