using CW03.Data;
using CW03.IRepositories;
using CW03.Models;

namespace CW03.Repositories
{
    public class ItemTextfileRepo : IItemTextfileRepo
    {
        private readonly CW03Context db;

        public ItemTextfileRepo(CW03Context context)
        {
            db = context;
        }
        public async  void DeleteAsync(int Id)
        {
            var i = await db.ItemTextFile.FindAsync(Id);
            db.ItemTextFile.Remove(i);
        }

        public async void InsertAsync(ItemTextFile Obj)
        {
            await db.ItemTextFile.AddAsync(Obj);
        }

        public void Update(ItemTextFile Obj)
        {
            db.Entry(Obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
    }
}
