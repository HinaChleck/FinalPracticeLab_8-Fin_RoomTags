using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalPracticeLab_8_Fin_RoomTags
{
    public class GetAssociatedLevels
    {
        public static List<Level> ToViewPlans(Document doc)
        {
            List<Level> ViewAndLevel = new List<Level>();
            List<Level> levelCollector = new FilteredElementCollector(doc)//находит элементы - уровни
           .WhereElementIsNotElementType()
           .OfClass(typeof(Level))
           .Cast<Level>()
           .OfType<Level>()
           .ToList();

            List<ViewPlan> viewCollector = new FilteredElementCollector(doc)//находит элементы - виды, у которых GenLevel не null (иначе исключениев цикле foreach)
           .WhereElementIsNotElementType()
           .OfClass(typeof(ViewPlan))
           .Cast<ViewPlan>()
           .OfType<ViewPlan>()
           .Where(x => x.GenLevel != null)
           .ToList();


            foreach (Level level in levelCollector)

            {
                foreach (ViewPlan view in viewCollector)
                {
                    if (view.GenLevel.Id == level.Id)
                    {
                        ViewAndLevel.Add(level as Level);
                        break;
                    }
                }
            }

            return ViewAndLevel;
        }
    }
}
