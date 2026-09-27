using Hekki.UI.ViewModels;
using Hekki.UI.Views.Shared.Table;
using System.Windows.Controls;

namespace Hekki.UI.Views.Race
{
    public partial class RaceView : UserControl
    {
        public RaceView()
        {
            InitializeComponent();
            HeatsList.AddHandler(TableRow.CellCommitEvent, new CellCommitEventHandler(OnHeatCellCommit));
        }

        private void OnHeatCellCommit(object sender, CellCommitEventArgs e)
        {
            if (DataContext is not RaceViewModel viewModel || e.Row is not HeatRowViewModel row) return;

            e.Handled = true;
            _ = viewModel.HeatsTable.SaveCellAsync(row, e.Cell);
        }
    }
}
