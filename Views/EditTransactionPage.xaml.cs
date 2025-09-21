using MoMoney.ViewModels;

namespace MoMoney.Views
{
    /// <summary>
    /// Represents the page for editing an existing transaction.
    /// Provides UI bindings to the <see cref="EditTransactionViewModel"/>.
    /// </summary>
    [QueryProperty(nameof(TransactionId), "TransactionId")]
    public partial class EditTransactionPage : ContentPage
    {
        private readonly EditTransactionViewModel _viewModel;

        /// <summary>
        /// Gets or sets the transaction identifier passed as a query parameter.
        /// </summary>
        public int TransactionId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditTransactionPage"/> class.
        /// </summary>
        /// <param name="vm">The view model used for editing transaction data and logic.</param>
        public EditTransactionPage(EditTransactionViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            _viewModel = vm;
        }

        /// <summary>
        /// Called when the page appears. Loads the transaction details if a valid identifier is provided.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Load transaction once the page appears
            if (TransactionId > 0)
            {
                await _viewModel.LoadTransactionAsync(TransactionId);
            }
        }
    }
}
