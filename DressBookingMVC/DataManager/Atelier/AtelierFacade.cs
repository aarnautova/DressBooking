using DressBookingMVC.Helpers;
using DressBookingMVC.Models.DressBookingDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DressBookingMVC.DataManager.Atelier
{
    public class AtelierFacade
    {

        private DressesDataManager _dressesDataManager;
        private OrderValidationDataManager _orderDataManager;
        private DressesStorage _ds;


        public AtelierFacade(DressBookingDB1Entities context)
        {
            _dressesDataManager = new DressesDataManager(context);
            _orderDataManager = new OrderValidationDataManager(context);
            _ds = DressesStorage.Instance(_dressesDataManager);
        }
        public Dress SewDress(long key, int size, string login)
        {
            if (size < 0 || size >= Constants.sizes.Length)
                throw new Exception("Wrong size");
            try
            {
                Dress d = _ds.GetDressPrototype(key);
                d.SizeId = size;
                string newName = d.Name + "(Copy)";
                d.Name = newName;
                decimal newPrice = d.PriceToBuy * (decimal)1.5;
                d.PriceToBuy = newPrice;
                d.IsBought = true;
                d.IsCopy = true;
                long dressId = _dressesDataManager.Add(d);
                _orderDataManager.Add(OrderBuilder(dressId, login));
                return d;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        private Order OrderBuilder(long dressId, string login)
        {
            Order o = new Order();
            o.DressId = dressId;
            DateTime now = DateTime.Now;
            o.StartDate = now.AddDays(1);
            o.Booking = false;
            o.UserName = login;
            return o;
        }
    }
}