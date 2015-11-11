using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;


namespace ProcCore.Business.DB0
{
    public partial class C38A0_ChengShenEntities : DbContext
    {
        public C38A0_ChengShenEntities(string connectionstring)
            : base(connectionstring)
        {
        }

        public override Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }
        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                Log.Write(ex.Message, ex.StackTrace);
                foreach (var err_Items in ex.EntityValidationErrors)
                {
                    foreach (var err_Item in err_Items.ValidationErrors)
                    {
                        Log.Write("欄位驗證錯誤", err_Item.PropertyName, err_Item.ErrorMessage);
                    }
                }

                throw ex;
            }
            catch (DbUpdateException ex)
            {
                Log.Write("DbUpdateException", ex.InnerException.Message);
                throw ex;
            }
            catch (EntityException ex)
            {
                Log.Write("EntityException", ex.Message);
                throw ex;
            }
            catch (UpdateException ex)
            {
                Log.Write("UpdateException", ex.Message);
                throw ex;
            }
            catch (Exception ex)
            {
                 Log.Write("Exception", ex.Message);
                throw ex;
            }
        }

    }

    #region Model Expand
    public partial class m_ProductType : BaseEntityTable
    {
        public List<m_ProductData> ProductData { get; set; }
    }
    public partial class m_ProductData : BaseEntityTable
    {
        public string type_name { get; set; }
        public string img { get; set; }
        public Boolean is_second { get; set; }
    }
    public partial class ProductData : BaseEntityTable
    {
        public string type_name { get; set; }
        public string img { get; set; }
        public List<imgs> imgs { get; set; }
        public Boolean is_second { get; set; }
    }
    public class parm
    {
        public string receiveMails { get; set; }
        public string BccMails { get; set; }
    }
    public class imgs 
    {
        public string small_size { get; set; }

        public string big_size { get; set; }
    }
    #endregion

    #region q_Model_Define
    public class q_AspNetRoles : QueryBase
    {
        public string Name { set; get; }

    }
    public class q_AspNetUsers : QueryBase
    {
        public string UserName { set; get; }

    }
    public class q_ProductData : QueryBase
    {
        public string type_name { set; get; }
        public string name { get; set; }
        public int type_id { get; set; }
        public Boolean? is_second { get; set; }
    }
    public class q_ProductType : QueryBase
    {
        public string name { set; get; }
        public Boolean? is_second { get; set; }

    }
    #endregion

    #region c_Model_Define
    public class c_Product
    {
        public q_ProductType q { get; set; }
        public q_ProductData qs { get; set; }
        public ProductType m { get; set; }
        public ProductData ms { get; set; }
    }

    public class c_ProductType
    {
        public q_ProductType q { get; set; }

        public ProductType m { get; set; }
    }
    public class c_ProductData
    {
        public q_ProductData q { get; set; }

        public ProductData m { get; set; }
    }
    public class c_AspNetRoles
    {
        public q_AspNetRoles q { get; set; }
        public AspNetRoles m { get; set; }
    }
    public partial class c_AspNetUsers
    {
        public q_AspNetUsers q { get; set; }
        public AspNetUsers m { get; set; }
    }

    #endregion
}
