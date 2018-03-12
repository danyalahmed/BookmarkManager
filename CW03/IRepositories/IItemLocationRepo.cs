using CW03.Models;

namespace CW03.IRepositories
{
    public interface IItemLocationRepo
    {
        void InsertAsync(ItemLocation Obj);
        void Update(ItemLocation Obj);
        void DeleteAsync(int Id);
    }
}
