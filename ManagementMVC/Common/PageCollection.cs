using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class PageCollection
    {
        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }

        public int OnePageSize { get; set; }

        public long TotalRows { get; set; }

        public string OrderBy { get; set; }

        public PageCollection()
        {
            this.CurrentPage = 0;
            this.OnePageSize = 15;
        }

        /// <summary>
        /// 分页逻辑处理类 linq to entites
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        public class PageInfo<TEntity> where TEntity : class
        {
            public int Index { get; private set; }
            public int PageSize { get; private set; }
            public int Count { get; private set; }
            public List<TEntity> List { get; set; }

            public string Url { get; set; }

            public int BeginPage { get; private set; }
            public int EndPage { get; private set; }

            public PageInfo(int index, int pagesize, int count, List<TEntity> list, string url = "")
            {
                Index = index;
                PageSize = pagesize;
                Count = count;
                List = list;
                Url = url;

                if (count == 0)
                {
                    BeginPage = 0;
                    EndPage = 0;
                }
                else
                {
                    int maxpage = count/pagesize;

                    if (count%pagesize > 0)
                    {
                        maxpage++;
                    }
                    if (index >= maxpage)
                    {
                        index = maxpage;

                        BeginPage = pagesize*index - pagesize + 1;
                        EndPage = count;
                    }
                    else
                    {
                        BeginPage = pagesize*index - pagesize + 1;
                        EndPage = pagesize*index;
                    }
                }
            }
        }

        /// <summary>
        /// 分页逻辑处理类 dynamic
        /// </summary>
        public class PageInfo
        {
            public int Index { get; private set; }
            public int PageSize { get; private set; }
            public int Count { get; private set; }
            public dynamic List { get; set; }

            public string Url { get; set; }

            public int BeginPage { get; private set; }
            public int EndPage { get; private set; }

            public PageInfo(int index, int pagesize, int count, dynamic list, string url = "")
            {
                Index = index;
                PageSize = pagesize;
                Count = count;
                List = list;
                Url = url;

                if (count == 0)
                {
                    BeginPage = 0;
                    EndPage = 0;
                }
                else
                {
                    int maxpage = count/pagesize;

                    if (count%pagesize > 0)
                    {
                        maxpage++;
                    }
                    if (index >= maxpage)
                    {
                        index = maxpage;

                        BeginPage = pagesize*index - pagesize + 1;
                        EndPage = count;
                    }
                    else
                    {
                        BeginPage = pagesize*index - pagesize + 1;
                        EndPage = pagesize*index;
                    }
                }
            }
        }

        /// <summary>
        /// Eazyui分页处理逻辑类
        /// </summary>
        public class PageEazyUI
        {
            public int page { get; private set; }
            public int pagesize { get; private set; }
            public int total { get; private set; }
            public object rows { get; private set; }
            public PageEazyUI(int _page, int _pagesize, int _total, object _rows)
            {
                page = _page;
                pagesize = _pagesize;
                total = _total;
                rows = _rows;
            }
        }
    }
}
