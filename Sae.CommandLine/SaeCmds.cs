using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Sae.Domain.Enum;
using Sae.Infra.Data;
using Sae.Service;
using System;
using System.Linq;
using Autodesk.AutoCAD.Colors;

namespace Sae.CommandLine
{
    public class SaeCmds : IExtensionApplication
    {
        private readonly Editor _editor;

        public SaeCmds()
        {
            _editor = Application.DocumentManager.MdiActiveDocument.Editor;
        }

        public void Initialize()
        {
            var lyRepository = new LayerRepository();
            if (lyRepository.GetLayer("BE") == ObjectId.Null)
            {
                lyRepository.CreateALayer("BE", ColorMethod.ByAci, 30, LineWeight.LineWeight030);
            }

            if (lyRepository.GetLayer("") == ObjectId.Null)
            {
                lyRepository.CreateALayer("BE-AM", ColorMethod.ByAci, 101, LineWeight.LineWeight030);
            }
            _editor.WriteMessage("SAE (Sistema de Alvenaria Estrutural) carregado.");
        }

        public void Terminate()
        {
            Console.WriteLine("Cleaning up...");
        }

        [CommandMethod("IBAE", CommandFlags.UsePickSet | CommandFlags.Redraw | CommandFlags.Modal)]
        public void CreateABlock()
        {
            try
            {
                var pdo = new PromptDoubleOptions("Informe a altura da parede: ")
                {
                    AllowNegative = false,
                    AllowNone = false,
                    AllowZero = false,
                    DefaultValue = 2.80
                };

                PromptDoubleResult pdr;

                do
                {
                    pdr = _editor.GetDouble(pdo);

                    if (pdr.Status == PromptStatus.Cancel) return;

                    if (pdr.Status == PromptStatus.OK)
                    {
                        var altura = pdr.Value;
                        var namesOfBlocks = Enum.GetNames(typeof(TipoBloco));
                        var pko = new PromptKeywordOptions("")
                        {
                            AllowNone = false,
                            Message = "Enter an option "
                        };

                        foreach (var name in namesOfBlocks)
                        {
                            pko.Keywords.Add(name);
                        }

                        var pr = _editor.GetKeywords(pko);

                        if (pr.Status == PromptStatus.OK)
                        {
                            var blk = new BlockRepository<Entity, AttributeDefinition>();
                            var tipBloc = (TipoBloco)Enum.Parse(typeof(TipoBloco), pr.StringResult, true);
                            using (var ces = new CustomEntityService())
                            {
                                var blockId = blk.Get(tipBloc.ToString());
                                if (blockId == ObjectId.Null)
                                {
                                    _editor.WriteMessage(
                                        $"\nBloco não existente.\nCriando bloco {tipBloc.ToString()}.");
                                    var (entities, attributes) = ces.GetEntitiesToBlock(tipBloc);
                                    blockId = blk.CreatingABlock(tipBloc.ToString(), entities, attributes,
                                        new Point3d(0, 0.07, 0));
                                }

                                if (blockId != ObjectId.Null)
                                {
                                    var pio = new PromptIntegerOptions("");
                                    pio.Keywords.Add("1 - Inserir um bloco");
                                    pio.Keywords.Add("2 - Inserir multiplos blocos");
                                    var op = _editor.GetInteger(pio);
                                    if(op.Status == PromptStatus.Cancel) return;
                                    if (op.Status == PromptStatus.OK)
                                    {
                                        PromptPointResult pPtRes;
                                        var pPtOpts = new PromptPointOptions("");
                                        switch (op.Value)
                                        {
                                            case 1:
                                                pPtOpts.Message = "\nInforme o ponto de inserção: ";
                                                pPtRes = _editor.GetPoint(pPtOpts);
                                                if (pPtRes.Status == PromptStatus.Cancel) return;
                                                blk.InsertBlock(tipBloc.ToString(), pPtRes.Value, 0, altura);
                                                break;

                                            case 2:
                                                pPtOpts.Message = "\nInforme o ponto inicial: ";
                                                pPtRes = _editor.GetPoint(pPtOpts);

                                                if (pPtRes.Status == PromptStatus.Cancel) return;
                                                var ptStart = pPtRes.Value;

                                                pPtOpts.Message = "\nInforme o ponto final: ";
                                                pPtOpts.UseBasePoint = true;
                                                pPtOpts.BasePoint = ptStart;
                                                pPtRes = _editor.GetPoint(pPtOpts);

                                                if (pPtRes.Status == PromptStatus.Cancel) return;

                                                var ptEnd = pPtRes.Value;
                                                var tamanho = CustomEntityService.GetDistanceBetweenTwoPoints(ptStart, ptEnd);
                                                var angulo = CustomEntityService.GetAngleBetweenTwoPoints(ptStart, ptEnd);
                                                var blkComprimento = (double.Parse(tipBloc.ToString()
                                                                                       .Split(new[] { "x" },
                                                                                           StringSplitOptions.RemoveEmptyEntries)
                                                                                       .LastOrDefault() ??
                                                                                   throw new InvalidOperationException(
                                                                                       "Operação de conversão inválida.")) + 1) /
                                                                     100;

                                                var dist = 0.0;
                                                do
                                                {
                                                    var pointInsert = CustomEntityService.GetPolarPoints(ptStart, angulo, dist);
                                                    blk.InsertBlock(tipBloc.ToString(), pointInsert, angulo, altura);
                                                    dist += blkComprimento;
                                                } while (dist < tamanho);
                                                break;
                                            default:
                                                _editor.WriteMessage("Opção inválida.");
                                                return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                } while (pdr.Status != PromptStatus.OK);
            }
            catch (ArgumentNullException ex)
            {
                _editor.WriteMessage(ex.Message);
            }
            catch (System.Exception ex)
            {
                _editor.WriteMessage(ex.Message);
            }
        }
    }
}