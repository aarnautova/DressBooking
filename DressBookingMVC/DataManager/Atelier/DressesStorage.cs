using DressBookingMVC.Models.DressBookingDataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DressBookingMVC.DataManager.Atelier
{
    
    public class DressesStorage
    {
        private static DressesStorage _instance;
        private Hashtable dressMap = new Hashtable();
        public DressesDataManager DataManager;
        protected DressesStorage()
        {
        }

        public static DressesStorage Instance(DressesDataManager DressDataManager)
        {
            if (_instance == null)
            {
                _instance = new DressesStorage();
                _instance.DataManager = DressDataManager;
                _instance.LoadCache();
            }
                _instance.DataManager = DressDataManager;
            return _instance;
        }


        public Dress GetDressPrototype(long key)
        {
            Dress cachedDress = (Dress)dressMap[key];
            if (cachedDress == null) throw new Exception("Wrong key");
            return (Dress)DataManager.GetCopy(cachedDress.Id);
        }

        public void AddDressToCache(Dress dress)
        {
           dressMap[dress.Id] = dress;
        }
        public void DeleteDressFromCache(long id)
        {
            dressMap.Remove(id);
        }

        public void UpdateDressFromCache(Dress dress)
        {
            dressMap[dress.Id] = dress;
        }
        public void LoadCache()
        {
            foreach (Dress d in this.DataManager.GetAll())
            {
                AddDressToCache(d);
            }
        }
    }
}