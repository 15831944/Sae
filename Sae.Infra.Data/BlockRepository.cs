using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Sae.Domain.Repository;
using System.Collections.Generic;
using Autodesk.AutoCAD.Colors;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Sae.Infra.Data
{
    public class BlockRepository<T, TE> : IBlockRepository<T, TE> where T : Entity where TE : AttributeDefinition
    {
        public ObjectId CreatingABlock(string name, ICollection<T> entities, ICollection<TE> attributes, Point3d? origin = null)
        {
            using (var database = Application.DocumentManager.MdiActiveDocument.Database)
            {
                using (var transaction = database.TransactionManager.StartTransaction())
                {
                    try
                    {
                        var objectId = ObjectId.Null;
                        var acBlkTbl = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;

                        if (acBlkTbl != null && !acBlkTbl.Has(name))
                        {
                            using (var acBlkTblRec = new BlockTableRecord())
                            {
                                acBlkTblRec.Name = name;
                                acBlkTblRec.Origin = origin ?? new Point3d(0, 0, 0);

                                foreach (var entity in entities)
                                {
                                    acBlkTblRec.AppendEntity(entity);
                                }

                                if (attributes != null)
                                {
                                    foreach (var attribute in attributes)
                                    {
                                        acBlkTblRec.AppendEntity(attribute);
                                    }
                                }

                                transaction.GetObject(database.BlockTableId, OpenMode.ForWrite);
                                acBlkTbl.Add(acBlkTblRec);
                                transaction.AddNewlyCreatedDBObject(acBlkTblRec, true);

                                foreach (var entity in entities)
                                {
                                    entity.Dispose();
                                }

                                if (attributes != null)
                                {
                                    foreach (var attribute in attributes)
                                    {
                                        attribute.Dispose();
                                    }
                                }

                                objectId = acBlkTblRec.ObjectId;
                            }
                        }

                        transaction.Commit();
                        return objectId;
                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception)
                    {
                        transaction.Abort();
                        throw;
                    }
                }
            }
        }

        public void InsertBlock(string nome, Point3d pontoInsercao, double? rotacao = 0.0, double? altura = 2.80)
        {
            if (string.IsNullOrEmpty(nome) || string.IsNullOrWhiteSpace(nome)) throw new ArgumentNullException($"É necessário informar o nome do bloco");
            if (!rotacao.HasValue) throw new ArgumentNullException($"É necessário informar o ângulo de rotação para a inserção do bloco");
            if (!altura.HasValue) throw new ArgumentNullException($"É necessário informar a altura da parede");

            var alt = (int)(altura.Value * 100);
            var count = (int)Math.Ceiling((alt * 1.0) / 20);

            using (var database = Application.DocumentManager.MdiActiveDocument.Database)
            {
                using (var transaction = database.TransactionManager.StartTransaction())
                {
                    try
                    {
                        var acBlkTbl = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
                        if (acBlkTbl != null && acBlkTbl.Has(nome))
                        {
                            var blkRecId = acBlkTbl[nome];
                            if (blkRecId != ObjectId.Null)
                            {
                                using (var acBlkRef = new BlockReference(pontoInsercao, blkRecId))
                                {
                                    acBlkRef.Rotation = rotacao.Value;
                                    var blkTblRec = transaction.GetObject(blkRecId, OpenMode.ForRead) as BlockTableRecord;
                                    var acCurSpaceBlkTblRec = transaction.GetObject(database.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                                    if (acCurSpaceBlkTblRec != null) acCurSpaceBlkTblRec.AppendEntity(acBlkRef);
                                    transaction.AddNewlyCreatedDBObject(acBlkRef, true);
                                    if (blkTblRec != null && blkTblRec.HasAttributeDefinitions)
                                    {
                                        var lyRepository = new LayerRepository();
                                        var lyObjectId = ObjectId.Null;
                                        if (nome.ToLower().Contains("be") && !nome.ToLower().Contains("am"))
                                        {
                                            lyObjectId = lyRepository.GetLayer("BE");
                                            if (lyObjectId == ObjectId.Null)
                                            {
                                                lyObjectId = lyRepository.CreateALayer("BE", ColorMethod.ByAci, 30, LineWeight.LineWeight030);
                                            }
                                        }
                                        else if (nome.ToLower().Contains("be") && nome.ToLower().Contains("am"))
                                        {
                                            lyObjectId = lyRepository.GetLayer("BE-AM");
                                            if (lyObjectId == ObjectId.Null)
                                            {
                                                lyObjectId = lyRepository.CreateALayer("BE-AM", ColorMethod.ByAci, 101, LineWeight.LineWeight030);
                                            }
                                        }

                                        if (acBlkRef is Entity ent)
                                        {
                                            ent.UpgradeOpen();
                                            ent.LayerId = lyObjectId;
                                        }

                                        foreach (var objId in blkTblRec)
                                        {
                                            var dbObj = transaction.GetObject(objId, OpenMode.ForRead);
                                            if (dbObj is AttributeDefinition acAtt)
                                            {
                                                if (!acAtt.Constant)
                                                {
                                                    if (acAtt.Tag.ToUpper().Equals("AP"))
                                                    {
                                                        using (var acAttRef = new AttributeReference())
                                                        {
                                                            acAttRef.SetAttributeFromBlock(acAtt, acBlkRef.BlockTransform);
                                                            acAttRef.Position = acAtt.Position.TransformBy(acBlkRef.BlockTransform);
                                                            acAttRef.TextString = altura.ToString();
                                                            acBlkRef.AttributeCollection.AppendAttribute(acAttRef);
                                                            transaction.AddNewlyCreatedDBObject(acAttRef, true);
                                                        }
                                                    }
                                                    else if (acAtt.Tag.ToUpper().Equals("Q"))
                                                    {
                                                        using (var acAttRef = new AttributeReference())
                                                        {
                                                            acAttRef.SetAttributeFromBlock(acAtt, acBlkRef.BlockTransform);
                                                            acAttRef.Position = acAtt.Position.TransformBy(acBlkRef.BlockTransform);
                                                            acAttRef.TextString = count.ToString();
                                                            acBlkRef.AttributeCollection.AppendAttribute(acAttRef);
                                                            transaction.AddNewlyCreatedDBObject(acAttRef, true);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        using (var acAttRef = new AttributeReference())
                                                        {
                                                            acAttRef.SetAttributeFromBlock(acAtt, acBlkRef.BlockTransform);
                                                            acAttRef.Position = acAtt.Position.TransformBy(acBlkRef.BlockTransform);
                                                            acAttRef.TextString = acAtt.TextString;
                                                            acBlkRef.AttributeCollection.AppendAttribute(acAttRef);
                                                            transaction.AddNewlyCreatedDBObject(acAttRef, true);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
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
        }

        public ObjectId Get(string nome)
        {
            using (var database = Application.DocumentManager.MdiActiveDocument.Database)
            {
                using (var transaction = database.TransactionManager.StartTransaction())
                {
                    var acBlkTbl = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
                    var blkRecId = ObjectId.Null;
                    if (acBlkTbl != null && acBlkTbl.Has(nome))
                    {
                        blkRecId = acBlkTbl[nome];
                    }
                    return blkRecId;
                }
            }
        }
    }
}