using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace Sae.Domain.Repository
{
    public interface IBlockRepository<T, TE> where T : class where TE : class
    {
        ObjectId CreatingABlock(string nome, ICollection<T> entidades, ICollection<TE> atributos, Point3d? origin = null);
        void InsertBlock(string nome, Point3d pontoInsercao, double? rotacao = 0.0, double? altura = 2.80);
        ObjectId Get(string nome);
    }
}