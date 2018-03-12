using CW03.Data;
using CW03.IRepositories;
using CW03.Models;

namespace CW03.Repositories
{
    public class ItemLocationRepo : IItemLocationRepo
    {
        private readonly CW03Context db;

        public ItemLocationRepo(CW03Context context)
        {
            db = context;
        }
        public async void DeleteAsync(int Id)
        {
            var i = await db.ItemLocation.FindAsync(Id);
            db.ItemLocation.Remove(i);
        }

        public async void InsertAsync(ItemLocation Obj)
        {
           await db.ItemLocation.AddAsync(Obj);
        }

        public void Update(ItemLocation Obj)
        {
            db.Entry(Obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
    }
}
