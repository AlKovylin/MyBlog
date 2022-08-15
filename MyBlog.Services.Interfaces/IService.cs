using System;
using System.Collections.Generic;
using System.Text;

namespace MyBlog.Services.Interfaces
{
    public interface IService<T> where T : class
    {
        public void Create(T item);

        public void Delete(T item);

        public T Get(int id);

        public List<T> GetList();

        public void Update(T item);
    }
}
