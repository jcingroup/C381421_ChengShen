using ProcCore.Business.DB0;
using ProcCore.HandleResult;
using ProcCore.Web;
using ProcCore.WebCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace DotWeb.Api
{
    public class ProductTypeController : ajaxApi<ProductType, q_ProductType>
    {
        public async Task<IHttpActionResult> Get(int id)
        {
            using (db0 = getDB0())
            {
                item = await db0.ProductType.FindAsync(id);
                r = new rAjaxGetData<ProductType>() { data = item };
            }

            return Ok(r);
        }
        public IHttpActionResult Get([FromUri]q_ProductType q)
        {
            #region 連接BusinessLogicLibary資料庫並取得資料

            using (db0 = getDB0())
            {
                var items = (from x in db0.ProductType 
                             orderby x.is_second,x.sort descending
                             select new m_ProductType()
                             {
                                 id = x.id,
                                 type_name = x.type_name,
                                 is_second=x.is_second,
                                 sort = x.sort,
                                 memo = x.memo
                             });

                if (q.name != null)
                    items = items.Where(x =>x.type_name.Contains(q.name));


                if (q.is_second != null)
                    items = items.Where(x => x.is_second==q.is_second);

                //if (q.user_id != null)
                //    items = items.Where(x => x.USERID == q.user_id);

                int PageSize = q.PageSize == null ? this.defPageSize : (int)q.PageSize;
                int page = (q.page == null ? 1 : (int)q.page);
                int startRecord = PageCount.PageInfo(page, PageSize, items.Count());//this.defPageSize

                var resultItems = items.Skip(startRecord).Take(PageSize).ToList();

                return Ok(new GridInfo2<m_ProductType>()
                {
                    rows = resultItems,
                    total = PageCount.TotalPage,
                    page = PageCount.Page,
                    records = PageCount.RecordCount,
                    startcount = PageCount.StartCount,
                    endcount = PageCount.EndCount
                });
            }
            #endregion
        }
        public async Task<IHttpActionResult> Put([FromBody]ProductType md)
        {
            ResultInfo rAjaxResult = new ResultInfo();
            try
            {
                db0 = getDB0();

                item = await db0.ProductType.FindAsync(md.id);
                item.type_name = md.type_name;
                item.is_second = md.is_second;
                item.sort = md.sort;
                item.memo = md.memo;
                item.i_Hide = md.i_Hide;
                item.i_UpdateDateTime = DateTime.Now;

                await db0.SaveChangesAsync();
                rAjaxResult.result = true;
            }
            catch (Exception ex)
            {
                rAjaxResult.result = false;
                rAjaxResult.message = ex.ToString();
            }
            finally
            {
                db0.Dispose();
            }
            return Ok(rAjaxResult);
        }
        public async Task<IHttpActionResult> Post([FromBody]ProductType md)
        {
            ResultInfo rAjaxResult = new ResultInfo();

            if (!ModelState.IsValid)
            {
                rAjaxResult.message = ModelStateErrorPack();
                rAjaxResult.result = false;
                return Ok(rAjaxResult);
            }

            try
            {
                #region working a
                db0 = getDB0();
                md.i_InsertDateTime = DateTime.Now;
                md.i_Lang = "zh-TW";

                db0.ProductType.Add(md);
                await db0.SaveChangesAsync();

                rAjaxResult.result = true;
                rAjaxResult.id = md.id;
                return Ok(rAjaxResult);
                #endregion
            }
            catch (Exception ex)
            {
                rAjaxResult.result = false;
                rAjaxResult.message = ex.Message;
                return Ok(rAjaxResult);
            }
            finally
            {
                db0.Dispose();
            }
        }
        public async Task<IHttpActionResult> Delete([FromUri]int[] ids)
        {
            ResultInfo rAjaxResult = new ResultInfo();
            try
            {
                db0 = getDB0();

                foreach (var id in ids)
                {
                    Boolean isExist = db0.ProductData.Any(x => x.type_id == id);
                    if (isExist)
                    {
                        rAjaxResult.message = "請先刪除底層產品資料,再刪除此產品分類!";
                        rAjaxResult.result = false;
                        return Ok(rAjaxResult);
                    }
                    item = new ProductType() { id = id };
                    db0.ProductType.Attach(item);
                    db0.ProductType.Remove(item);
                }

                await db0.SaveChangesAsync();

                rAjaxResult.result = true;
                return Ok(rAjaxResult);
            }
            catch (Exception ex)
            {
                rAjaxResult.result = false;
                rAjaxResult.message = ex.Message;
                return Ok(rAjaxResult);
            }
            finally
            {
                db0.Dispose();
            }
        }
    }
}
