using Autodesk.AutoCAD.DatabaseServices;
using Sae.Domain.Repository;

namespace Sae.Infra.Data
{
    public class TextRepository : ITextRepository
    {
        private readonly Database _db;

        public TextRepository()
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            _db = doc.Database;
        }

        public ObjectId GetTextStyleByName(string name = "Legend")
        {
            var textStyleName = name;
            var textStyleId = ObjectId.Null;

            using (var acTrans = _db.TransactionManager.StartTransaction())
            {
                try
                {
                    var textStyleTable = acTrans.GetObject(_db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                    if (textStyleTable == null || !textStyleTable.Has(textStyleName)) return textStyleId;
                    textStyleId = textStyleTable[textStyleName];
                }
                catch (Autodesk.AutoCAD.Runtime.Exception)
                {
                    acTrans.Abort();
                    throw;
                }
            }

            return textStyleId;
        }
    }
}