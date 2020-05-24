using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HandshakeGame.Database.Models
{
    public interface IDBModel<T, TCreate>
    {
        public List<T> GetAll();
        public T GetOne(int id);
        public void Update(T item);
        public void Delete(T item);
        public T Create(TCreate item);
    }
}
