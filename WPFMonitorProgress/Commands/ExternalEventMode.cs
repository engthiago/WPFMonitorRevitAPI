using Autodesk.Revit.UI;
using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;

namespace WPFMonitorProgress.Commands
{
    [Transaction(TransactionMode.Manual)]
    class ExternalEventMode : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Selection sel = uidoc.Selection;

            try
            {
                Reference refer = sel.PickObject(ObjectType.Element, "Select a Wall");
                Models.ChangeParameterModeless model = new Models.ChangeParameterModeless(uidoc.Document.GetElement(refer));

                model.ProgressModeless();
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
