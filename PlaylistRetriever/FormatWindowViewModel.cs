using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace PlaylistRetriever
{
    class FormatWindowViewModel : INotifyPropertyChanged
    {
        // Declarations //
        private List<PlaylistWriter.PlaylistColumn> remainingColumns;
        private List<PlaylistWriter.PlaylistColumn> takenColumns;

        public event PropertyChangedEventHandler PropertyChanged;

        // Constructors //
        public FormatWindowViewModel(List<PlaylistWriter.PlaylistColumn> playlistColumns)
        {
            TakeEnabled = true;
            ReturnEnabled = false;
            remainingColumns = playlistColumns;

            takenColumns = new List<PlaylistWriter.PlaylistColumn>();
            ReturningColumns = new List<PlaylistWriter.PlaylistColumn>();
        }

        // Properties //
        public bool TakeEnabled { get; set; }
        public bool ReturnEnabled { get; set; }
        public List<PlaylistWriter.PlaylistColumn> ReturningColumns { get; private set; }
        public ListView TotalColumnItems { get; set; }
        public ListView OrderedColumnItems { get; set; }

        // Public Methods //
        internal void Save()
        {
            foreach (string sItem in OrderedColumnItems.Items)
            {
                ReturningColumns.Add(RecreateColumn(sItem));
            }
        }

        internal void TakeAll()
        {
            foreach (string item in TotalColumnItems.Items)
            {
                OrderedColumnItems.Items.Add(item);
            }
            TotalColumnItems.Items.Clear();
        }

        internal void Take()
        {
            int selectIndex = TotalColumnItems.SelectedIndex;
            if (selectIndex < 0 || selectIndex >= TotalColumnItems.Items.Count)
                return;

            OrderedColumnItems.Items.Add(TotalColumnItems.SelectedItem);
            TotalColumnItems.Items.Remove(TotalColumnItems.SelectedItem);

            TotalColumnItems.SelectedIndex = TotalColumnItems.Items.IsEmpty ? -1 : (TotalColumnItems.Items.Count > selectIndex) ? selectIndex : selectIndex - 1;
        }

        internal void Return()
        {
            int selectIndex = OrderedColumnItems.SelectedIndex;
            if (selectIndex < 0 || selectIndex >= OrderedColumnItems.Items.Count)
                return;

            TotalColumnItems.Items.Add(OrderedColumnItems.SelectedItem);
            OrderedColumnItems.Items.Remove(OrderedColumnItems.SelectedItem);

            OrderedColumnItems.SelectedIndex = OrderedColumnItems.Items.IsEmpty ? -1 : (OrderedColumnItems.Items.Count > selectIndex) ? selectIndex : selectIndex - 1;
        }

        internal void ReturnAll()
        {
            foreach (string item in OrderedColumnItems.Items)
            {
                TotalColumnItems.Items.Add(item);
            }
            OrderedColumnItems.Items.Clear();
        }

        public void PopulateListView()
        {
            if (remainingColumns == null || TotalColumnItems == null || OrderedColumnItems == null)
                return;

            foreach (PlaylistWriter.PlaylistColumn col in remainingColumns)
            {
                TotalColumnItems.Items.Add(col.ToString());
            }
        }

        // Protected & Private Methods //
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private PlaylistWriter.PlaylistColumn RecreateColumn(string columnName)
        {
            if (columnName == PlaylistWriter.PlaylistColumn.TrackName.ToString())
                return PlaylistWriter.PlaylistColumn.TrackName;
            else if (columnName == PlaylistWriter.PlaylistColumn.TrackArtists.ToString())
                return PlaylistWriter.PlaylistColumn.TrackArtists;
            else if (columnName == PlaylistWriter.PlaylistColumn.AlbumName.ToString())
                return PlaylistWriter.PlaylistColumn.AlbumName;
            else if (columnName == PlaylistWriter.PlaylistColumn.Duration.ToString())
                return PlaylistWriter.PlaylistColumn.Duration;
            else if (columnName == PlaylistWriter.PlaylistColumn.AddedAt.ToString())
                return PlaylistWriter.PlaylistColumn.AddedAt;
            else if (columnName == PlaylistWriter.PlaylistColumn.AddedBy.ToString())
                return PlaylistWriter.PlaylistColumn.AddedBy;
            else if (columnName == PlaylistWriter.PlaylistColumn.IsLocal.ToString())
                return PlaylistWriter.PlaylistColumn.IsLocal;
            else if (columnName == PlaylistWriter.PlaylistColumn.TrackID.ToString())
                return PlaylistWriter.PlaylistColumn.TrackID;
            else
                return PlaylistWriter.PlaylistColumn.TrackName;
        }
    }
}
