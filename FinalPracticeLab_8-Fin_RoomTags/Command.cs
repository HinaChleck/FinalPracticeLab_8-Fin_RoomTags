using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalPracticeLab_8_Fin_RoomTags
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand

    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            FamilySymbol familySymbol = new FilteredElementCollector(doc)//ищем семейство нужных марок в проекте
                    .OfClass(typeof(FamilySymbol))
                    .OfCategory(BuiltInCategory.OST_RoomTags)//Категоря марок верная, т.к. копировала стандартное семейство
                    .OfType<FamilySymbol>()
                    .Where(x => x.Name.Equals("Марка помещения Этаж - помещение"))
                    .Where(x => x.FamilyName.Equals("M_Марка помещения_Этаж_Номер_Учеба"))
                    .FirstOrDefault();

            if (familySymbol == null)//проверка, заггружено ли семейство отверстий в проект
            {
                TaskDialog.Show("Ошибка", "Не найдено семейство марок. Загрузите семейство в проект");
                return Result.Cancelled;
            }

            List<Level> listLevel = GetAssociatedLevels.ToViewPlans(doc);//отбор только тех уровней, у которых есть ассоциированнные виды. Без этого выдает ошибку, если не все уровни в проекте ассоциированы

            List<List<ElementId>> allRooms = new List<List<ElementId>>();

            Transaction transaction = new Transaction(doc);
            transaction.Start("Построение помещений");

            foreach (Level level in listLevel)
            {
                ICollection<ElementId> roomsIdsOnLevel = doc.Create.NewRooms2(level);

                allRooms.Add(roomsIdsOnLevel as List<ElementId>);

                foreach (ElementId roomId in roomsIdsOnLevel)
                {
                    Room room = doc.GetElement(roomId) as Room;
                    XYZ center = new XYZ(0, 0, 0);
                    FamilyInstance roomTag = doc.Create.NewFamilyInstance(center, familySymbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);

                }
                for (int i = 0; i < allRooms.Count; i++)
                {
                    for (int j = 0; j < allRooms[i].Count; j++)
                    {

                       (doc.GetElement(allRooms[i][j]) as Room).Name = $"{i + 1}-{j + 1}";// Вариант, когда нумерация начинается на этаже заново
                        //(doc.GetElement(allRooms[i][j]) as Room).Name = $"{i + 1}-{(doc.GetElement(allRooms[i][j]) as Room).Number}";//Вариант со сквозной нумерацией по всему проекту

                    }
                }
            }
            transaction.Commit();
            return Result.Succeeded;
        }

      

    }
}
