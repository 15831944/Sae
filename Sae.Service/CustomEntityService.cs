using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autofac;
using Sae.Domain.Enum;
using Sae.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Sae.Infra.IoC;

namespace Sae.Service
{
    public class CustomEntityService : IDisposable
    {
        private ICollection<AttributeDefinition> _atributos;
        private const double Radius = 0.0069;
        private readonly ITextRepository _textRepository;

        public CustomEntityService()
        {
            var container = NativeInjectorBootStrapper.Container();
            var scope = container.BeginLifetimeScope();
            _textRepository = scope.Resolve<ITextRepository>();
            _atributos = new List<AttributeDefinition>();
        }

        public Tuple<ICollection<Entity>, ICollection<AttributeDefinition>> GetEntitiesToBlock(TipoBloco tipo)
        {
            Tuple<ICollection<Entity>, ICollection<AttributeDefinition>> list;
            switch (tipo)
            {
                case TipoBloco.BE14x19x14:
                    list = GetBe14X19X14();
                    break;
                case TipoBloco.BE14x19x29:
                    list = GetBe14X19X29();
                    break;
                case TipoBloco.BE14x19x34:
                    list = GetBe14X19X34();
                    break;
                case TipoBloco.BE14x19x39:
                    list = GetBe14X19X39();
                    break;
                case TipoBloco.BEAM14x19x44:
                    list = GetBe14X19X44();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tipo), tipo, null);
            }
            return list;
        }

        private Tuple<ICollection<Entity>, ICollection<AttributeDefinition>> GetBe14X19X14()
        {
            var entitiesList = new List<Entity>();
            var acPolyExt = new Polyline();
            acPolyExt.AddVertexAt(0, new Point2d(0.1400, 0.0000), 0, 0, 0);
            acPolyExt.AddVertexAt(1, new Point2d(0.0000, 0.0000), 0, 0, 0);
            acPolyExt.AddVertexAt(2, new Point2d(0.0000, 0.1400), 0, 0, 0);
            acPolyExt.AddVertexAt(3, new Point2d(0.1400, 0.1400), 0, 0, 0);
            acPolyExt.Closed = true;

            var acPolyInt = new Polyline();
            acPolyInt.AddVertexAt(0, new Point2d(0.0250, 0.0250), 0, 0, 0);
            acPolyInt.AddVertexAt(1, new Point2d(0.1150, 0.0250), 0, 0, 0);
            acPolyInt.AddVertexAt(2, new Point2d(0.1150, 0.1150), 0, 0, 0);
            acPolyInt.AddVertexAt(3, new Point2d(0.0250, 0.1150), 0, 0, 0);
            acPolyInt.Closed = true;

            var acCirc1 = new Circle
            {
                Center = new Point3d(0.1275, 0.0125, 0),
                Radius = Radius
            };

            var acCirc2 = new Circle
            {
                Center = new Point3d(0.0125, 0.0125, 0),
                Radius = Radius
            };

            var acPol1 = new Polyline();
            acPol1.AddVertexAt(0, new Point2d(0.1150, 0.0250), 0, 0, 0);
            acPol1.AddVertexAt(1, new Point2d(0.1400, 0.0250), 0, 0, 0);

            var acPol2 = new Polyline();
            acPol2.AddVertexAt(0, new Point2d(0.1150, 0.0250), 0, 0, 0);
            acPol2.AddVertexAt(1, new Point2d(0.1150, 0.0000), 0, 0, 0);

            var acPol3 = new Polyline();
            acPol3.AddVertexAt(0, new Point2d(0.0700, 0.0250), 0, 0, 0);
            acPol3.AddVertexAt(1, new Point2d(0.0700, 0.0000), 0, 0, 0);

            var acPol4 = new Polyline();
            acPol4.AddVertexAt(0, new Point2d(0.0250, 0.0250), 0, 0, 0);
            acPol4.AddVertexAt(1, new Point2d(0.0250, 0.0000), 0, 0, 0);

            var acPol5 = new Polyline();
            acPol5.AddVertexAt(0, new Point2d(0.0250, 0.0250), 0, 0, 0);
            acPol5.AddVertexAt(1, new Point2d(0.0000, 0.0250), 0, 0, 0);

            var tmpList = new List<Entity>
            {
                acCirc1, acCirc2, acPol1, acPol2, acPol3, acPol4, acPol5
            };

            entitiesList.Add(acPolyExt);
            entitiesList.Add(acPolyInt);
            entitiesList.AddRange(tmpList);

            entitiesList.AddRange(from entity in tmpList
                let acPtFrom = new Point3d(0.1150, 0.0700, 0)
                let acPtTo = new Point3d(0.1400, 0.0700, 0)
                let acLine3d = new Line3d(acPtFrom, acPtTo)
                select entity.GetTransformedCopy(Matrix3d.Mirroring(acLine3d)));

            var acPol6 = new Polyline();
            acPol6.AddVertexAt(0, new Point2d(0.1150, 0.0700), 0, 0, 0);
            acPol6.AddVertexAt(1, new Point2d(0.1400, 0.0700), 0, 0, 0);

            var acPol7 = new Polyline();
            acPol7.AddVertexAt(0, new Point2d(0.0250, 0.0700), 0, 0, 0);
            acPol7.AddVertexAt(1, new Point2d(0.0000, 0.0700), 0, 0, 0);

            entitiesList.Add(acPol6);
            entitiesList.Add(acPol7);

            var attDefLargura = new AttributeDefinition
            {
                Position = new Point3d(0.1634, 0.2144, 0.0000),
                Verifiable = true,
                Prompt = "Digite a largura: ",
                Tag = "L",

                TextString = "L=0.14",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefLargura.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefLargura.AlignmentPoint = new Point3d(0.1634, 0.2144, 0.0000);

            var attDefAltura = new AttributeDefinition
            {
                Position = new Point3d(0.1634, 0.1517, 0.0000),
                Verifiable = true,
                Prompt = "Digite a altura: ",
                Tag = "A",

                TextString = "A=0.19",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefAltura.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefAltura.AlignmentPoint = new Point3d(0.1634, 0.1517, 0.0000);

            var attDefComprimento = new AttributeDefinition
            {
                Position = new Point3d(0.1634, 0.1017, 0.0000),
                Verifiable = true,
                Prompt = "Digite o comprimento: ",
                Tag = "C",

                TextString = "C=0.29",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefComprimento.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefComprimento.AlignmentPoint = new Point3d(0.1634, 0.1017, 0.0000);

            var attDefAlturaParede = new AttributeDefinition
            {
                Position = new Point3d(0.1634, 0.0517, 0.0000),
                Verifiable = true,
                Prompt = "Digite altura da parede: ",
                Tag = "AP",

                TextString = "AP=2.80",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefAlturaParede.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefAlturaParede.AlignmentPoint = new Point3d(0.1634, 0.0517, 0.0000);

            var attDefQuantidade = new AttributeDefinition
            {
                Position = new Point3d(0.1634, 0.0017, 0.0000),
                Verifiable = true,
                Prompt = "Digite a quantidade: ",
                Tag = "Q",

                TextString = "Q=14",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefQuantidade.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefQuantidade.AlignmentPoint = new Point3d(0.1634, 0.0017, 0.0000);

            var attDefMeiaCanaletaU = new AttributeDefinition
            {
                Position = new Point3d(0.1634, -0.0483, 0.0000),
                Verifiable = true,
                Prompt = "Digite a quantidade canaleta: ",
                Tag = "MeiaCanaletaU",

                TextString = "",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefMeiaCanaletaU.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefMeiaCanaletaU.AlignmentPoint = new Point3d(0.1634, -0.0483, 0.0000);

            var attDefMeiaCanaletaJ = new AttributeDefinition
            {
                Position = new Point3d(0.1634, -0.0983, 0.0000),
                Verifiable = true,
                Prompt = "Digite a quantidade canaleta: ",
                Tag = "MeiaCanaletaJ",

                TextString = "",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefMeiaCanaletaJ.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefMeiaCanaletaJ.AlignmentPoint = new Point3d(0.1634, -0.0983, 0.0000);

            var attDefNome = new AttributeDefinition
            {
                Position = new Point3d(0.07, -0.0609, 0.0000),
                Verifiable = true,
                Prompt = "Digite o nome: ",
                Tag = "N",

                TextString = TipoBloco.BE14x19x29.ToString(),
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefNome.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefNome.AlignmentPoint = new Point3d(0.07, -0.0609, 0.0000);

            ((List<AttributeDefinition>)_atributos).AddRange(new List<AttributeDefinition>
            {
                attDefLargura,
                attDefAltura,
                attDefComprimento,
                attDefAlturaParede,
                attDefQuantidade,
                attDefMeiaCanaletaU,
                attDefMeiaCanaletaJ,
                attDefNome
            });

            return new Tuple<ICollection<Entity>, ICollection<AttributeDefinition>>(entitiesList, _atributos);
        }

        private Tuple<ICollection<Entity>, ICollection<AttributeDefinition>> GetBe14X19X29()
        {
            var entitiesList = new List<Entity>();
            var acPolyExt = new Polyline();
            acPolyExt.AddVertexAt(0, new Point2d(0.0000, 0.0000), 0, 0, 0);
            acPolyExt.AddVertexAt(1, new Point2d(0.0000, 0.1400), 0, 0, 0);
            acPolyExt.AddVertexAt(2, new Point2d(0.2900, 0.1400), 0, 0, 0);
            acPolyExt.AddVertexAt(3, new Point2d(0.2900, 0.0000), 0, 0, 0);
            acPolyExt.Closed = true;

            var acPolyInt = new Polyline();
            acPolyInt.AddVertexAt(0, new Point2d(0.0250, 0.0250), 0, 0, 0);
            acPolyInt.AddVertexAt(1, new Point2d(0.1150, 0.0250), 0, 0, 0);
            acPolyInt.AddVertexAt(2, new Point2d(0.1150, 0.1150), 0, 0, 0);
            acPolyInt.AddVertexAt(3, new Point2d(0.0250, 0.1150), 0, 0, 0);
            acPolyInt.Closed = true;

            var acCirc1 = new Circle
            {
                Center = new Point3d(0.0125, 0.0125, 0),
                Radius = Radius
            };

            var acCirc2 = new Circle
            {
                Center = new Point3d(0.0125, 0.1275, 0),
                Radius = Radius
            };

            var acPol1 = new Polyline();
            acPol1.AddVertexAt(0, new Point2d(0.1150, 0.0250), 0, 0, 0);
            acPol1.AddVertexAt(1, new Point2d(0.1150, 0.0000), 0, 0, 0);

            var acPol2 = new Polyline();
            acPol2.AddVertexAt(0, new Point2d(0.0700, 0.0250), 0, 0, 0);
            acPol2.AddVertexAt(1, new Point2d(0.0700, 0.0000), 0, 0, 0);

            var acPol3 = new Polyline();
            acPol3.AddVertexAt(0, new Point2d(0.0250, 0.0250), 0, 0, 0);
            acPol3.AddVertexAt(1, new Point2d(0.0250, 0.0000), 0, 0, 0);

            var acPol4 = new Polyline();
            acPol4.AddVertexAt(0, new Point2d(0.0250, 0.0250), 0, 0, 0);
            acPol4.AddVertexAt(1, new Point2d(0.0000, 0.0250), 0, 0, 0);

            var acPol5 = new Polyline();
            acPol5.AddVertexAt(0, new Point2d(0.0250, 0.0700), 0, 0, 0);
            acPol5.AddVertexAt(1, new Point2d(0.0000, 0.0700), 0, 0, 0);

            var acPol6 = new Polyline();
            acPol6.AddVertexAt(0, new Point2d(0.0250, 0.1150), 0, 0, 0);
            acPol6.AddVertexAt(1, new Point2d(0.0000, 0.1150), 0, 0, 0);

            var acPol7 = new Polyline();
            acPol7.AddVertexAt(0, new Point2d(0.0250, 0.1150), 0, 0, 0);
            acPol7.AddVertexAt(1, new Point2d(0.0250, 0.1400), 0, 0, 0);

            var acPol8 = new Polyline();
            acPol8.AddVertexAt(0, new Point2d(0.0700, 0.1150), 0, 0, 0);
            acPol8.AddVertexAt(1, new Point2d(0.0700, 0.1400), 0, 0, 0);

            var acPol9 = new Polyline();
            acPol9.AddVertexAt(0, new Point2d(0.1150, 0.1150), 0, 0, 0);
            acPol9.AddVertexAt(1, new Point2d(0.1150, 0.1400), 0, 0, 0);

            var tmpList = new List<Entity>
            {
                acPolyInt, acCirc1, acCirc2, acPol1, acPol2, acPol3, acPol4, acPol5, acPol6, acPol7, acPol8, acPol9
            };

            entitiesList.Add(acPolyExt);
            entitiesList.AddRange(tmpList);

            entitiesList.AddRange(from entity in tmpList
                                  let acPtFrom = new Point3d(0.1450, 0.0850, 0)
                                  let acPtTo = new Point3d(0.1450, 0.2583, 0)
                                  let acLine3d = new Line3d(acPtFrom, acPtTo)
                                  select entity.GetTransformedCopy(Matrix3d.Mirroring(acLine3d)));

            var acPol10 = new Polyline();
            acPol10.AddVertexAt(0, new Point2d(0.1450, 0.0550), 0, 0, 0);
            acPol10.AddVertexAt(1, new Point2d(0.1450, 0.0000), 0, 0, 0);

            var acPol11 = new Polyline();
            acPol11.AddVertexAt(0, new Point2d(0.1150, 0.0550), 0, 0, 0);
            acPol11.AddVertexAt(1, new Point2d(0.1750, 0.0550), 0, 0, 0);

            entitiesList.Add(acPol10);
            entitiesList.Add(acPol11);
            entitiesList.Add(acPol10.GetTransformedCopy(Matrix3d.Mirroring(new Line3d(new Point3d(0.1200, 0.0700, 0), new Point3d(0.1700, 0.0700, 0)))));
            entitiesList.Add(acPol11.GetTransformedCopy(Matrix3d.Mirroring(new Line3d(new Point3d(0.1200, 0.0700, 0), new Point3d(0.1700, 0.0700, 0)))));


            var attDefLargura = new AttributeDefinition
            {
                Position = new Point3d(0.3135, 0.2144, 0.0000),
                Verifiable = true,
                Prompt = "Digite a largura: ",
                Tag = "L",

                TextString = "L=0.14",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefLargura.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefLargura.AlignmentPoint = new Point3d(0.3135, 0.2144, 0.0000);

            var attDefAltura = new AttributeDefinition
            {
                Position = new Point3d(0.3135, 0.1517, 0.0000),
                Verifiable = true,
                Prompt = "Digite a altura: ",
                Tag = "A",

                TextString = "A=0.19",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefAltura.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefAltura.AlignmentPoint = new Point3d(0.3135, 0.1517, 0.0000);

            var attDefComprimento = new AttributeDefinition
            {
                Position = new Point3d(0.3135, 0.1017, 0.0000),
                Verifiable = true,
                Prompt = "Digite o comprimento: ",
                Tag = "C",

                TextString = "C=0.29",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefComprimento.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefComprimento.AlignmentPoint = new Point3d(0.3135, 0.1017, 0.0000);

            var attDefAlturaParede = new AttributeDefinition
            {
                Position = new Point3d(0.3135, 0.0517, 0.0000),
                Verifiable = true,
                Prompt = "Digite altura da parede: ",
                Tag = "AP",

                TextString = "AP=2.80",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefAlturaParede.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefAlturaParede.AlignmentPoint = new Point3d(0.3135, 0.0517, 0.0000);

            var attDefQuantidade = new AttributeDefinition
            {
                Position = new Point3d(0.3135, 0.0017, 0.0000),
                Verifiable = true,
                Prompt = "Digite a quantidade: ",
                Tag = "Q",

                TextString = "Q=14",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefQuantidade.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefQuantidade.AlignmentPoint = new Point3d(0.3135, 0.0017, 0.0000);

            var attDefCanaletaU = new AttributeDefinition
            {
                Position = new Point3d(0.1634, -0.0483, 0.0000),
                Verifiable = true,
                Prompt = "Digite a quantidade canaleta: ",
                Tag = "CanaletaU",

                TextString = "",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefCanaletaU.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefCanaletaU.AlignmentPoint = new Point3d(0.1634, -0.0483, 0.0000);

            var attDefCanaletaJ = new AttributeDefinition
            {
                Position = new Point3d(0.1634, -0.0983, 0.0000),
                Verifiable = true,
                Prompt = "Digite a quantidade canaleta: ",
                Tag = "CanaletaJ",

                TextString = "",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefCanaletaJ.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefCanaletaJ.AlignmentPoint = new Point3d(0.1634, -0.0983, 0.0000);

            var attDefNome = new AttributeDefinition
            {
                Position = new Point3d(0.1339, -0.0559, 0.0000),
                Verifiable = true,
                Prompt = "Digite o nome: ",
                Tag = "N",

                TextString = TipoBloco.BE14x19x29.ToString(),
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefNome.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefNome.AlignmentPoint = new Point3d(0.1339, -0.0559, 0.0000);

            ((List<AttributeDefinition>)_atributos).AddRange(new List<AttributeDefinition>
            {
                attDefLargura,
                attDefAltura,
                attDefComprimento,
                attDefAlturaParede,
                attDefQuantidade,
                attDefCanaletaU,
                attDefCanaletaJ,
                attDefNome
            });

            return new Tuple<ICollection<Entity>, ICollection<AttributeDefinition>>(entitiesList, _atributos);
        }

        private Tuple<ICollection<Entity>, ICollection<AttributeDefinition>> GetBe14X19X34()
        {
            var entitiesList = new List<Entity>();
            var acPolyExt = new Polyline();
            acPolyExt.AddVertexAt(0, new Point2d(0.0000, 0.0000), 0, 0, 0);
            acPolyExt.AddVertexAt(1, new Point2d(0.0000, 0.1400), 0, 0, 0);
            acPolyExt.AddVertexAt(2, new Point2d(0.3400, 0.1400), 0, 0, 0);
            acPolyExt.AddVertexAt(3, new Point2d(0.3400, 0.0000), 0, 0, 0);
            acPolyExt.Closed = true;

            var acPolyInt = new Polyline();
            acPolyInt.AddVertexAt(0, new Point2d(0.0250, 0.0250), 0, 0, 0);
            acPolyInt.AddVertexAt(1, new Point2d(0.1150, 0.0250), 0, 0, 0);
            acPolyInt.AddVertexAt(2, new Point2d(0.1150, 0.1150), 0, 0, 0);
            acPolyInt.AddVertexAt(3, new Point2d(0.0250, 0.1150), 0, 0, 0);
            acPolyInt.Closed = true;

            var acPolyInt2 = new Polyline();
            acPolyInt2.AddVertexAt(0, new Point2d(0.1750, 0.1150), 0, 0, 0);
            acPolyInt2.AddVertexAt(1, new Point2d(0.3150, 0.1150), 0, 0, 0);
            acPolyInt2.AddVertexAt(2, new Point2d(0.3150, 0.0250), 0, 0, 0);
            acPolyInt2.AddVertexAt(3, new Point2d(0.1750, 0.0250), 0, 0, 0);
            acPolyInt2.Closed = true;

            var acCirc1 = new Circle
            {
                Center = new Point3d(0.3275, 0.0125, 0),
                Radius = Radius
            };

            var acCirc2 = new Circle
            {
                Center = new Point3d(0.0125, 0.0125, 0),
                Radius = Radius
            };

            var acPol1 = new Polyline();
            acPol1.AddVertexAt(0, new Point2d(0.3150, 0.0250), 0, 0, 0);
            acPol1.AddVertexAt(1, new Point2d(0.3400, 0.0250), 0, 0, 0);

            var acPol2 = new Polyline();
            acPol2.AddVertexAt(0, new Point2d(0.3150, 0.0250), 0, 0, 0);
            acPol2.AddVertexAt(1, new Point2d(0.3150, 0.0000), 0, 0, 0);

            var acPol3 = new Polyline();
            acPol3.AddVertexAt(0, new Point2d(0.2683, 0.0250), 0, 0, 0);
            acPol3.AddVertexAt(1, new Point2d(0.2683, 0.0000), 0, 0, 0);

            var acPol4 = new Polyline();
            acPol4.AddVertexAt(0, new Point2d(0.2217, 0.0250), 0, 0, 0);
            acPol4.AddVertexAt(1, new Point2d(0.2217, 0.0000), 0, 0, 0);

            var acPol5 = new Polyline();
            acPol5.AddVertexAt(0, new Point2d(0.1750, 0.0250), 0, 0, 0);
            acPol5.AddVertexAt(1, new Point2d(0.1750, 0.0000), 0, 0, 0);

            var acPol6 = new Polyline();
            acPol6.AddVertexAt(0, new Point2d(0.1450, 0.0550), 0, 0, 0);
            acPol6.AddVertexAt(1, new Point2d(0.1450, 0.0000), 0, 0, 0);

            var acPol7 = new Polyline();
            acPol7.AddVertexAt(0, new Point2d(0.1150, 0.0250), 0, 0, 0);
            acPol7.AddVertexAt(1, new Point2d(0.1150, 0.0000), 0, 0, 0);

            var acPol8 = new Polyline();
            acPol8.AddVertexAt(0, new Point2d(0.0700, 0.0250), 0, 0, 0);
            acPol8.AddVertexAt(1, new Point2d(0.0700, 0.0000), 0, 0, 0);

            var acPol9 = new Polyline();
            acPol9.AddVertexAt(0, new Point2d(0.0250, 0.0250), 0, 0, 0);
            acPol9.AddVertexAt(1, new Point2d(0.0250, 0.0000), 0, 0, 0);

            var acPol10 = new Polyline();
            acPol10.AddVertexAt(0, new Point2d(0.0250, 0.0250), 0, 0, 0);
            acPol10.AddVertexAt(1, new Point2d(0.0000, 0.0250), 0, 0, 0);

            var acPol11 = new Polyline();
            acPol11.AddVertexAt(0, new Point2d(0.1150, 0.0550), 0, 0, 0);
            acPol11.AddVertexAt(1, new Point2d(0.1750, 0.0550), 0, 0, 0);

            var tmpList = new List<Entity>
            {
                acCirc1, acCirc2, acPol1, acPol2, acPol3, acPol4, acPol5, acPol6, acPol7, acPol8, acPol9, acPol10, acPol11
            };

            entitiesList.Add(acPolyExt);
            entitiesList.Add(acPolyInt);
            entitiesList.Add(acPolyInt2);
            entitiesList.AddRange(tmpList);

            entitiesList.AddRange(from entity in tmpList
                                  let acPtFrom = new Point3d(0.1150, 0.0700, 0)
                                  let acPtTo = new Point3d(0.1750, 0.0700, 0)
                                  let acLine3d = new Line3d(acPtFrom, acPtTo)
                                  select entity.GetTransformedCopy(Matrix3d.Mirroring(acLine3d)));

            var acPol12 = new Polyline();
            acPol12.AddVertexAt(0, new Point2d(0.3150, 0.0700), 0, 0, 0);
            acPol12.AddVertexAt(1, new Point2d(0.3400, 0.0700), 0, 0, 0);

            var acPol13 = new Polyline();
            acPol13.AddVertexAt(0, new Point2d(0.0250, 0.0700), 0, 0, 0);
            acPol13.AddVertexAt(1, new Point2d(0.0000, 0.0700), 0, 0, 0);

            entitiesList.Add(acPol12);
            entitiesList.Add(acPol13);

            var attDefLargura = new AttributeDefinition
            {
                Position = new Point3d(0.3708, 0.2017, 0.0000),
                Verifiable = true,
                Prompt = "Digite a largura: ",
                Tag = "L",

                TextString = "L=0.14",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefLargura.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefLargura.AlignmentPoint = new Point3d(0.3708, 0.2017, 0.0000);

            var attDefAltura = new AttributeDefinition
            {
                Position = new Point3d(0.3708, 0.1517, 0.0000),
                Verifiable = true,
                Prompt = "Digite a altura: ",
                Tag = "A",

                TextString = "A=0.19",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefAltura.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefAltura.AlignmentPoint = new Point3d(0.3708, 0.1517, 0.0000);

            var attDefComprimento = new AttributeDefinition
            {
                Position = new Point3d(0.3135, 0.1017, 0.0000),
                Verifiable = true,
                Prompt = "Digite o comprimento: ",
                Tag = "C",

                TextString = "C=0.34",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefComprimento.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefComprimento.AlignmentPoint = new Point3d(0.3708, 0.1017, 0.0000);

            var attDefAlturaParede = new AttributeDefinition
            {
                Position = new Point3d(0.3708, 0.0517, 0.0000),
                Verifiable = true,
                Prompt = "Digite altura da parede: ",
                Tag = "AP",

                TextString = "AP=2.80",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefAlturaParede.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefAlturaParede.AlignmentPoint = new Point3d(0.3708, 0.0517, 0.0000);

            var attDefQuantidade = new AttributeDefinition
            {
                Position = new Point3d(0.3708, 0.0017, 0.0000),
                Verifiable = true,
                Prompt = "Digite a quantidade: ",
                Tag = "Q",

                TextString = "Q=14",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefQuantidade.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefQuantidade.AlignmentPoint = new Point3d(0.3708, 0.0017, 0.0000);

            var attDefCanaletaU = new AttributeDefinition
            {
                Position = new Point3d(0.1634, -0.0483, 0.0000),
                Verifiable = true,
                Prompt = "Digite a quantidade canaleta: ",
                Tag = "CanaletaU",

                TextString = "",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefCanaletaU.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefCanaletaU.AlignmentPoint = new Point3d(0.1634, -0.0483, 0.0000);

            var attDefCanaletaJ = new AttributeDefinition
            {
                Position = new Point3d(0.1634, -0.0983, 0.0000),
                Verifiable = true,
                Prompt = "Digite a quantidade canaleta: ",
                Tag = "CanaletaJ",

                TextString = "",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefCanaletaJ.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefCanaletaJ.AlignmentPoint = new Point3d(0.1634, -0.0983, 0.0000);

            var attDefNome = new AttributeDefinition
            {
                Position = new Point3d(0.17, -0.0609, 0.0000),
                Verifiable = true,
                Prompt = "Digite o nome: ",
                Tag = "N",

                TextString = TipoBloco.BE14x19x34.ToString(),
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefNome.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefNome.AlignmentPoint = new Point3d(0.17, -0.0609, 0.0000);

            ((List<AttributeDefinition>)_atributos).AddRange(new List<AttributeDefinition>
            {
                attDefLargura,
                attDefAltura,
                attDefComprimento,
                attDefAlturaParede,
                attDefQuantidade,
                attDefCanaletaU,
                attDefCanaletaJ,
                attDefNome
            });

            return new Tuple<ICollection<Entity>, ICollection<AttributeDefinition>>(entitiesList, _atributos);
        }

        private Tuple<ICollection<Entity>, ICollection<AttributeDefinition>> GetBe14X19X39()
        {
            var entitiesList = new List<Entity>();
            var acPolyExt = new Polyline();

            acPolyExt.AddVertexAt(0, new Point2d(0.0000, 0.0000), 0, 0, 0);
            acPolyExt.AddVertexAt(1, new Point2d(0.0000, 0.1400), 0, 0, 0);
            acPolyExt.AddVertexAt(2, new Point2d(0.3900, 0.1400), 0, 0, 0);
            acPolyExt.AddVertexAt(3, new Point2d(0.3900, 0.0000), 0, 0, 0);
            acPolyExt.Closed = true;

            var acPolyInt = new Polyline();
            acPolyInt.AddVertexAt(0, new Point2d(0.1650, 0.1150), 0, 0, 0);
            acPolyInt.AddVertexAt(1, new Point2d(0.0250, 0.1150), 0, 0, 0);
            acPolyInt.AddVertexAt(2, new Point2d(0.0250, 0.0250), 0, 0, 0);
            acPolyInt.AddVertexAt(3, new Point2d(0.1650, 0.0250), 0, 0, 0);
            acPolyInt.Closed = true;

            var acCirc1 = new Circle
            {
                Center = new Point3d(0.0125, 0.0125, 0),
                Radius = Radius
            };

            var acCirc2 = new Circle
            {
                Center = new Point3d(0.0125, 0.1275, 0),
                Radius = Radius
            };

            var acPol1 = new Polyline();
            acPol1.AddVertexAt(0, new Point2d(0.1650, 0.0250), 0, 0, 0);
            acPol1.AddVertexAt(1, new Point2d(0.1650, 0.0000), 0, 0, 0);

            var acPol2 = new Polyline();
            acPol2.AddVertexAt(0, new Point2d(0.1183, 0.0250), 0, 0, 0);
            acPol2.AddVertexAt(1, new Point2d(0.1183, 0.0000), 0, 0, 0);

            var acPol3 = new Polyline();
            acPol3.AddVertexAt(0, new Point2d(0.0717, 0.0250), 0, 0, 0);
            acPol3.AddVertexAt(1, new Point2d(0.0717, 0.0000), 0, 0, 0);

            var acPol4 = new Polyline();
            acPol4.AddVertexAt(0, new Point2d(0.0250, 0.0250), 0, 0, 0);
            acPol4.AddVertexAt(1, new Point2d(0.0250, 0.0000), 0, 0, 0);

            var acPol5 = new Polyline();
            acPol5.AddVertexAt(0, new Point2d(0.0250, 0.0250), 0, 0, 0);
            acPol5.AddVertexAt(1, new Point2d(0.0000, 0.0250), 0, 0, 0);

            var acPol6 = new Polyline();
            acPol6.AddVertexAt(0, new Point2d(0.0250, 0.0700), 0, 0, 0);
            acPol6.AddVertexAt(1, new Point2d(0.0000, 0.0700), 0, 0, 0);

            var acPol7 = new Polyline();
            acPol7.AddVertexAt(0, new Point2d(0.0250, 0.1150), 0, 0, 0);
            acPol7.AddVertexAt(1, new Point2d(0.0000, 0.1150), 0, 0, 0);

            var acPol8 = new Polyline();
            acPol8.AddVertexAt(0, new Point2d(0.0250, 0.1150), 0, 0, 0);
            acPol8.AddVertexAt(1, new Point2d(0.0250, 0.1400), 0, 0, 0);

            var acPol9 = new Polyline();
            acPol9.AddVertexAt(0, new Point2d(0.0717, 0.1150), 0, 0, 0);
            acPol9.AddVertexAt(1, new Point2d(0.0717, 0.1400), 0, 0, 0);

            var acPol10 = new Polyline();
            acPol10.AddVertexAt(0, new Point2d(0.1183, 0.1150), 0, 0, 0);
            acPol10.AddVertexAt(1, new Point2d(0.1183, 0.1400), 0, 0, 0);

            var acPol11 = new Polyline();
            acPol11.AddVertexAt(0, new Point2d(0.1650, 0.1150), 0, 0, 0);
            acPol11.AddVertexAt(1, new Point2d(0.1650, 0.1400), 0, 0, 0);

            var tmpList = new List<Entity>
            {
                acPolyInt, acCirc1, acCirc2, acPol1, acPol2, acPol3, acPol4, acPol5,
                acPol6, acPol7, acPol8, acPol9, acPol10, acPol11
            };

            entitiesList.Add(acPolyExt);    
            entitiesList.AddRange(tmpList);

            entitiesList.AddRange(from entity in tmpList
                                  let acPtFrom = new Point3d(0.1950, 0.0850, 0)
                                  let acPtTo = new Point3d(0.1950, 0.1400, 0)
                                  let acLine3d = new Line3d(acPtFrom, acPtTo)
                                  select entity.GetTransformedCopy(Matrix3d.Mirroring(acLine3d)));

            var acPol12 = new Polyline();
            acPol12.AddVertexAt(0, new Point2d(0.1950, 0.0550), 0, 0, 0);
            acPol12.AddVertexAt(1, new Point2d(0.1950, 0.0000), 0, 0, 0);

            var acPol13 = new Polyline();
            acPol13.AddVertexAt(0, new Point2d(0.1650, 0.0550), 0, 0, 0);
            acPol13.AddVertexAt(1, new Point2d(0.2250, 0.0550), 0, 0, 0);

            entitiesList.Add(acPol12);
            entitiesList.Add(acPol13);

            var ptFromPoint3d = new Point3d(0.1650, 0.0700, 0);
            var ptTPoint3d = new Point3d(0.2250, 0.0700, 0);
            var baseLine3d = new Line3d(ptFromPoint3d, ptTPoint3d);

            entitiesList.Add(acPol12.GetTransformedCopy(Matrix3d.Mirroring(baseLine3d)));
            entitiesList.Add(acPol13.GetTransformedCopy(Matrix3d.Mirroring(baseLine3d)));

            var attDefLargura = new AttributeDefinition
            {
                Position = new Point3d(0.438, 0.2017, 0.0000),
                Verifiable = true,
                Prompt = "Digite a largura: ",
                Tag = "L",
                TextString = "L=0.14",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,
                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefLargura.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefLargura.AlignmentPoint = new Point3d(0.438, 0.2017, 0.0000);

            var attDefAltura = new AttributeDefinition
            {
                Position = new Point3d(0.438, 0.1517, 0.0000),
                Verifiable = true,
                Prompt = "Digite a altura: ",
                Tag = "A",
                TextString = "A=0.19",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,
                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefAltura.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefAltura.AlignmentPoint = new Point3d(0.438, 0.1517, 0.0000);

            var attDefComprimento = new AttributeDefinition
            {
                Position = new Point3d(0.438, 0.1017, 0.0000),
                Verifiable = true,
                Prompt = "Digite o comprimento: ",
                Tag = "C",
                TextString = "C=0.29",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,
                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefComprimento.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefComprimento.AlignmentPoint = new Point3d(0.438, 0.1017, 0.0000);

            var attDefAlturaParede = new AttributeDefinition
            {
                Position = new Point3d(0.438, 0.0517, 0.0000),
                Verifiable = true,
                Prompt = "Digite altura da parede: ",
                Tag = "AP",
                TextString = "AP=2.80",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,
                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefAlturaParede.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefAlturaParede.AlignmentPoint = new Point3d(0.438, 0.0517, 0.0000);

            var attDefQuantidade = new AttributeDefinition
            {
                Position = new Point3d(0.438, 0.0017, 0.0000),
                Verifiable = true,
                Prompt = "Digite a quantidade: ",
                Tag = "Q",
                TextString = "Q=14",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,
                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefQuantidade.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefQuantidade.AlignmentPoint = new Point3d(0.438, 0.0017, 0.0000);

            var attDefCanaletaU = new AttributeDefinition
            {
                Position = new Point3d(0.1634, -0.0483, 0.0000),
                Verifiable = true,
                Prompt = "Digite a quantidade canaleta: ",
                Tag = "CanaletaU",
                TextString = "",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,
                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefCanaletaU.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefCanaletaU.AlignmentPoint = new Point3d(0.1634, -0.0483, 0.0000);

            var attDefCanaletaJ = new AttributeDefinition
            {
                Position = new Point3d(0.1634, -0.0983, 0.0000),
                Verifiable = true,
                Prompt = "Digite a quantidade canaleta: ",
                Tag = "CanaletaJ",
                TextString = "",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,
                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefCanaletaJ.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefCanaletaJ.AlignmentPoint = new Point3d(0.1634, -0.0983, 0.0000);

            var attDefNome = new AttributeDefinition
            {
                Position = new Point3d(0.17, -0.0609, 0.0000),
                Verifiable = true,
                Prompt = "Digite o nome: ",
                Tag = "N",
                TextString = TipoBloco.BE14x19x34.ToString(),
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,
                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefNome.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefNome.AlignmentPoint = new Point3d(0.17, -0.0609, 0.0000);

            ((List<AttributeDefinition>)_atributos).AddRange(new List<AttributeDefinition>
            {
                attDefLargura,
                attDefAltura,
                attDefComprimento,
                attDefAlturaParede,
                attDefQuantidade,
                attDefCanaletaU,
                attDefCanaletaJ,
                attDefNome
            });

            return new Tuple<ICollection<Entity>, ICollection<AttributeDefinition>>(entitiesList, _atributos);
        }

        private Tuple<ICollection<Entity>, ICollection<AttributeDefinition>> GetBe14X19X44()
        {
            var entitiesList = new List<Entity>();
            var acPolyExt = new Polyline();
            acPolyExt.AddVertexAt(0, new Point2d(0.0000, 0.0000), 0, 0, 0);
            acPolyExt.AddVertexAt(1, new Point2d(0.0000, 0.1400), 0, 0, 0);
            acPolyExt.AddVertexAt(2, new Point2d(0.4400, 0.1400), 0, 0, 0);
            acPolyExt.AddVertexAt(3, new Point2d(0.4400, 0.0000), 0, 0, 0);
            acPolyExt.Closed = true;

            var acPolyInt = new Polyline();
            acPolyInt.AddVertexAt(0, new Point2d(0.0250, 0.0250), 0, 0, 0);
            acPolyInt.AddVertexAt(1, new Point2d(0.1150, 0.0250), 0, 0, 0);
            acPolyInt.AddVertexAt(2, new Point2d(0.1150, 0.1150), 0, 0, 0);
            acPolyInt.AddVertexAt(3, new Point2d(0.0250, 0.1150), 0, 0, 0);
            acPolyInt.Closed = true;

            var acPolyInt2 = new Polyline();
            acPolyInt2.AddVertexAt(0, new Point2d(0.2650, 0.0250), 0, 0, 0);
            acPolyInt2.AddVertexAt(1, new Point2d(0.1750, 0.0250), 0, 0, 0);
            acPolyInt2.AddVertexAt(2, new Point2d(0.1750, 0.1150), 0, 0, 0);
            acPolyInt2.AddVertexAt(3, new Point2d(0.2650, 0.1150), 0, 0, 0);
            acPolyInt2.Closed = true;

            var acPolyInt3 = new Polyline();
            acPolyInt3.AddVertexAt(0, new Point2d(0.4150, 0.0250), 0, 0, 0);
            acPolyInt3.AddVertexAt(1, new Point2d(0.3250, 0.0250), 0, 0, 0);
            acPolyInt3.AddVertexAt(2, new Point2d(0.3250, 0.1150), 0, 0, 0);
            acPolyInt3.AddVertexAt(3, new Point2d(0.4150, 0.1150), 0, 0, 0);
            acPolyInt3.Closed = true;

            var acCirc1 = new Circle
            {
                Center = new Point3d(0.4275, 0.0125, 0),
                Radius = Radius
            };

            var acCirc2 = new Circle
            {
                Center = new Point3d(0.0125, 0.0125, 0),
                Radius = Radius
            };

            var acPol1 = new Polyline();
            acPol1.AddVertexAt(0, new Point2d(0.4150, 0.0250), 0, 0, 0);
            acPol1.AddVertexAt(1, new Point2d(0.4400, 0.0250), 0, 0, 0);

            var acPol2 = new Polyline();
            acPol2.AddVertexAt(0, new Point2d(0.4150, 0.0250), 0, 0, 0);
            acPol2.AddVertexAt(1, new Point2d(0.4150, 0.0000), 0, 0, 0);

            var acPol3 = new Polyline();
            acPol3.AddVertexAt(0, new Point2d(0.3700, 0.0250), 0, 0, 0);
            acPol3.AddVertexAt(1, new Point2d(0.3700, 0.0000), 0, 0, 0);

            var acPol4 = new Polyline();
            acPol4.AddVertexAt(0, new Point2d(0.3250, 0.0250), 0, 0, 0);
            acPol4.AddVertexAt(1, new Point2d(0.3250, 0.0000), 0, 0, 0);

            var acPol5 = new Polyline();
            acPol5.AddVertexAt(0, new Point2d(0.2950, 0.0550), 0, 0, 0);
            acPol5.AddVertexAt(1, new Point2d(0.2950, 0.0000), 0, 0, 0);

            var acPol6 = new Polyline();
            acPol6.AddVertexAt(0, new Point2d(0.2650, 0.0250), 0, 0, 0);
            acPol6.AddVertexAt(1, new Point2d(0.2650, 0.0000), 0, 0, 0);

            var acPol7 = new Polyline();
            acPol7.AddVertexAt(0, new Point2d(0.2200, 0.0250), 0, 0, 0);
            acPol7.AddVertexAt(1, new Point2d(0.2200, 0.0000), 0, 0, 0);

            var acPol8 = new Polyline();
            acPol8.AddVertexAt(0, new Point2d(0.1750, 0.0250), 0, 0, 0);
            acPol8.AddVertexAt(1, new Point2d(0.1750, 0.0000), 0, 0, 0);

            var acPol9 = new Polyline();
            acPol9.AddVertexAt(0, new Point2d(0.1450, 0.0550), 0, 0, 0);
            acPol9.AddVertexAt(1, new Point2d(0.1450, 0.0000), 0, 0, 0);

            var acPol10 = new Polyline();
            acPol10.AddVertexAt(0, new Point2d(0.1150, 0.0250), 0, 0, 0);
            acPol10.AddVertexAt(1, new Point2d(0.1150, 0.0000), 0, 0, 0);

            var acPol11 = new Polyline();
            acPol11.AddVertexAt(0, new Point2d(0.0700, 0.0250), 0, 0, 0);
            acPol11.AddVertexAt(1, new Point2d(0.0700, 0.0000), 0, 0, 0);

            var acPol12 = new Polyline();
            acPol12.AddVertexAt(0, new Point2d(0.0250, 0.0250), 0, 0, 0);
            acPol12.AddVertexAt(1, new Point2d(0.0250, 0.0000), 0, 0, 0);

            var acPol13 = new Polyline();
            acPol13.AddVertexAt(0, new Point2d(0.0250, 0.0250), 0, 0, 0);
            acPol13.AddVertexAt(1, new Point2d(0.0000, 0.0250), 0, 0, 0);

            var acPol14 = new Polyline();
            acPol14.AddVertexAt(0, new Point2d(0.3250, 0.0550), 0, 0, 0);
            acPol14.AddVertexAt(1, new Point2d(0.2650, 0.0550), 0, 0, 0);

            var acPol15 = new Polyline();
            acPol15.AddVertexAt(0, new Point2d(0.1150, 0.0550), 0, 0, 0);
            acPol15.AddVertexAt(1, new Point2d(0.1750, 0.0550), 0, 0, 0);

            var tmpList = new List<Entity>
            {
                acCirc1, acCirc2, acPol1, acPol2, acPol3, acPol4, acPol5, acPol6, acPol7, acPol8,
                acPol9, acPol10, acPol11, acPol12, acPol13, acPol14, acPol15
            };

            entitiesList.Add(acPolyExt);
            entitiesList.Add(acPolyInt);
            entitiesList.Add(acPolyInt2);
            entitiesList.Add(acPolyInt3);

            entitiesList.AddRange(tmpList);
            entitiesList.AddRange(from entity in tmpList
                                  let acPtFrom = new Point3d(0.4150, 0.0700, 0)
                                  let acPtTo = new Point3d(0.4400, 0.0700, 0)
                                  let acLine3d = new Line3d(acPtFrom, acPtTo)
                                  select entity.GetTransformedCopy(Matrix3d.Mirroring(acLine3d)));

            var acPol16 = new Polyline();
            acPol16.AddVertexAt(0, new Point2d(0.4150, 0.0700), 0, 0, 0);
            acPol16.AddVertexAt(1, new Point2d(0.4400, 0.0700), 0, 0, 0);

            var acPol17 = new Polyline();
            acPol17.AddVertexAt(0, new Point2d(0.0250, 0.0700), 0, 0, 0);
            acPol17.AddVertexAt(1, new Point2d(0.0000, 0.0700), 0, 0, 0);

            entitiesList.Add(acPol16);
            entitiesList.Add(acPol17);

            var attDefLargura = new AttributeDefinition
            {
                Position = new Point3d(0.4591, 0.2017, 0.0000),
                Verifiable = true,
                Prompt = "Digite a largura: ",
                Tag = "L",

                TextString = "L=0.14",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefLargura.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefLargura.AlignmentPoint = new Point3d(0.4591, 0.2017, 0.0000);

            var attDefAltura = new AttributeDefinition
            {
                Position = new Point3d(0.4591, 0.1517, 0.0000),
                Verifiable = true,
                Prompt = "Digite a altura: ",
                Tag = "A",

                TextString = "A=0.19",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefAltura.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefAltura.AlignmentPoint = new Point3d(0.4591, 0.1517, 0.0000);

            var attDefComprimento = new AttributeDefinition
            {
                Position = new Point3d(0.4591, 0.1017, 0.0000),
                Verifiable = true,
                Prompt = "Digite o comprimento: ",
                Tag = "C",

                TextString = "C=0.44",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefComprimento.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefComprimento.AlignmentPoint = new Point3d(0.4591, 0.1017, 0.0000);

            var attDefAlturaParede = new AttributeDefinition
            {
                Position = new Point3d(0.4591, 0.0517, 0.0000),
                Verifiable = true,
                Prompt = "Digite altura da parede: ",
                Tag = "AP",

                TextString = "AP=2.80",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefAlturaParede.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefAlturaParede.AlignmentPoint = new Point3d(0.4591, 0.0517, 0.0000);

            var attDefQuantidade = new AttributeDefinition
            {
                Position = new Point3d(0.4591, 0.0017, 0.0000),
                Verifiable = true,
                Prompt = "Digite a quantidade: ",
                Tag = "Q",

                TextString = "Q=14",
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefQuantidade.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefQuantidade.AlignmentPoint = new Point3d(0.4591, 0.0017, 0.0000);

            var attDefNome = new AttributeDefinition
            {
                Position = new Point3d(0.22, -0.0609, 0.0000),
                Verifiable = true,
                Prompt = "Digite o nome: ",
                Tag = "N",

                TextString = TipoBloco.BEAM14x19x44.ToString(),
                Height = 0.03,
                Justify = AttachmentPoint.BottomLeft,

                Invisible = true,
                TextStyleId = _textRepository.GetTextStyleByName()
            };
            attDefNome.AdjustAlignment(HostApplicationServices.WorkingDatabase);
            attDefNome.AlignmentPoint = new Point3d(0.22, -0.0609, 0.0000);

            ((List<AttributeDefinition>)_atributos).AddRange(new List<AttributeDefinition>
            {
                attDefLargura,
                attDefAltura,
                attDefComprimento,
                attDefAlturaParede,
                attDefQuantidade,
                attDefNome
            });
            return new Tuple<ICollection<Entity>, ICollection<AttributeDefinition>>(entitiesList, _atributos);
        }

        public static Point2d GetPolarPoints(Point2d pPt, double dAng, double dDist)
        {
            return new Point2d(pPt.X + dDist * Math.Cos(dAng), pPt.Y + dDist * Math.Sin(dAng));
        }

        public static Point3d GetPolarPoints(Point3d pPt, double dAng, double dDist)
        {
            return new Point3d(pPt.X + dDist * Math.Cos(dAng), pPt.Y + dDist * Math.Sin(dAng), pPt.Z);
        }

        public static double GetAngleBetweenTwoPoints(Point2d startPoint2d, Point2d endPoint2d)
        {
            return startPoint2d.GetVectorTo(endPoint2d).Angle;
        }

        public static double GetAngleBetweenTwoPoints(Point3d starPoint3d, Point3d endPoint3d)
        {
            return starPoint3d.GetVectorTo(endPoint3d).GetAngleTo(Vector3d.XAxis, Vector3d.ZAxis.Negate());
        }

        public static double GetDistanceBetweenTwoPoints(Point2d startPoint2d, Point2d endPoint2d)
        {
            return startPoint2d.GetDistanceTo(endPoint2d);
        }

        public static double GetDistanceBetweenTwoPoints(Point3d starPoint3d, Point3d endPoint3d)
        {
            return starPoint3d.DistanceTo(endPoint3d);
        }

        #region IDisposable Support
        private bool _disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    foreach (var attributeDefinition in _atributos)
                    {
                        attributeDefinition.Dispose();
                    }
                    _atributos = null;
                }
                _disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}