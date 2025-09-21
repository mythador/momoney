using MoMoney.ViewModels;

namespace MoMoney.Views
{
    /// <summary>
    /// Represents the page for editing an existing budget.
    /// Provides UI bindings to the <see cref="EditBudgetViewModel"/>.
    /// </summary>
    [QueryProperty(nameof(BudgetId), "BudgetId")]
    public partial class EditBudgetPage : ContentPage
    {
        private readonly EditBudgetViewModel _viewModel;

        /// <summary>
        /// Gets or sets the budget identifier passed as a query parameter.
        /// </summary>
        public int BudgetId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditBudgetPage"/> class.
        /// </summary>
        /// <param name="vm">The view model used for editing budget data and logic.</param>
        public EditBudgetPage(EditBudgetViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            _viewModel = vm;
        }

        /// <summary>
        /// Called when the page appears. Loads the budget details if a valid identifier is provided.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Load budget once the query parameter has been set
            if (BudgetId > 0)
            {
                await _viewModel.LoadBudgetAsync(BudgetId);
            }
        }
    }
}
