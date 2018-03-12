using CW03.Data;
using CW03.IRepositories;
using CW03.Models;

namespace CW03.Repositories
{
    public class ItemLinkRepo : IItemLinkRepo
    {
        private readonly CW03Context db;

        public ItemLinkRepo(CW03Context context)
        {
            db = context;
        }
        public async void DeleteAsync(int Id)
        {
            var i = await db.ItemLink.FindAsync(Id);
            db.ItemLink.Remove(i);
        }

        public async void InsertAsync(ItemLink Obj)
        {
            await db.ItemLink.AddAsync(Obj);
        }

        public void Update(ItemLink Obj)
        {
            db.Entry(Obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }       
    }
}
