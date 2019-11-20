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
using PlaylistRetriever.ViewModels;

namespace PlaylistRetriever.Infrastructure
{
    internal class ViewModelLocator
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

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<FormatColumnsDialogViewModel>();
            SimpleIoc.Default.Register<BuildKeyDialogViewModel>();
            SimpleIoc.Default.Register<ManualPlaylistRipperViewModel>();
        }

        // Properties //
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public FormatColumnsDialogViewModel FormatWindow => ServiceLocator.Current.GetInstance<FormatColumnsDialogViewModel>();

        public BuildKeyDialogViewModel BuildKeyDialog => ServiceLocator.Current.GetInstance<BuildKeyDialogViewModel>();

        public ManualPlaylistRipperViewModel ManualPlaylistRipper => ServiceLocator.Current.GetInstance<ManualPlaylistRipperViewModel>();

        // Methods //
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
