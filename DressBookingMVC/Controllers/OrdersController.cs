using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DressBookingMVC.DataManager;
using DressBookingMVC.Helpers;
using DressBookingMVC.Models;
using DressBookingMVC.Models.DressBookingDataModel;

namespace DressBookingMVC.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IDataManager<Order> dataManager;

        public OrdersController(IDataManager<Order> _dataManager)
        {
            dataManager = _dataManager;
        }

        // GET: Orders
        public ActionResult Index()
        {
            if (User.IsInRole(Constants.AdminRoleName))
            {
                return View(dataManager.GetAll(FilterState.Administrator));
            }
            else if (User.IsInRole(Constants.UserRoleName))
            {
                string userName = User.Identity.Name;
                Dictionary<string, string> filters = new Dictionary<string, string>();
                filters.Add("username", userName);
                return View(dataManager.GetAll(FilterState.Customer, filters));
            }
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        // GET: Orders/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = dataManager.Get((long)id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }
        public ActionResult Create(long dressId, string error)
        {
            Dictionary<string, string> filters = new Dictionary<string, string>();
            filters.Add("dressId", dressId.ToString());
            ViewBag.Dates = dataManager.GetAll(FilterState.Customer, filters);
            if (String.IsNullOrEmpty(error))
                ViewBag.Error = "";
            else ViewBag.Error = error;
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DressId,UserId,StartDate,EndDate,Sum,Booking")] Order order)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string login = User.Identity.Name;
                    if (String.IsNullOrEmpty(login)) login = null;
                    order.UserName = login;
                    dataManager.Add(order);
                }
                catch(Exception e)
                {
                    return RedirectToAction("Create", new {error = e.Message, dressId = order.DressId });
                }
                return RedirectToAction("Details", new { id = order.Id });
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = dataManager.Get((long)id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DressId,UserId,StartDate,EndDate,Sum,Booking")] Order order)
        {
            if (ModelState.IsValid)
            {
                dataManager.Update(order);
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = dataManager.Get((long)id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Order order = dataManager.Get((long)id);
            dataManager.Delete(order);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dataManager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
