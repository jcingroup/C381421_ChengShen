using ProcCore.Business.DB0;
using ProcCore.HandleResult;
using ProcCore.Web;
using ProcCore.WebCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace DotWeb.Api
{
    public class GetActionController : BaseApiController
    {
        public GetActionController()
        {

        }
        public IHttpActionResult GetNewsWWW(int? y, int? page)
        {
            #region

            using (db0 = getDB0())
            {
                var items = db0.最新消息
                    .Where(x => x.顯示狀態Flag)
                    .OrderByDescending(x => x.活動日期)
                    .Select(x => new { x.流水號, x.活動日期, x.標題 });

                if (y != null)
                {
                    items = items.Where(x => x.活動日期.Year == y);
                }
                var nowpage = page == null ? 1 : (int)page;
                int startRecord = PageCount.PageInfo(nowpage, 8, items.Count());
                var resultItems = items.Skip(startRecord).Take(8);

                var GridInfo = new
                {
                    rows = resultItems.ToList(),
                    total = PageCount.TotalPage,
                    page = PageCount.Page,
                    records = PageCount.RecordCount,
                    startcount = PageCount.StartCount,
                    endcount = PageCount.EndCount
                };

                return Ok(GridInfo);
            }
            #endregion
        }
        public IHttpActionResult GetActiveWWW(int? page)
        {
            #region

            using (db0 = getDB0())
            {
                var items = db0.活動花絮主檔
                    .Where(x => x.顯示狀態Flag)
                    .OrderByDescending(x => x.活動日期)
                    .Select(x => new { x.流水號, x.活動日期, x.標題 });

                var nowpage = page == null ? 1 : (int)page;
                int startRecord = PageCount.PageInfo(nowpage, 6, items.Count());
                var resultItems = items.Skip(startRecord).Take(6);

                var GridInfo = new
                {
                    rows = resultItems.ToList(),
                    total = PageCount.TotalPage,
                    page = PageCount.Page,
                    records = PageCount.RecordCount,
                    startcount = PageCount.StartCount,
                    endcount = PageCount.EndCount
                };

                return Ok(GridInfo);
            }
            #endregion
        }
        public IHttpActionResult GetShareWWW(int? page)
        {
            #region

            using (db0 = getDB0())
            {
                var items = db0.文件管理
                    .Where(x => x.顯示狀態Flag && x.分類==1)
                    .OrderByDescending(x => x.活動日期)
                    .Select(x => new { x.流水號, x.活動日期, x.標題 });

                var nowpage = page == null ? 1 : (int)page;
                int startRecord = PageCount.PageInfo(nowpage, 6, items.Count());
                var resultItems = items.Skip(startRecord).Take(6);

                var GridInfo = new
                {
                    rows = resultItems.ToList(),
                    total = PageCount.TotalPage,
                    page = PageCount.Page,
                    records = PageCount.RecordCount,
                    startcount = PageCount.StartCount,
                    endcount = PageCount.EndCount
                };

                return Ok(GridInfo);
            }
            #endregion
        }
        public IHttpActionResult GetMeetingWWW(int? page)
        {
            #region

            using (db0 = getDB0())
            {
                var items = db0.文件管理
                    .Where(x => x.顯示狀態Flag && x.分類 == 3)
                    .OrderByDescending(x => x.活動日期)
                    .Select(x => new { x.流水號, x.活動日期, x.標題 });

                var nowpage = page == null ? 1 : (int)page;
                int startRecord = PageCount.PageInfo(nowpage, 6, items.Count());
                var resultItems = items.Skip(startRecord).Take(6);

                var GridInfo = new
                {
                    rows = resultItems.ToList(),
                    total = PageCount.TotalPage,
                    page = PageCount.Page,
                    records = PageCount.RecordCount,
                    startcount = PageCount.StartCount,
                    endcount = PageCount.EndCount
                };

                return Ok(GridInfo);
            }
            #endregion
        }
        public IHttpActionResult GetDocWWW(int? page)
        {
            #region

            using (db0 = getDB0())
            {
                var items = db0.文件管理
                    .Where(x => x.顯示狀態Flag && x.分類 == 4)
                    .OrderByDescending(x => x.活動日期)
                    .Select(x => new { x.流水號, x.活動日期, x.標題 });

                var nowpage = page == null ? 1 : (int)page;
                int startRecord = PageCount.PageInfo(nowpage, 6, items.Count());
                var resultItems = items.Skip(startRecord).Take(6);

                var GridInfo = new
                {
                    rows = resultItems.ToList(),
                    total = PageCount.TotalPage,
                    page = PageCount.Page,
                    records = PageCount.RecordCount,
                    startcount = PageCount.StartCount,
                    endcount = PageCount.EndCount
                };

                return Ok(GridInfo);
            }
            #endregion
        }
        public IHttpActionResult GetNewsContentWWW(int id)
        {
            #region

            using (db0 = getDB0())
            {
                var items = db0.最新消息.Find(id);
                return Ok(items);
            }
            #endregion
        }
    }
}
