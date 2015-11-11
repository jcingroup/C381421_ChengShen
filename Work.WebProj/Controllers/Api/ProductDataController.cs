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
    public class ProductDataController : ajaxApi<ProductData, q_ProductData>
    {
        public async Task<IHttpActionResult> Get(int id)
        {
            using (db0 = getDB0())
            {
                item = await db0.ProductData.FindAsync(id);
                item.is_second = db0.ProductType.Where(x => x.id == item.type_id).First().is_second;
                r = new rAjaxGetData<ProductData>() { data = item };
            }

            return Ok(r);
        }
        public IHttpActionResult Get([FromUri]q_ProductData q)
        {
            #region 連接BusinessLogicLibary資料庫並取得資料
            using (db0 = getDB0())
            {
                var items = (from x in db0.ProductData
                             orderby x.sort descending
                             select new m_ProductData()
                             {
                                 id = x.id,
                                 product_name = x.product_name,
                                 sort = x.sort,
                                 type_name = x.ProductType.type_name,
                                 type_id = x.ProductType.id,
                                 is_second = x.ProductType.is_second
                             });

                if (q.name != null)
                    items = items.Where(x => x.product_name.Contains(q.name));

                if (q.is_second != null)
                    items = items.Where(x => x.is_second == q.is_second);

                if (q.type_id != 0)
                    items = items.Where(x => x.type_id == q.type_id);

                int PageSize = q.PageSize == null ? this.defPageSize : (int)q.PageSize;
                int page = (q.page == null ? 1 : (int)q.page);
                int startRecord = PageCount.PageInfo(page, PageSize, items.Count());//this.defPageSize

                var resultItems = items.Skip(startRecord).Take(PageSize).ToList();

                return Ok(new GridInfo2<m_ProductData>()
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

        public async Task<IHttpActionResult> Put([FromBody]ProductData md)
        {
            ResultInfo rAjaxResult = new ResultInfo();
            try
            {
                db0 = getDB0();
                item = await db0.ProductData.FindAsync(md.id);
                item.product_name = md.product_name;
                item.sort = md.sort;
                item.type_id = md.type_id;
                item.supporting_capacity = md.supporting_capacity;
                item.engine = md.engine;
                item.i_Hide = md.i_Hide;
                if (md.introduction != null)
                    item.introduction = RemoveScriptTag(md.introduction);
                if (md.memo != null)
                    item.memo = RemoveScriptTag(md.memo);
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
        public async Task<IHttpActionResult> Post([FromBody]ProductData md)
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
                db0.ProductData.Add(md);
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
                    item = new ProductData() { id = id };
                    db0.ProductData.Attach(item);
                    db0.ProductData.Remove(item);
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
