using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace UApp1
{
    public class SettingVM : ViewModelBase
    {
        private readonly ApplicationDataContainer _appSettings;

        public SettingVM()
        {
            _appSettings = ApplicationData.Current.RoamingSettings;
        }

        protected T GetOrDefault<T>(T defaultValue, [CallerMemberName] string propertyName = null)
        {
            if (_appSettings.Values.ContainsKey(propertyName))
                return (T)_appSettings.Values[propertyName];

            return defaultValue;
        }

        protected bool Set<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (_appSettings.Values.ContainsKey(propertyName))
            {
                var currentValue = (T)_appSettings.Values[propertyName];
                if (EqualityComparer<T>.Default.Equals(currentValue, value))
                    return false;
            }

            _appSettings.Values[propertyName] = value;
            return true;
        }

        public int WorkMins
        {
            get
            {
                return GetOrDefault(1);
            }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public int RestMins
        {
            get { return GetOrDefault(1); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

    }
}
