using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DressBookingMVC.DataManager.Filter
{

    public interface IFilter<TEntity>
    {
        bool Apply(TEntity entity);
    }
}
