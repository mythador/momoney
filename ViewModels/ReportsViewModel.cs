using CommunityToolkit.Mvvm.ComponentModel;
using MoMoney.Models;
using MoMoney.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using Microcharts; // Needed for Reports Charts
using SkiaSharp; // Needed for Reports Charts

namespace MoMoney.ViewModels
{
    /// <summary>
    /// ViewModel responsible for generating and managing financial reports,
    /// including monthly summaries, category breakdowns, and budget utilization.
    /// </summary>
    public partial class ReportsViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// Gets or sets the category chart for visualizing category-based spending.
        /// </summary>
        [ObservableProperty] private Chart categoryChart;

        /// <summary>
        /// Gets or sets the title of the reports page.
        /// </summary>
        [ObservableProperty] private string title = "Reports";

        /// <summary>
        /// Gets or sets the collection of monthly income vs. expenses summaries.
        /// </summary>
        [ObservableProperty] private ObservableCollection<MonthlySummary> monthlySummaries = new();

        /// <summary>
        /// Gets or sets the collection of reports grouped by category.
        /// </summary>
        [ObservableProperty] private ObservableCollection<CategoryReport> categoryReports = new();

        /// <summary>
        /// Gets or sets the collection of budget utilization reports.
        /// </summary>
        [ObservableProperty] private ObservableCollection<BudgetReport> budgetReports = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportsViewModel"/> class.
        /// </summary>
        /// <param name="dataService">The data service for accessing transactions and budgets.</param>
        public ReportsViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// Asynchronously loads report data, including monthly summaries, category breakdowns,
        /// and budget utilization reports. Updates charts and logs progress.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task LoadReportsAsync()
        {
            try
            {
                LoggerService.LogInfo<ReportsViewModel>("Loading reports...");

                MonthlySummaries.Clear();
                CategoryReports.Clear();
                BudgetReports.Clear();

                // Load transactions & budgets from DB
                var transactions = await _dataService.GetTransactionsAsync();
                var budgets = await _dataService.GetBudgetsAsync();

                LoggerService.LogInfo<ReportsViewModel>(
                    $"Fetched {transactions.Count()} transactions and {budgets.Count()} budgets from DB.");

                // --- Monthly Income vs. Expenses ---
                var monthlyGroups = transactions
                    .GroupBy(t => new { t.Date.Year, t.Date.Month })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month);

                foreach (var g in monthlyGroups)
                {
                    var income = g.Where(t => t.IsIncome).Sum(t => t.Amount);
                    var expenses = g.Where(t => !t.IsIncome).Sum(t => t.Amount);

                    var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month);
                    MonthlySummaries.Add(new MonthlySummary
                    {
                        Month = $"{monthName} {g.Key.Year}",
                        Income = income,
                        Expenses = expenses
                    });
                }

                LoggerService.LogInfo<ReportsViewModel>(
                    $"Built {MonthlySummaries.Count} monthly summaries.");

                // --- Category Breakdown ---
                var categoryGroups = transactions
                    .Where(t => !t.IsIncome)
                    .GroupBy(t => t.Category);

                var chartEntries = new List<ChartEntry>();

                foreach (var g in categoryGroups)
                {
                    var category = g.Key ?? "Uncategorized";
                    var total = g.Sum(t => t.Amount);

                    var colorHex = "#" + Random.Shared.Next(0x1000000).ToString("X6");
                    var sk = SKColor.Parse(colorHex);

                    CategoryReports.Add(new CategoryReport
                    {
                        Category = category,
                        Amount = total,
                        ColorHex = colorHex
                    });

                    chartEntries.Add(new ChartEntry((float)total)
                    {
                        Color = sk,
                        Label = string.Empty,
                        ValueLabel = string.Empty,
                        TextColor = SKColors.Transparent,
                        ValueLabelColor = SKColors.Transparent
                    });
                }

                CategoryChart = new DonutChart
                {
                    Entries = chartEntries,
                    BackgroundColor = SKColors.Transparent,
                    HoleRadius = 0.6f
                };

                LoggerService.LogInfo<ReportsViewModel>(
                    $"Built {CategoryReports.Count} category reports.");

                // --- Budget Utilization ---
                foreach (var b in budgets)
                {
                    var spent = transactions
                        .Where(t => !t.IsIncome && t.Category == b.Category)
                        .Sum(t => t.Amount);

                    var progress = b.Amount > 0 ? (double)(spent / b.Amount) : 0;
                    if (progress > 1) progress = 1;

                    BudgetReports.Add(new BudgetReport
                    {
                        Category = b.Category,
                        Progress = progress,
                        Spent = spent,
                        Limit = b.Amount
                    });

                    if (spent > b.Amount)
                    {
                        LoggerService.LogWarning<ReportsViewModel>(
                            $"Budget overrun: Category='{b.Category}', Spent={spent:C}, Limit={b.Amount:C}");
                    }
                }

                LoggerService.LogInfo<ReportsViewModel>(
                    $"Built {BudgetReports.Count} budget reports.");

                LoggerService.LogInfo<ReportsViewModel>("Reports loaded successfully.");
            }
            catch (Exception ex)
            {
                LoggerService.LogError<ReportsViewModel>(ex, "Failed to load reports.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to load reports.", "OK");
            }
        }
    }
}
