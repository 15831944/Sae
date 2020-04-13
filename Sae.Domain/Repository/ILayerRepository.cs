using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;

namespace Sae.Domain.Repository
{
    public interface ILayerRepository
    {
        ObjectId CreateALayer(string name, ColorMethod method, short indexColor, LineWeight? lineWeight, string description = null);
        ObjectId GetLayer(string name);
    }
}