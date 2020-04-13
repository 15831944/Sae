using Autodesk.AutoCAD.DatabaseServices;

namespace Sae.Domain.Repository
{
    public interface ITextRepository
    {
        ObjectId GetTextStyleByName(string name = "Legend");
    }
}