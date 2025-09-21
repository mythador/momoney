using MoMoney.ViewModels;

namespace MoMoney.Views
{
    /// <summary>
    /// Represents the page that displays financial reports.
    /// Provides UI bindings to the <see cref="ReportsViewModel"/>.
    /// </summary>
    public partial class ReportsPage : ContentPage
    {
        private readonly ReportsViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportsPage"/> class.
        /// </summary>
        /// <param name="vm">The view model used for generating and managing reports.</param>
        public ReportsPage(ReportsViewModel vm)
        {
            InitializeComponent();
            BindingContext = _viewModel = vm;
        }

        /// <summary>
        /// Called when the page appears. Refreshes reports to ensure the latest data is loaded.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Always refresh transactions on page load
            await _viewModel.LoadReportsAsync();
        }
    }
}
