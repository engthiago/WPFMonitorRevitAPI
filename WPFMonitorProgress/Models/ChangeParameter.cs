using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFMonitorProgress.Models
{

    class ChangeParameter
    {
        Wall CurrentWall { get; set; }
        Controls.ProgressMonitorControl CurrentControl { get; set; }
        Views.ProgressMonitorView CurrentUI { get; set; }
        bool Cancel { get; set; }
        private delegate void ProgressBarDelegate();

        ExternalEvent ExternalEvent { get; set; }

        public ChangeParameter(Element wallElement)
        {
            CurrentWall = wallElement as Wall;
        }

        public void ProgressModal()
        {
            if (CurrentWall == null)
                throw new Exception("Selected Element is not a wall");

            CurrentControl = new Controls.ProgressMonitorControl();
            CurrentControl.MaxValue = 100;
            CurrentUI = new Views.ProgressMonitorView();
            CurrentUI.DataContext = CurrentControl;
            CurrentUI.Closed += CurrentUI_Closed;
            CurrentUI.ContentRendered += FireUPModal;

            CurrentUI.ShowDialog();
        }
        
        private void FireUPModal(object sender, EventArgs e)
        {
            CurrentUI.ContentRendered -= FireUPModal;
            Parameter parameter = CurrentWall.get_Parameter(BuiltInParameter.DOOR_NUMBER);
            if (parameter.IsReadOnly)
            {
                CloseWindow();
                throw new Exception("Mark parameter is read only");
            }

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
                        throw new Exception("Error trying to set Mark paramter");
                    }

                    CurrentControl.CurrentContext = string.Format("progress {0} of {1} done", CurrentControl.CurrentValue, CurrentControl.MaxValue);
                    CurrentUI.Dispatcher.Invoke(new ProgressBarDelegate(CurrentControl.NotifyUI), System.Windows.Threading.DispatcherPriority.Background);

                }
                t.Commit();

            }

            CloseWindow();
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
    }
}
