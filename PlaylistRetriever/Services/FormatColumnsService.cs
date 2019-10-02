using PlaylistRetriever.Models;
using PlaylistRetriever.ViewModels;

namespace PlaylistRetriever.Services
{
    class FormatColumnsService
    {
        public static FormatColumnsResponse ShowFormatColumnsDialog()
        {
            var dialog = new FormatWindow();
            dialog.ShowDialog();

            return dialog.DataContext == null ? null : (dialog.DataContext as FormatWindowViewModel).Response;
        }
    }
}
