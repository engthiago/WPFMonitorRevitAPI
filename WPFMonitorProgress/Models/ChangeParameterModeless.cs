using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFMonitorProgress.Models
{
    class ChangeParameterModeless
    {
        Wall CurrentWall { get; set; }
        ExternalEvent ExternalEvent { get; set; }

        public ChangeParameterModeless(Element wallElement)
        {
            CurrentWall = wallElement as Wall;
        }

        internal void ProgressModeless()
        {

            if (CurrentWall == null)
                throw new Exception("Selected Element is not a wall");

            Views.ProgressMonitorView currentUI = new Views.ProgressMonitorView();

            Events.ProcessEventHandler progressEventHandler = new Events.ProcessEventHandler();
            progressEventHandler.CurrentWall = CurrentWall;
            progressEventHandler.CurrentUI = currentUI;
            Controls.ProgressMonitorControl currentControl = new Controls.ProgressMonitorControl();
            currentControl.MaxValue = 200;
            progressEventHandler.CurrentControl = currentControl;

            ExternalEvent = ExternalEvent.Create(progressEventHandler);

            currentUI.DataContext = currentControl;
            currentUI.ContentRendered += CurrentUI_ContentRendered;
            currentUI.Topmost = true;
            currentUI.Show();
        }

        private void CurrentUI_ContentRendered(object sender, EventArgs e)
        {
            ExternalEvent.Raise();
        }
    }
}

