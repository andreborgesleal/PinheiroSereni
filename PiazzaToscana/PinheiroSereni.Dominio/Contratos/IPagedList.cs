using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Web.Mvc
{
    public interface IPagedListObject
    {
        int index { get; set; }
    }

    public class PagedListObject : Object, IPagedListObject
    {
        public int index { get; set; }
    }

    public interface IPagedList
    {
        int TotalCount
        {
            get;
            set;
        }

        int PageIndex
        {
            get;
            set;
        }

        int PageSize
        {
            get;
            set;
        }

        int LastPage 
        { 
            get; 
        }

        bool IsPreviousPage
        {
            get;
        }

        bool IsNextPage
        {
            get;
        }

        string action { get; set; }

        IPagedListObject[] RouteValues
        { 
            get; 
            set;
        }
    }

    public class PagedList<T> : List<T>, IPagedList
    {
        public PagedList(IQueryable<T> source, int index, int pageSize)
        {
            this.TotalCount = source.Count();
            this.PageSize = pageSize;
            this.PageIndex = index > LastPage ? LastPage : index;
            this.AddRange(source.Skip(PageIndex * pageSize).Take(pageSize).ToList());
            this.action = "Browse";

            PagedListObject First = new PagedListObject() { index = 0 };
            PagedListObject Prior = new PagedListObject() { index = (IsPreviousPage ? PageIndex - 1 : PageIndex) };
            PagedListObject Next = new PagedListObject() { index = (IsNextPage ? PageIndex + 1 : PageIndex) };
            PagedListObject Last = new PagedListObject() { index = LastPage };

            PagedListObject[] routeValues = { First, Prior, Next, Last };

            this.RouteValues = routeValues;
        }

        public PagedList(List<T> source, int index, int pageSize)
        {
            this.TotalCount = source.Count();
            this.PageSize = pageSize;
            this.PageIndex = index > LastPage ? LastPage : index;
            this.AddRange(source.Skip(PageIndex * pageSize).Take(pageSize).ToList());
            this.action = "Browse";

            PagedListObject First = new PagedListObject() { index = 0 } ;
            PagedListObject Prior = new PagedListObject() { index = (IsPreviousPage ? PageIndex - 1 : PageIndex)};
            PagedListObject Next = new PagedListObject() { index =(IsNextPage ? PageIndex + 1 : PageIndex) };
            PagedListObject Last = new PagedListObject() { index = LastPage };

            PagedListObject[] routeValues = { First, Prior, Next, Last };

            this.RouteValues = routeValues;
        }

        public PagedList(List<T> source, int index, int pageSize, IPagedListObject[] routeValues)
        {
            this.TotalCount = source.Count();
            this.PageSize = pageSize;
            this.PageIndex = index > LastPage ? LastPage : index;
            this.AddRange(source.Skip(PageIndex * pageSize).Take(pageSize).ToList());
            this.action = "Browse";

            RouteValues = routeValues;

            RouteValues[0].index = 0;
            RouteValues[1].index = (IsPreviousPage ? PageIndex - 1 : PageIndex);
            RouteValues[2].index = (IsNextPage ? PageIndex + 1 : PageIndex);
            RouteValues[3].index = LastPage;
        }

        public PagedList(List<T> source, int index, int pageSize, IPagedListObject[] routeValues, string action)
        {
            this.TotalCount = source.Count();
            this.PageSize = pageSize;
            this.PageIndex = index > LastPage ? LastPage : index;
            this.AddRange(source.Skip(PageIndex * pageSize).Take(pageSize).ToList());
            this.action = action;

            RouteValues = routeValues;

            RouteValues[0].index = 0;
            RouteValues[1].index = (IsPreviousPage ? PageIndex - 1 : PageIndex);
            RouteValues[2].index = (IsNextPage ? PageIndex + 1 : PageIndex);
            RouteValues[3].index = LastPage;
        }

        public string action { get; set; }

        public IPagedListObject[] RouteValues 
        { 
            get; set; 
        }

        public int TotalCount
        {
            get;
            set;
        }

        public int LastPage
        {
            get {
                    int resto = TotalCount % PageSize;
                    int quociente = (TotalCount / PageSize) - 1; 
                    return quociente + (resto > 0 ? 1 : 0);
                }
        }

        public int PageIndex
        {
            get;
            set;
        }

        public int PageSize
        {
            get;
            set;
        }

        public bool IsPreviousPage
        {
            get
            {
                return ((PageIndex - 1) >= 0);
            }
        }

        public bool IsNextPage
        {
            get
            {
                return ((PageIndex + 1) * PageSize) <= TotalCount;
            }
        }
    }

    public static class Pagination
    {
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int index, int pageSize)
        {
            return new PagedList<T>(source, index, pageSize);
        }

        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int index)
        {
            return new PagedList<T>(source, index, 15);
        }
    }
}