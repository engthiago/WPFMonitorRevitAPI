using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFMonitorProgress.Events
{
    class ProcessEventHandler : IExternalEventHandler
    {
        public Views.ProgressMonitorView CurrentUI { get; set; }
        public Controls.ProgressMonitorControl CurrentControl { get; set; }
        public Wall CurrentWall { get; set; }
        bool Cancel = false;

        private delegate void ProgressBarDelegate();

        public void Execute(UIApplication app)
        {
            if (app == null)
            {
                CloseWindow();
                return;
            }

            if (app.ActiveUIDocument == null)
                return;

            if (app.ActiveUIDocument.Document == null)
                return;

            if (CurrentWall == null)
                return;

            if (CurrentUI == null)
                return;

            if (CurrentControl == null)
                return;

            Parameter parameter = CurrentWall.get_Parameter(BuiltInParameter.DOOR_NUMBER);
            if (parameter.IsReadOnly)
            {
                CloseWindow();
                return;
            }

            CurrentUI.btnCancel.Click += CurrentUI_Closed;

            using (Transaction t = new Transaction(CurrentWall.Document, "Process"))
            {
                t.Start();
                for (CurrentControl.CurrentValue = 0; CurrentControl.CurrentValue <= CurrentControl.MaxValue; ++CurrentControl.CurrentValue)
                {
                    if (Cancel)
                        break;

                    System.Threading.Thread.Sleep(50);

                    try
                    {
                        parameter.Set(CurrentControl.CurrentValue.ToString());
                    }
                    catch
                    {
                        CloseWindow();
                        return;
                    }

                    CurrentControl.CurrentContext = string.Format("progress {0} of {1} done", CurrentControl.CurrentValue, CurrentControl.MaxValue);
                    CurrentUI.Dispatcher.Invoke(new ProgressBarDelegate(CurrentControl.NotifyUI), System.Windows.Threading.DispatcherPriority.Background);

                }
                t.Commit();

                CloseWindow();
            }

        }

        private void CloseWindow()
        {
            CurrentUI.Closed -= CurrentUI_Closed;
            CurrentUI.Close();
        }

        private void CurrentUI_Closed(object sender, EventArgs e)
        {
            Cancel = true;
        }

        public string GetName()
        {
            return "ProgressMonitor";
        }
    }
}
