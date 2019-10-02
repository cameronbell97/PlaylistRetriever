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
        private int _remainingColumnsIndex;
        private int _takenColumnsIndex;

        private DialogResultAction dialogResult;

        // Constructors //
        public FormatColumnsDialogViewModel()
        {
            // Initialize Declarations
            TakeEnabled = true;
            ReturnEnabled = false;
            RemainingColumns = new ObservableCollection<PlaylistWriter.PlaylistColumn>
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
            TakenColumns = new ObservableCollection<PlaylistWriter.PlaylistColumn>();

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
                Columns = new List<PlaylistWriter.PlaylistColumn>(TakenColumns),
                DialogResult = this.dialogResult
            };
        }
        public bool TakeEnabled { get; set; }
        public bool ReturnEnabled { get; set; }
        public ObservableCollection<PlaylistWriter.PlaylistColumn> RemainingColumns
        {
            get => _remainingColumns;
            set
            {
                _remainingColumns = value;
                RaisePropertyChanged(() => RemainingColumns);
            }
        }
        public ObservableCollection<PlaylistWriter.PlaylistColumn> TakenColumns
        {
            get => _takenColumns;
            set
            {
                _takenColumns = value;
                RaisePropertyChanged(() => TakenColumns);
            }
        }
        public int RemainingColumnsIndex
        {
            get => _remainingColumnsIndex;
            set
            {
                _remainingColumnsIndex = value;
                RaisePropertyChanged(() => RemainingColumnsIndex);
            }
        }
        public int TakenColumnsIndex
        {
            get => _takenColumnsIndex;
            set
            {
                _takenColumnsIndex = value;
                RaisePropertyChanged(() => TakenColumnsIndex);
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
            if (RemainingColumns == null)
                return;

            foreach (var item in RemainingColumns)
            {
                TakenColumns.Add(item);
            }
            RemainingColumns.Clear();

            RaiseListViewPropertiesChanged();
        }

        private void Take()
        {
            if (RemainingColumns == null)
                return;

            if (RemainingColumnsIndex < 0 || RemainingColumnsIndex >= RemainingColumns.Count)
                return;
            
            TakenColumns.Add(RemainingColumns[RemainingColumnsIndex]);
            int savedIndex = RemainingColumnsIndex;
            RemainingColumns.Remove(RemainingColumns[RemainingColumnsIndex]);
            RemainingColumnsIndex = savedIndex >= RemainingColumns.Count ? RemainingColumns.Count - 1 : savedIndex;

            RaiseListViewPropertiesChanged();
        }

        private void Return()
        {
            if (TakenColumns == null)
                return;

            if (TakenColumnsIndex < 0 || TakenColumnsIndex >= TakenColumns.Count)
                return;

            RemainingColumns.Add(TakenColumns[TakenColumnsIndex]);
            int savedIndex = TakenColumnsIndex;
            TakenColumns.Remove(TakenColumns[TakenColumnsIndex]);
            TakenColumnsIndex = savedIndex >= TakenColumns.Count ? TakenColumns.Count - 1 : savedIndex;

            RaiseListViewPropertiesChanged();
        }

        private void ReturnAll()
        {
            if (TakenColumns == null)
                return;

            foreach (var item in TakenColumns)
            {
                RemainingColumns.Add(item);
            }
            TakenColumns.Clear();

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
            RaisePropertyChanged(() => RemainingColumns);
            RaisePropertyChanged(() => TakenColumns);
        }
    }
}
