using DressBookingMVC.Helpers;
using DressBookingMVC.Models.DressBookingDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DressBookingMVC.DataManager.Filter
{
    public abstract class DressesFilter : IFilter<Dress>
    {
        public abstract bool Apply(Dress entity);
    }
    public class StocktakingFilter : DressesFilter
    {
        public override bool Apply(Dress d) => true;
    }
    public class OriginalsFilter : DressesFilter
    {
        public override bool Apply(Dress d)
        {
            return !d.IsCopy;
        }
    }


    public abstract class DecoratorFilter : DressesFilter
    {
        protected DressesFilter _f;

        public void SetFilter(DressesFilter filter)
        {
            _f = filter;
        }

        public override bool Apply(Dress d)
        {
            return _f.Apply(d);
        }
    }

    public abstract class StateFilter : DecoratorFilter
    {
        protected bool _state;

        public void SetState(bool state)
        {
            _state = state;
        }
    }
    public class IsWeddingFilter : StateFilter
    {
        public override bool Apply(Dress d)
        {
            return base.Apply(d) && (d.Wedding == _state);
        }
    }

    public class IsBoughtFilter : StateFilter
    {
        public override bool Apply(Dress d)
        {
            return base.Apply(d) && (d.IsBought == _state);
        }
    }

    public abstract class StringFilter : DecoratorFilter
    {
        protected string _parameter;

        public void SetString(string parameter)
        {
            _parameter = parameter;
        }
    }
    public class SizeFilter : StringFilter
    {
        
        public override bool Apply(Dress d)
        {
            int sizeId = Array.IndexOf(Constants.sizes, _parameter.ToUpper());
            if (sizeId < 0) return base.Apply(d);
            return base.Apply(d) && (d.SizeId == sizeId);
        }
    }
    public class SearchFilter : StringFilter
    {

        public override bool Apply(Dress d)
        {
            if (String.IsNullOrEmpty(_parameter)) return base.Apply(d);
            return base.Apply(d) && d.Name.Contains(_parameter);
        }
    }
    public enum DressesFilterName
    {
        SearchFilter,
        IsWeddingFilter,
        IsBoughtFilter,
        IsCopyFilter,
        SizeFilter
    }
    class FilterFactory
    {
        private static FilterFactory _instance;

        private Dictionary<DressesFilterName, StateFilter> _stateFilters
                    = new Dictionary<DressesFilterName, StateFilter>();
        private Dictionary<DressesFilterName, StringFilter> _stringFilters
                   = new Dictionary<DressesFilterName, StringFilter>();

        protected FilterFactory() { }

        public static FilterFactory Instance()
        {
            if (_instance == null) _instance = new FilterFactory();
            return _instance;
        }
        public StateFilter GetStateFilter(DressesFilterName key)
        {
            StateFilter filter = null;
            if (_stateFilters.ContainsKey(key))
            {
                filter = _stateFilters[key];
            }
            else
            {
                switch (key)
                {
                    case DressesFilterName.IsWeddingFilter:
                        filter = new IsWeddingFilter(); break;
                    case DressesFilterName.IsBoughtFilter:
                        filter = new IsBoughtFilter(); break;
                    default: throw new Exception("Wrong key");

                }
                _stateFilters.Add(key, filter);
            }
            return filter;
        }

        public StringFilter GetStringFilter(DressesFilterName key)
        {
            StringFilter filter = null;
            if (_stringFilters.ContainsKey(key))
            {
                filter = _stringFilters[key];
            }
            else
            {
                switch (key)
                {
                    case DressesFilterName.SearchFilter:
                        filter = new SearchFilter(); break;
                    case DressesFilterName.SizeFilter:
                        filter = new SizeFilter(); break;
                    default: throw new Exception("Wrong key");

                }
                _stringFilters.Add(key, filter);
            }
            return filter;
        }
    }
}
