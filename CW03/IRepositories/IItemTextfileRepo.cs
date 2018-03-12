using CW03.Models;

namespace CW03.IRepositories
{
    public interface IItemTextfileRepo
    {
        void InsertAsync(ItemTextFile Obj);
        void Update(ItemTextFile Obj);
        void DeleteAsync(int Id);
    }
}
