using CW03.Models;

namespace CW03.IRepositories
{
    public interface IItemLinkRepo
    {
        void InsertAsync(ItemLink Obj);
        void Update(ItemLink Obj);
        void DeleteAsync(int Id);
    }
}
