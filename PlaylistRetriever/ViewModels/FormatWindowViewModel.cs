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
    public class FormatWindowViewModel : ViewModelBase
    {
        // Declarations //
        private ObservableCollection<PlaylistWriter.PlaylistColumn> _remainingColumns;
        private ObservableCollection<PlaylistWriter.PlaylistColumn> _takenColumns;
        private int _totalColumnIndex;
        private int _orderedColumnIndex;

        // Constructors //
        public FormatWindowViewModel()
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
            ReturningColumns = new List<PlaylistWriter.PlaylistColumn>();
            PopulateListView();

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
                Columns = ReturningColumns
            };
        }
        public bool TakeEnabled { get; set; }
        public bool ReturnEnabled { get; set; }
        public List<PlaylistWriter.PlaylistColumn> ReturningColumns { get; private set; }
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
            foreach (var sItem in OrderedColumnItems)
                ReturningColumns.Add(sItem);

            if (window != null)
                window.Close();
        }

        private void Cancel(Window window)
        {
            ReturningColumns = null;

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

            int selectIndex = TotalColumnItems.SelectedIndex;
            if (selectIndex < 0 || selectIndex >= TotalColumnItems.Items.Count)
                return;

            OrderedColumnItems.Items.Add(TotalColumnItems.SelectedItem);
            TotalColumnItems.Items.Remove(TotalColumnItems.SelectedItem);

            TotalColumnItems.SelectedIndex = TotalColumnItems.Items.IsEmpty ? -1 : (TotalColumnItems.Items.Count > selectIndex) ? selectIndex : selectIndex - 1;

            RaiseListViewPropertiesChanged();
        }

        private void Return()
        {
            if (OrderedColumnItems == null)
                return;

            int selectIndex = OrderedColumnItems.SelectedIndex;
            if (selectIndex < 0 || selectIndex >= OrderedColumnItems.Items.Count)
                return;

            TotalColumnItems.Items.Add(OrderedColumnItems.SelectedItem);
            OrderedColumnItems.Items.Remove(OrderedColumnItems.SelectedItem);

            OrderedColumnItems.SelectedIndex = OrderedColumnItems.Items.IsEmpty ? -1 : (OrderedColumnItems.Items.Count > selectIndex) ? selectIndex : selectIndex - 1;

            RaiseListViewPropertiesChanged();
        }

        private void ReturnAll()
        {
            if (OrderedColumnItems == null)
                return;

            foreach (string item in OrderedColumnItems.Items)
            {
                TotalColumnItems.Items.Add(item);
            }
            OrderedColumnItems.Items.Clear();

            RaiseListViewPropertiesChanged();
        }

        private void PopulateListView()
        {
            if (_remainingColumns == null || TotalColumnItems == null || OrderedColumnItems == null)
                return;

            foreach (PlaylistWriter.PlaylistColumn col in _remainingColumns)
            {
                TotalColumnItems.Items.Add(col.ToString());
            }

            RaiseListViewPropertiesChanged();
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

        public void RaiseListViewPropertiesChanged()
        {
            RaisePropertyChanged(() => TotalColumnItems);
            RaisePropertyChanged(() => OrderedColumnItems);
        }
    }
}
