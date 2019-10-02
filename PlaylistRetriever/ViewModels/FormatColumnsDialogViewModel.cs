using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PlaylistRetriever.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace PlaylistRetriever.ViewModels
{
    public class FormatColumnsDialogViewModel : ViewModelBase
    {
        // Declarations //
        private ObservableCollection<PlaylistWriter.PlaylistColumn> _remainingColumns;
        private ObservableCollection<PlaylistWriter.PlaylistColumn> _takenColumns;
        private int _totalColumnIndex;
        private int _orderedColumnIndex;

        private DialogResultAction dialogResult;

        // Constructors //
        public FormatColumnsDialogViewModel()
        {
            // Initialize Declarations
            TakeEnabled = true;
            ReturnEnabled = false;
            TotalColumnItems = new ObservableCollection<PlaylistWriter.PlaylistColumn>
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
            OrderedColumnItems = new ObservableCollection<PlaylistWriter.PlaylistColumn>();

            // Initialize Commands
            SaveCommand = new RelayCommand<Window>(Save);
            CancelCommand = new RelayCommand<Window>(Cancel);
            TakeCommand = new RelayCommand(Take);
            ReturnCommand = new RelayCommand(Return);
            TakeAllCommand = new RelayCommand(TakeAll);
            ReturnAllCommand = new RelayCommand(ReturnAll);
        }

        // Properties //
        public FormatColumnsResponse Response
        {
            get => new FormatColumnsResponse
            {
                Columns = new List<PlaylistWriter.PlaylistColumn>(OrderedColumnItems),
                DialogResult = this.dialogResult
            };
        }
        public bool TakeEnabled { get; set; }
        public bool ReturnEnabled { get; set; }
        public ObservableCollection<PlaylistWriter.PlaylistColumn> TotalColumnItems
        {
            get => _remainingColumns;
            set
            {
                _remainingColumns = value;
                RaisePropertyChanged(() => TotalColumnItems);
            }
        }
        public ObservableCollection<PlaylistWriter.PlaylistColumn> OrderedColumnItems
        {
            get => _takenColumns;
            set
            {
                _takenColumns = value;
                RaisePropertyChanged(() => OrderedColumnItems);
            }
        }
        public int TotalColumnIndex
        {
            get => _totalColumnIndex;
            set
            {
                _totalColumnIndex = value;
                RaisePropertyChanged(() => TotalColumnIndex);
            }
        }
        public int OrderedColumnIndex
        {
            get => _orderedColumnIndex;
            set
            {
                _orderedColumnIndex = value;
                RaisePropertyChanged(() => OrderedColumnIndex);
            }
        }

        // Commands //
        public RelayCommand<Window> SaveCommand { get; private set; }
        public RelayCommand<Window> CancelCommand { get; private set; }
        public RelayCommand TakeCommand { get; private set; }
        public RelayCommand TakeAllCommand { get; private set; }
        public RelayCommand ReturnCommand { get; private set; }
        public RelayCommand ReturnAllCommand { get; private set; }

        // Methods //
        private void Save(Window window)
        {
            dialogResult = DialogResultAction.Submit;

            if (window != null)
                window.Close();
        }

        private void Cancel(Window window)
        {
            dialogResult = DialogResultAction.Cancel;

            if (window != null)
                window.Close();
        }

        private void TakeAll()
        {
            if (TotalColumnItems == null)
                return;

            foreach (var item in TotalColumnItems)
            {
                OrderedColumnItems.Add(item);
            }
            TotalColumnItems.Clear();

            RaiseListViewPropertiesChanged();
        }

        private void Take()
        {
            if (TotalColumnItems == null)
                return;

            if (TotalColumnIndex < 0 || TotalColumnIndex >= TotalColumnItems.Count)
                return;
            
            OrderedColumnItems.Add(TotalColumnItems[TotalColumnIndex]);
            int savedIndex = TotalColumnIndex;
            TotalColumnItems.Remove(TotalColumnItems[TotalColumnIndex]);
            TotalColumnIndex = savedIndex >= TotalColumnItems.Count ? TotalColumnItems.Count - 1 : savedIndex;

            RaiseListViewPropertiesChanged();
        }

        private void Return()
        {
            if (OrderedColumnItems == null)
                return;

            if (OrderedColumnIndex < 0 || OrderedColumnIndex >= OrderedColumnItems.Count)
                return;

            TotalColumnItems.Add(OrderedColumnItems[OrderedColumnIndex]);
            int savedIndex = OrderedColumnIndex;
            OrderedColumnItems.Remove(OrderedColumnItems[OrderedColumnIndex]);
            OrderedColumnIndex = savedIndex >= OrderedColumnItems.Count ? OrderedColumnItems.Count - 1 : savedIndex;

            RaiseListViewPropertiesChanged();
        }

        private void ReturnAll()
        {
            if (OrderedColumnItems == null)
                return;

            foreach (var item in OrderedColumnItems)
            {
                TotalColumnItems.Add(item);
            }
            OrderedColumnItems.Clear();

            RaiseListViewPropertiesChanged();
        }

        //private void PopulateListView()
        //{
        //    if (TotalColumnItems == null || OrderedColumnItems == null)
        //        return;

        //    foreach (PlaylistWriter.PlaylistColumn col in TotalColumnItems)
        //    {
        //        TotalColumnItems.Add(col);
        //    }

        //    RaiseListViewPropertiesChanged();
        //}

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

        public void RaiseListViewPropertiesChanged()
        {
            RaisePropertyChanged(() => TotalColumnItems);
            RaisePropertyChanged(() => OrderedColumnItems);
        }
    }
}
