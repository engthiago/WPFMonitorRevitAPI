using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFMonitorProgress.Controls
{
    class ProgressMonitorControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public double MaxValue { get; set; }
        public double CurrentValue { get; set; }
        public string CurrentContext { get; set; }

        public ProgressMonitorControl()
        {
            MaxValue = 100;
            CurrentValue = 0;
            CurrentContext = string.Empty;
        }

        public void NotifyUI()
        {
            Type classType = this.GetType();
            if (classType != null)
            {
                System.Reflection.PropertyInfo[] currentProperties = classType.GetProperties();
                foreach (System.Reflection.PropertyInfo currentProperty in currentProperties)
                    OnPropertyChanged(currentProperty.Name);
            }
        }

        private void OnPropertyChanged(string targetProperty)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(targetProperty));
            }
        }

    }
}
