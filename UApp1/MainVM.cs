using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace UApp1
{
    class MainVM : ViewModelBase
    {
        public MainVM()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);

            _timer.Tick += T_Tick;
        }

        private void T_Tick(object sender, object e)
        {
            _currTs = _currTs.Subtract(TimeSpan.FromSeconds(1));
            currSecs += 1;

            RefreshData();

            if (_currTs.TotalSeconds == 0)
            {
                if (currSecs < totalSecs)
                {
                    _currTs = TimeSpan.FromMinutes(currRestLength);
                    OnShowNotif(null);
                }
                else
                {
                    // End
                    OnShowNotif(null);

                    _timer.Stop();
                    RefreshCmds();
                }
            }
        }

        public void RefreshSettings()
        {
            var appSettings = new SettingVM();

            currWorkLength = appSettings.WorkMins;
            currRestLength = appSettings.RestMins;

            totalSecs = (currWorkLength + currRestLength) * 60;

            SepAngle = (double)currWorkLength / (currWorkLength + currRestLength) * 360;

            ResetData();
            RefreshData();
        }

        int currWorkLength = 0;
        int currRestLength = 0;
        int totalSecs = 0;
        int currSecs = 0;

        public event EventHandler ShowNotif;
        protected virtual void OnShowNotif(EventArgs e)
        {
            ShowNotif?.Invoke(this, e);
        }

        DispatcherTimer _timer;

        TimeSpan _currTs;

        string _min;
        public string Min
        {
            get { return _currTs.Minutes.ToString("D2"); }
            set
            {
                _min = value;
                RaisePropertyChanged();
            }
        }

        string _sec;
        public string Sec
        {
            get { return _sec; }
            set
            {
                _sec = value;
                RaisePropertyChanged();
            }
        }

        private double _sepAngle;

        public double SepAngle
        {
            get { return _sepAngle; }
            set
            {
                _sepAngle = value;
                RaisePropertyChanged();
            }
        }

        private double _doneAngle;

        public double DoneAngle
        {
            get { return _doneAngle; }
            set
            {
                _doneAngle = value;
                RaisePropertyChanged();
            }
        }

        private double _remainingWorkAngle;

        public double RemainingWorkAngle
        {
            get { return _remainingWorkAngle; }
            set
            {
                _remainingWorkAngle = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _startCmd;
        public RelayCommand StartCmd
        {
            get
            {
                if (_startCmd == null)
                {
                    _startCmd = new RelayCommand(() =>
                    {
                        if (currSecs == totalSecs)
                        {
                            // New
                            ResetData();

                            RefreshData();
                        }

                        _timer.Start();
                        RefreshCmds();
                    }, () =>
                    {
                        return !_timer.IsEnabled;
                    });
                }
                return _startCmd;
            }
        }

        private RelayCommand _pauseCmd;
        public RelayCommand PauseCmd
        {
            get {
                if (_pauseCmd == null)
                {
                    _pauseCmd = new RelayCommand(() =>
                    {
                        _timer.Stop();

                        RefreshCmds();
                    }, () =>
                    {
                        return _timer.IsEnabled;
                    });
                }

                return _pauseCmd;
            }
        }

        RelayCommand _stopCmd;
        public RelayCommand StopCmd
        {
            get
            {
                if (_stopCmd == null)
                {
                    _stopCmd = new RelayCommand(() =>
                    {
                        _timer.Stop();

                        ResetData();
                        RefreshData();

                        RefreshCmds();
                    }, () =>
                    {
                        return _timer.IsEnabled;
                    });
                }

                return _stopCmd;
            }
        }

        private void RefreshData()
        {
            Min = _currTs.Minutes.ToString("D2");
            Sec = _currTs.Seconds.ToString("D2");

            DoneAngle = ((double)currSecs / totalSecs) * 360.0;
            RemainingWorkAngle = SepAngle > DoneAngle ? SepAngle - DoneAngle : 0.0;
        }

        private void RefreshCmds()
        {
            _startCmd?.RaiseCanExecuteChanged();
            _pauseCmd?.RaiseCanExecuteChanged();
            _stopCmd?.RaiseCanExecuteChanged();
        }

        private void ResetData()
        {
            currSecs = 0;
            _currTs = TimeSpan.FromMinutes(currWorkLength);
        }
    }
}
