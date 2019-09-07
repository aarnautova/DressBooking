using DressBookingMVC.Models.DressBookingDataModel;
using DressBookingWebApi.Models.AdditionalModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DressBookingMVC.DataManager
{
    interface IOrderProcessing
    {
        void Process(Order order);
    }
    internal class BuyOrder : IOrderProcessing
    {
        public void Process(Order order)
        {
            List<Order> bookings
                          = order.Dress.Orders
                          .Where(o => o.EndDate != null
                          && DateTime.Compare((DateTime)order.StartDate, (DateTime)o.EndDate) < 0)
                          .ToList();
            if (bookings.Count() == 0)
            {
                order.Dress.IsBought = true;
                order.Sum = order.Dress.PriceToBuy;
            }

            else throw new Exception("Dress is booked, try to buy it later");
        }
    }
    internal class BookingOrder : IOrderProcessing
    {
        private bool DateComparisment(Order o, Order order)
        {
            return o.EndDate != null && o.StartDate != null
                         && ((DateTime.Compare((DateTime)order.StartDate, (DateTime)o.StartDate) >= 0
                         && DateTime.Compare((DateTime)order.StartDate, (DateTime)o.EndDate) <= 0)
                         || (DateTime.Compare((DateTime)order.EndDate, (DateTime)o.StartDate) >= 0
                         && DateTime.Compare((DateTime)order.EndDate, (DateTime)o.EndDate) <= 0));
        }
        public void Process(Order order)
        {
            List<Order> bookings
                         = order.Dress.Orders
                         .Where(o => DateComparisment(o, order))
                         .ToList();
            if (bookings.Count() == 0)
            {
                double days = ((DateTime)order.EndDate - order.StartDate).TotalDays;
                order.Sum = order.Dress.PriceToBook * (decimal)days;
            }

            else throw new Exception("Dress is booked, try to book it later");
        }
    }
    internal class OrderProcessingContext
    {
        private IOrderProcessing _orderProcessing;

        public void SetOrderProcessing(IOrderProcessing orderProcessing)
        {
            _orderProcessing = orderProcessing;
        }

        public void OrderProcess(Order o)
        {
            _orderProcessing.Process(o);
        }
    }
    public class OrderDataManager : IDataManager<Order>
    {
        readonly DressBookingDB1Entities _DressBookingDBContext;
        private OrderProcessingContext _orderProcessingContext = new OrderProcessingContext();
        private BuyOrder buyOrder = new BuyOrder();
        private BookingOrder bookingOrder = new BookingOrder();
        private UserAndDressIdFilter _ordersFilter = new UserAndDressIdFilter();

        private Func<Order, bool> OrderFilter { get { return o => _ordersFilter.Apply(o); } }

        public OrderDataManager(DressBookingDB1Entities context)
        {
            _DressBookingDBContext = context;
        }
        public long Add(Order entity)
        {
            if (entity.Booking) _orderProcessingContext.SetOrderProcessing(bookingOrder);
            else _orderProcessingContext.SetOrderProcessing(buyOrder);
            _orderProcessingContext.OrderProcess(entity);
            entity.Id = DateTime.Now.Ticks;
            _DressBookingDBContext.Orders.Add(entity);
            _DressBookingDBContext.SaveChanges();
            return entity.Id;
        }

        public void Delete(Order entity)
        {
            _DressBookingDBContext.Orders.Remove(entity);
            _DressBookingDBContext.SaveChanges();
        }

        public Order Get(long id)
        {
            var Order = _DressBookingDBContext.Orders.SingleOrDefault(b => b.Id == id);
            if (Order == null) return null;
            _DressBookingDBContext.Entry(Order).Reference(b => b.Dress).Load();
            return Order;
        }

        public IEnumerable<Order> GetAll()
        {
            return _DressBookingDBContext.Orders
                .Include(b => b.Dress)
                .Where(OrderFilter)
                .ToList();
        }
        public IEnumerable<Order> GetAll(FilterState filter)
        {
            SetFilter(filter);
            return _DressBookingDBContext.Orders
               .Include(b => b.Dress)
               .Where(OrderFilter)
               .ToList();
        }

        public IEnumerable<Order> GetAll(FilterState filter, Dictionary<string, string> filters)
        {
            SetFilter(filter);
            AddFilterOptions(filters);
            return _DressBookingDBContext.Orders
               .Include(b => b.Dress)
               .Where(OrderFilter)
               .ToList();
        }

        public void Update( Order entity)
        {
            Order entityToUpdate = _DressBookingDBContext
                .Orders.SingleOrDefault(o => o.Id == entity.Id);
            bool booking = entity.Booking;
            entityToUpdate.Booking = booking;
            entityToUpdate.Dress.IsBought = !booking;
            entityToUpdate.StartDate = entity.StartDate;
            if (booking)
            {
                entityToUpdate.EndDate = entity.EndDate;
            }
            else
            {
                entityToUpdate.EndDate = null;
            }
            _DressBookingDBContext.SaveChanges();
        }

        public bool OrderDressPopulate(Order o)
        {
            o.Dress = _DressBookingDBContext.Dresses.SingleOrDefault(d => d.Id == o.DressId);
            if (o.Dress == null) return false;
            _DressBookingDBContext.Entry(o.Dress).Collection(d => d.Orders).Load();
            return true;
        }

        private void SetFilter(FilterState filter)
        {
            if (filter == FilterState.Administrator) { 
                _ordersFilter.SetUserId("");
                _ordersFilter.SetDressId("");
            }
        }

        private void AddFilterOptions(Dictionary<string, string> filters)
        {
            if (filters.ContainsKey("username"))
                _ordersFilter.SetUserId(filters["username"]);
            if(filters.ContainsKey("dressId"))
                _ordersFilter.SetDressId(filters["dressId"]);
        }

        public void Dispose()
        {
            _DressBookingDBContext.Dispose();
        }
    }

    public class OrderValidationDataManager : IDataManager<Order>
    {
        private OrderDataManager _orderDataManager;

        public OrderValidationDataManager(DressBookingDB1Entities context)
        {
            _orderDataManager = new OrderDataManager(context);
        }

        public OrderValidationDataManager(OrderDataManager orderDataManager)
        {
            _orderDataManager = orderDataManager;
        }

        private bool isValidOrder(Order o)
        {
            string error = "";
            if (_orderDataManager.OrderDressPopulate(o))
            {
                if (o.StartDate == null
                    || DateTime.Compare(DateTime.Now, (DateTime)o.StartDate) > 0)
                    error = "Start date is missed";
                else if (o.Dress.IsBought && !o.Dress.IsCopy) error = "Unable to order bought dress";
                else if (o.Booking
                    && (o.EndDate == null
                    || (DateTime.Compare(DateTime.Now, (DateTime)o.EndDate) > 0)))
                    error = "End date is missed";
                else return true;
            }
            else error = "Dress doesn't exist";
            throw new Exception(error);
        }
        public long Add(Order entity)
        {
            try
            {
                isValidOrder(entity);
                return _orderDataManager.Add(entity);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Delete(Order entity)
        {
            _orderDataManager.Delete(entity);
        }

        public Order Get(long id)
        {
            return _orderDataManager.Get(id);
        }

        public IEnumerable<Order> GetAll()
        {
            return _orderDataManager.GetAll();
        }

        public void Update(Order entity)
        {
            try
            {
                isValidOrder(entity);
                _orderDataManager.Update(entity);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public IEnumerable<Order> GetAll(FilterState filter)
        {
            return _orderDataManager.GetAll(filter);
        }

        public IEnumerable<Order> GetAll(FilterState filter, Dictionary<string, string> filters)
        {
            return _orderDataManager.GetAll(filter, filters);
        }

        public void Dispose()
        {
            _orderDataManager.Dispose();
        }
    }
}