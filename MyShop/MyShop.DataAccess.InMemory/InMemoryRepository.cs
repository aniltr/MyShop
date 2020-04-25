using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.InMemory
{
    public class InMemoryRepository<T> where T: BaseEntity
    {
        ObjectCache cache = MemoryCache.Default;
        string className;
        List<T> items;

        public InMemoryRepository()
        {
            this.className = typeof(T).Name;
            this.items = cache[className] as List<T>;
            if(items == null)
            {
                this.items = new List<T>();
            }
        }

        public void Commit()
        {
            cache[this.className] = items;
        }

        public void Insert(T t)
        {
            items.Add(t);
        }

        public void Update(T t)
        {
            T tToUpdate = items.Find(i => i.Id == t.Id);
            if(tToUpdate == null)
            {
                throw new Exception(className + " not found");
            }
            else
            {
                tToUpdate = t;
            }
        }

        public IQueryable<T> Collection()
        {
            return items.AsQueryable();
        }

        public T Find(string Id)
        {
            T t = items.Find(i => i.Id == Id);
            if(t == null)
            {
                throw new Exception(className + " not found");
            }
            else
            {
                return t;
            }
        }

        public void Delete(string id)
        {
            T tToDelete = items.Find(i => i.Id == id);
            if(tToDelete == null)
            {
                throw new Exception(className + " not found");
            }
            else
            {
                items.Remove(tToDelete);
            }
        }
    }
}
