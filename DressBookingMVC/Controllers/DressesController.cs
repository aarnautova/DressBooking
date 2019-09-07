using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DressBookingMVC.DataManager;
using DressBookingMVC.DataManager.Atelier;
using DressBookingMVC.Helpers;
using DressBookingMVC.Models;
using DressBookingMVC.Models.DressBookingDataModel;

namespace DressBookingMVC.Controllers
{
    public class DressesController : Controller
    {
        private readonly IDataManager<Dress> dataManager;
        private readonly AtelierFacade atelier;

        public DressesController(IDataManager<Dress> _dataManager, AtelierFacade _atelier)
        {
            dataManager = _dataManager;
            atelier = _atelier;
        }

        // GET: Dresses
        public ActionResult Index()
        {
            FilterState state;
            if (User.IsInRole(Constants.AdminRoleName))
                state = FilterState.Administrator;
            else state = FilterState.Customer;
            string weddingFilter = "";
            string boughtFilter = "false";
            if (Request["IsBought"] == "on")
                boughtFilter = "true";
            if (Request["Wedding"] == "on" && Request["Gown"] == null)
                weddingFilter = "true";
            else if (Request["Wedding"] == null && Request["Gown"] == "on")
                weddingFilter = "false";
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("wedding", weddingFilter);
            dict.Add("bought", boughtFilter);
            dict.Add("size", Request["Size"]);
            dict.Add("search", Request["Search"]);
            var dresses = dataManager.GetAll(state, dict);
            return View(dresses);
        }

        // GET: Dresses/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dress dress = dataManager.Get((long)id);
            if (dress == null)
            {
                return HttpNotFound();
            }
            var items = Constants.sizes.Select((s, index)
                            => new SelectListItem { Text = s, Value = index.ToString() });
            ViewBag.SizeId = new SelectList(items, "Value", "Text", dress.SizeId);
            return View(dress);
        }

        // GET: Dresses/Create
        public ActionResult Create(string error)
        {
            if(!User.IsInRole(Constants.AdminRoleName))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            var items = Constants.sizes.Select((s, index)
                            => new SelectListItem { Text = s, Value = index.ToString() });
            ViewBag.SizeId = new SelectList(items, "Value", "Text");
            if (String.IsNullOrEmpty(error))
                ViewBag.Error = "";
            else ViewBag.Error = error;
            return View();
        }

        // POST: Dresses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,SizeId,PriceToBuy,PriceToBook,IsBought,IsCopy,Wedding,FrontPicLink,CloserPicLink,BackPicLink")] Dress dress)
        {
            if (!User.IsInRole(Constants.AdminRoleName))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            if (ModelState.IsValid)
            {
                try
                {
                    dataManager.Add(dress);
                    return RedirectToAction("Index");
                } catch(Exception e)
                {
                    return RedirectToAction("Create", new { error = e.Message});
                }
            }
            var items = Constants.sizes.Select((s, index)
                            => new SelectListItem { Text = s, Value = index.ToString() });
            ViewBag.SizeId = new SelectList(items, "Value", "Text", dress.SizeId);
            return View(dress);
        }

        // GET: Dresses/Edit/5
        public ActionResult Edit(long? id)
        {
            if (!User.IsInRole(Constants.AdminRoleName))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dress dress = dataManager.Get((long)id);
            if (dress == null)
            {
                return HttpNotFound();
            }
             var items = Constants.sizes.Select((s, index)
                => new SelectListItem { Text = s, Value = index.ToString()});
            ViewBag.SizeId = new SelectList(items, "Value", "Text", dress.SizeId);
            return View(dress);
        }

        // POST: Dresses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,SizeId,PriceToBuy,PriceToBook,IsBought,IsCopy,Wedding,FrontPicLink,CloserPicLink,BackPicLink")] Dress dress)
        {
            if (!User.IsInRole(Constants.AdminRoleName))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            if (ModelState.IsValid)
            {
                dataManager.Update(dress);
                return RedirectToAction("Index");
            }
            var items = Constants.sizes.Select((s, index)
                => new SelectListItem { Text = s, Value = index.ToString() });
            ViewBag.SizeId = items;
            return View(dress);
        }

        // GET: Dresses/Delete/5
        public ActionResult Delete(long? id)
        {
            if (!User.IsInRole(Constants.AdminRoleName))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dress dress = dataManager.Get((long)id);
            if (dress == null)
            {
                return HttpNotFound();
            }
            return View(dress);
        }

        // POST: Dresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            if (!User.IsInRole(Constants.AdminRoleName))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            Dress dress = dataManager.Get((long)id);
            dataManager.Delete(dress);
            return RedirectToAction("Index");
        }


        public ActionResult CreateNewOrder(long? id)
        {
            return RedirectToAction("Create", "Orders", new { dressId = id });
        }

        public ActionResult CreateAtelierCopy(long? id)
        {
            int size = Convert.ToInt32(Request["SizeId"]);
            try
            {
                string login = User.Identity.Name;
                if (String.IsNullOrEmpty(login)) login = null; 
                Dress d = atelier.SewDress((long)id, size, login);
                if (d != null)
                    return RedirectToAction("Details", "Dresses", new { id = d.Id });
                return RedirectToAction("Details", "Dresses", new { id = d.Id, error = "Something went wrong" });
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, e.Message);
            }
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
