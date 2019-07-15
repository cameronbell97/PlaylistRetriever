using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PlaylistRetriever
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FormatWindow : Window
    {
        // Declarations //
        private static FormatWindowViewModel viewModel;

        // Constructors //
        public FormatWindow()
        {
            InitializeComponent();
            viewModel = new FormatWindowViewModel(PossibleColumns());
            this.DataContext = viewModel;

            viewModel.TotalColumnItems = lvwTotalColumnBox;
            viewModel.OrderedColumnItems = lvwOrderedColumnBox;
            viewModel.PopulateListView();
        }

        // Properties //
        public List<PlaylistWriter.PlaylistColumn> ReturnColumns
        {
            get
            {
                if (viewModel == null)
                    return null;

                return viewModel.ReturningColumns;
            }
        }

        // TODO : Replace Event Handlers //
        // Event Handlers //
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Save();
            DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void BtnTakeAll_Click(object sender, RoutedEventArgs e)
        {
            viewModel.TakeAll();
        }

        private void BtnTake_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Take();
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Return();
        }

        private void BtnReturnAll_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ReturnAll();
        }

        // Private Methods //
        private List<PlaylistWriter.PlaylistColumn> PossibleColumns()
        {
            return new List<PlaylistWriter.PlaylistColumn>
            {
                PlaylistWriter.PlaylistColumn.TrackName,
                PlaylistWriter.PlaylistColumn.TrackArtists,
                PlaylistWriter.PlaylistColumn.AlbumName,
                PlaylistWriter.PlaylistColumn.AddedAt,
                PlaylistWriter.PlaylistColumn.AddedBy,
                PlaylistWriter.PlaylistColumn.TrackID,
                PlaylistWriter.PlaylistColumn.Duration,
                PlaylistWriter.PlaylistColumn.IsLocal
            };
        }
        
    }
}
