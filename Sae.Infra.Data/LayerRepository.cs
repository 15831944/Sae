using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Sae.Domain.Repository;

namespace Sae.Infra.Data
{
    public class LayerRepository : ILayerRepository
    {
        public ObjectId CreateALayer(string name, ColorMethod method, short indexColor, LineWeight? lineWeight, string description = null)
        {
            var objectId = ObjectId.Null;
            using (var database = Application.DocumentManager.MdiActiveDocument.Database)
            {
                using (var transaction = database.TransactionManager.StartTransaction())
                {
                    try
                    {
                        var lyTbl = transaction.GetObject(database.LayerTableId, OpenMode.ForRead) as LayerTable;
                        var sLayerName = name;

                        if (lyTbl != null && !lyTbl.Has(sLayerName))
                        {
                            using (var lyTblRec = new LayerTableRecord())
                            {
                                lyTblRec.Color = Color.FromColorIndex(method, indexColor);
                                lyTblRec.Name = sLayerName;
                                lyTblRec.LineWeight = lineWeight ?? LineWeight.LineWeight000;
                                lyTblRec.Description = description ?? "";

                                transaction.GetObject(database.LayerTableId, OpenMode.ForWrite);

                                lyTbl.Add(lyTblRec);
                                transaction.AddNewlyCreatedDBObject(lyTblRec, true);
                                objectId = lyTblRec.ObjectId;
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception)
                    {
                        transaction.Abort();
                        throw;
                    }
                }
            }

            return objectId;
        }

        public ObjectId GetLayer(string nome)
        {
            var objectId = ObjectId.Null;
            using (var database = Application.DocumentManager.MdiActiveDocument.Database)
            {
                using (var transaction = database.TransactionManager.StartTransaction())
                {
                    try
                    {
                        var acLyrTbl = transaction.GetObject(database.LayerTableId, OpenMode.ForRead) as LayerTable;
                        var sLayerName = nome;

                        if (acLyrTbl != null && acLyrTbl.Has(sLayerName))
                        {
                            objectId = acLyrTbl[sLayerName];
                        }
                        transaction.Commit();
                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception)
                    {
                        transaction.Abort();
                        throw;
                    }
                }
            }

            return objectId;
        }
    }
}