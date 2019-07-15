using System.Windows;
using System.Windows.Controls;

namespace PlaylistRetriever
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Declarations //
        private static MainWindowViewModel ViewModel = new MainWindowViewModel();
        
        // Constructors //
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = ViewModel;
            ViewModel.LoadedPlaylistsList = lvwLoadedPlaylists;
        }

        // TODO : Implement loaded playlists box

        // TODO : Remove Event Handlers and use commands instead? Or something more MVVMy
        // Event Handlers //
        private void CmdClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void CmdSave_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.SavePlaylistFromID();
        }

        private async void CmdLoad_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadPlaylistFromIDAsync();
            ViewModel.CheckSaveButton();
        }

        private void TxtPlaylistID_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ViewModel.CheckSaveButton();
        }

        private void TxtSaveLocation_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ViewModel.CheckSaveButton();
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ViewModel.CheckSaveButton();
        }

        private void CmdFormat_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenFormatWindow();
        }

        private void CmdBrowseFilesystem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CmdDefault_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.FormatColumnsAsDefault();
        }

        private void BtnBuildAccessKey_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.BuildKeyButtonClick();
        }
    }
}
