using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DressBookingMVC.DataManager
{
    public interface IDataManager<TEntity> 
    {
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> GetAll(FilterState filter);
        IEnumerable<TEntity> GetAll(FilterState filter, Dictionary<string, string> filters);
        TEntity Get(long id);
        long Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);

        void Dispose();
    }
    public enum FilterState
    {
        Administrator,
        Customer
    };

}
