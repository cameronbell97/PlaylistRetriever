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
    /// Interaction logic for BuildKeyWindow.xaml
    /// </summary>
    public partial class BuildKeyWindow : Window
    {
        // Declarations //
        private static BuildKeyWindowViewModel ViewModel = new BuildKeyWindowViewModel();

        // Constructors //
        public BuildKeyWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        // Properties //
        public string ApiKey
        {
            get
            {
                return ViewModel.ApiKey;
            }
        }

        // Public Methods //
        private void BtnBuild_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.BuildKey();
            DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
