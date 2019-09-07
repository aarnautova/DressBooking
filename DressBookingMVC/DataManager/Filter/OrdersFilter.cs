using DressBookingMVC.DataManager.Filter;
using DressBookingMVC.Models.DressBookingDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DressBookingWebApi.Models.AdditionalModels
{
    public abstract class OrdersFilter : IFilter<Order>
    {
        public abstract bool Apply(Order entity);
    }

    public class UserAndDressIdFilter : OrdersFilter
    {
        private string _userId;
        private long _dressId;

        public void SetUserId(string id)
        {
            _userId = id;
        }

        public void SetDressId(string id)
        {
            if (String.IsNullOrEmpty(id))
                _dressId = -1;
            else
                _dressId = Convert.ToInt64(id);
        }

        public override bool Apply(Order entity)
        {
            bool result = true;
            if (!String.IsNullOrEmpty(_userId))
                result = (entity.UserName == _userId);
            if (_dressId > 0)
                result = result && (entity.DressId == _dressId && DateTime.Compare(DateTime.Today, entity.StartDate)<=0);
            return result;
        }
    }
}
