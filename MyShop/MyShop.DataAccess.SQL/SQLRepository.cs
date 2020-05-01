using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.SQL
{
    public class SQLRepository<T> : IRepository<T> where T : BaseEntity
    {
        DataContext context;
        DbSet<T> dbSet;

        public SQLRepository(DataContext context)   
        {
            this.context = context;
            this.dbSet = this.context.Set<T>();
        }

        public IQueryable<T> Collection()
        {
            return this.dbSet;
        }

        public void Commit()
        {
            this.context.SaveChanges();
        }

        public void Delete(string id)
        {
            var t = Find(id);
            if(context.Entry(t).State == EntityState.Detached)
            {
                dbSet.Attach(t);
            }

            dbSet.Remove(t);
        }

        public T Find(string Id)
        {
            return this.dbSet.Find(Id);
        }

        public void Insert(T t)
        {
            this.dbSet.Add(t);
        }

        public void Update(T t)
        {
            this.dbSet.Attach(t);
            this.context.Entry(t).State = EntityState.Modified;
        }
    }
}
