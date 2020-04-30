using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.WebShop.UI.Tests.Mock
{
    public class MockRepository<T> : IRepository<T> where T : BaseEntity
    {
        List<T> items;

        public MockRepository()
        {
            this.items = new List<T>();
        }

        public void Commit()
        {
            return;
        }

        public void Insert(T t)
        {
            items.Add(t);
        }

        public void Update(T t)
        {
            T tToUpdate = items.Find(i => i.Id == t.Id);
            if (tToUpdate == null)
            {
                return;
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
            if (t == null)
            {
                return null;
            }
            else
            {
                return t;
            }
        }

        public void Delete(string id)
        {
            T tToDelete = items.Find(i => i.Id == id);
            if (tToDelete == null)
            {
                return;
            }
            else
            {
                items.Remove(tToDelete);
            }
        }
    }
}
