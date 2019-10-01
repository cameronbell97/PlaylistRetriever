using PlaylistRetriever.Models;
using PlaylistRetriever.ViewModels;

namespace PlaylistRetriever.Services
{
    class BuildKeyDialogService
    {
        public static ApiKeyResponse ShowBuildKeyDialog()
        {
            var dialog = new BuildKeyWindow();
            dialog.ShowDialog();

            return dialog.DataContext == null ? null : (dialog.DataContext as BuildKeyDialogViewModel).Response;
        }
    }
}
