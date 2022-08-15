using Microsoft.EntityFrameworkCore;
using MyBlog.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MyBlog.Infrastructure.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected AppDbContext _db;
        private DbSet<T> dbSet { get; set; }

        public Repository(AppDbContext db)
        {
            _db = db;
            var set = db.Set<T>();
            set.Load();
            dbSet = set;
        }

        public void Create(T item)
        {
            dbSet.Add(item);
            _db.SaveChanges();
        }

        public void Delete(T item)
        {
            dbSet.Remove(item);
            _db.SaveChanges();
        }

        public T Get(int id)
        {
            return dbSet.Find(id);
        }

        public List<T> GetAll()
        {
            return dbSet.ToList();
        }

        public void Update(T item)
        {
            dbSet.Update(item);
            _db.SaveChanges();
        }
    }
}
