/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:PlaylistRetriever"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace PlaylistRetriever.ViewModels
{
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainWindowViewModel>();
            SimpleIoc.Default.Register<FormatWindowViewModel>();
            SimpleIoc.Default.Register<BuildKeyDialogViewModel>();
        }

        // Properties //
        public MainWindowViewModel Main
        {
            get => ServiceLocator.Current.GetInstance<MainWindowViewModel>();
        }

        public FormatWindowViewModel FormatWindow
        {
            get => ServiceLocator.Current.GetInstance<FormatWindowViewModel>();
        }

        public BuildKeyDialogViewModel BuildKeyDialog
        {
            get => ServiceLocator.Current.GetInstance<BuildKeyDialogViewModel>();
        }

        // Methods //
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
