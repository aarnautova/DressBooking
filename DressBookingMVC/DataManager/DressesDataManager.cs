using DressBookingMVC.DataManager.Filter;
using DressBookingMVC.Models.DressBookingDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using DressBookingMVC.DataManager.Atelier;

namespace DressBookingMVC.DataManager
{
    public class DressesDataManager : IDataManager<Dress>
    {
        readonly DressBookingDB1Entities _DressBookingDBContext;
        private DressesStorage _ds;
        private DressesFilter _currentFilter = new OriginalsFilter();
        private FilterFactory _filterFactory;

        private StocktakingFilter stocktaking;
        private OriginalsFilter originals;
        private Func<Dress, bool> CurrentFilter { get { return d => _currentFilter.Apply(d); } }


        public DressesDataManager(DressBookingDB1Entities context)
        {
            _DressBookingDBContext = context;
            _ds = DressesStorage.Instance(this);
            _filterFactory = FilterFactory.Instance();
            originals = new OriginalsFilter();
            stocktaking = new StocktakingFilter();
        }

        public long Add(Dress entity)
        {
            entity.Id = DateTime.Now.Ticks;
            _DressBookingDBContext.Dresses.Add(entity);
            if (!entity.IsCopy)
            {
                _ds = DressesStorage.Instance(this);
                _ds.AddDressToCache(entity);
            }
            _DressBookingDBContext.SaveChanges();
            return entity.Id;
        }

        public void Delete(Dress entity)
        {
            _DressBookingDBContext.Dresses.Remove(entity);
            _DressBookingDBContext.SaveChanges();
            _ds.DeleteDressFromCache(entity.Id);
            
        }

        public Dress Get(long id)
        {
            return _DressBookingDBContext.Dresses.SingleOrDefault(d => d.Id == id);
        }

        public IEnumerable<Dress> GetAll()
        {
            return _DressBookingDBContext.Dresses
                .Include(d => d.Size)
                .Where(CurrentFilter)
                .ToList();
        }

        public IEnumerable<Dress> GetAll(FilterState filter)
        {
            SetFilter(filter);
            return _DressBookingDBContext.Dresses
                .Include(d => d.Size)
                .Where(CurrentFilter)
                .ToList();
        }

        public IEnumerable<Dress> GetAll(FilterState filter, Dictionary<string, string> filters)
        {
            SetFilter(filter);
            AddFilterOptions(filters);
            return _DressBookingDBContext.Dresses
                .Include(d => d.Size)
                .Where(CurrentFilter)
                .ToList();
        }
        public void Update(Dress entity)
        {
            Dress entityToUpdate = _DressBookingDBContext
                .Dresses.SingleOrDefault(d => d.Id == entity.Id);
            entityToUpdate.IsBought = entity.IsBought;
            entityToUpdate.IsCopy = entity.IsCopy;
            entityToUpdate.Name = entity.Name;
            entityToUpdate.PriceToBook = entity.PriceToBook;
            entityToUpdate.PriceToBuy = entity.PriceToBuy;
            entityToUpdate.SizeId = entity.SizeId;
            entityToUpdate.Wedding = entity.Wedding;
            _DressBookingDBContext.SaveChanges();
        }

        private void SetFilter(FilterState filter)
        {
            if (filter == FilterState.Administrator)
                _currentFilter = stocktaking;
            else
                _currentFilter = originals;
        }

        private void AddFilterOptions(Dictionary<string, string> filters)
        {
            bool? weddingBool = null;
            bool? boughtBool = null;
            if (filters["wedding"] == "true") weddingBool = true;
            else if (filters["wedding"] == "false") weddingBool = false;
            if (filters["bought"] == "true") boughtBool = true;
            else if (filters["bought"] == "false") boughtBool = false;
            FormulateDecorators(weddingBool, boughtBool, filters["search"], filters["size"]);
        }
        private void FormulateDecorators(bool? wedding, bool? bought, string search, string size)
        {
            StateFilter stf = null;
            if (wedding != null) FormulateStateDecorator(stf, DressesFilterName.IsWeddingFilter, (bool)wedding);
            if (bought != null) FormulateStateDecorator(stf, DressesFilterName.IsBoughtFilter, (bool)bought);
            StringFilter strf = null;
            if (!String.IsNullOrEmpty(search)) FormulateStringDecorator(strf, DressesFilterName.SearchFilter, search);
            if (!String.IsNullOrEmpty(size)) FormulateStringDecorator(strf, DressesFilterName.SizeFilter, size);
        }

        private void FormulateStateDecorator(StateFilter sf, DressesFilterName dn, bool state)
        {
            sf = _filterFactory.GetStateFilter(dn);
            sf.SetState(state);
            Decorate(sf);
        }

        private void FormulateStringDecorator(StringFilter sf, DressesFilterName dn, string parameter)
        {
            sf = _filterFactory.GetStringFilter(dn);
            sf.SetString(parameter);
            Decorate(sf);
        }

        private void Decorate(DecoratorFilter df)
        {
            df.SetFilter(_currentFilter);
            _currentFilter = df;
        }
        public void Dispose()
        {
            _DressBookingDBContext.Dispose();
        }

        public Dress GetCopy(long id)
        {
            Dress d = Get(id);
            return (Dress)_DressBookingDBContext.Entry(d).CurrentValues.ToObject();
        }
    }

}
