using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;

namespace WPFMonitorProgress.Commands
{
    [Transaction(TransactionMode.Manual)]
    class ShareMainThreadMode : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Selection sel = uidoc.Selection;

            try
            {
                Reference refer = sel.PickObject(ObjectType.Element, "Select a Wall");
                Models.ChangeParameter model = new Models.ChangeParameter(uidoc.Document.GetElement(refer));
                model.ProgressModal();
            }
            catch (Exception e)
            {
                if (!(e is Autodesk.Revit.Exceptions.OperationCanceledException))
                {
                    message = e.Message;
                    return Result.Failed;
                }
            }

            return Result.Succeeded;
        }
    }
}
