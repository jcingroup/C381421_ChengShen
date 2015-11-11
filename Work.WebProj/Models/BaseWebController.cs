using DotWeb.CommSetup;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using ProcCore;
using ProcCore.Business;
using ProcCore.Business.DB0;
using ProcCore.Business.LogicConect;
using ProcCore.HandleResult;
using ProcCore.NetExtension;
using ProcCore.WebCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace DotWeb
{
    #region 基底控制器
    public abstract class SourceController : System.Web.Mvc.Controller
    {
        //protected string IP;
        protected C38A0_ChengShenEntities db0;
        protected virtual string getRecMessage(string MsgId)
        {
            String r = Resources.Res.ResourceManager.GetString(MsgId);
            return String.IsNullOrEmpty(r) ? MsgId : r;
        }
        protected virtual string getRecMessage(List<i_Code> codeSheet, string code)
        {

            var c = codeSheet.Where(x => x.Code == code).FirstOrDefault();

            if (c == null)
                return code;
            else
            {
                string r = Resources.Res.ResourceManager.GetString(c.LangCode);
                return string.IsNullOrEmpty(r) ? c.Value : r;
            }
        }
        protected string defJSON(object o)
        {
            return JsonConvert.SerializeObject(o, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }
        protected TransactionScope defAsyncScope()
        {
            return new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        }
        protected virtual LogicCenter openLogic()
        {
            LogicCenter dbLogic = new LogicCenter(CommSetup.CommWebSetup.DB0_CodeString);

            dbLogic.IP = System.Web.HttpContext.Current.Request.UserHostAddress;

            return dbLogic;
        }
        protected string getNowLnag()
        {
            return System.Globalization.CultureInfo.CurrentCulture.Name;
        }
        protected static C38A0_ChengShenEntities getDB0()
        {
            LogicCenter.SetDB0EntityString(CommSetup.CommWebSetup.DB0_CodeString);
            return LogicCenter.getDB0;
        }
    }
    [Authorize]
    public abstract class BaseController : SourceController
    {
        protected string UserId; //指的是廠商登錄帳號
        protected string aspUserId;
        protected int departmentId;

        protected int defPageSize = 0;

        //訂義取得本次執行 Controller Area Action名稱
        protected string getController = string.Empty;
        protected string getArea = string.Empty;
        protected string getAction = string.Empty;

        //訂義檔案上傳路行樣板
        protected string sysUpFilePathTpl = "~/_Code/SysUpFiles/{0}.{1}/{2}/{3}/{4}";

        //訂義檔案刪除路徑樣板
        protected string sysDelSysId = "~/_Code/SysUpFiles/{0}.{1}/{2}";

        //系統認可圖片檔副檔名
        protected string[] imgExtDef = new string[] { ".jpg", ".jpeg", ".gif", ".png", ".bmp" };

        //protected Log.LogPlamInfo plamInfo = new Log.LogPlamInfo() { AllowWrite = true };
        private ApplicationUserManager _userManager;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            var getUserIdCookie = Request.Cookies["user_id"];
            //var getUserName = Request.Cookies["user_name"];

            var getUserName = Request.Cookies["user_name"];

            UserId = getUserIdCookie == null ? null : getUserIdCookie.Value;

            ViewBag.UserId = UserId;
            ViewBag.UserName = getUserName == null ? "工業技術研究院" : Server.UrlDecode(getUserName.Value);
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public RoleManager<IdentityRole> roleManager
        {
            get
            {
                ApplicationDbContext context = ApplicationDbContext.Create();
                return new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            }
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            this.aspUserId = User.Identity.GetUserId();
            this.departmentId = int.Parse(Request.Cookies[CommWebSetup.Cookie_DepartmentId].Value);

            Log.SetupBasePath = System.Web.HttpContext.Current.Server.MapPath("~\\_Code\\Log\\");
            Log.Enabled = true;

            //plamInfo.BroswerInfo = System.Web.HttpContext.Current.Request.Browser.Browser + "." + System.Web.HttpContext.Current.Request.Browser.Version;
            //plamInfo.IP = this.IP;

            //plamInfo.UnitId = departmentId;

            defPageSize = CommSetup.CommWebSetup.MasterGridDefPageSize;
            this.getController = ControllerContext.RouteData.Values["controller"].ToString();
            this.getArea = ControllerContext.RouteData.DataTokens["area"].ToString();
            this.getAction = ControllerContext.RouteData.Values["action"].ToString();
        }
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            Log.WriteToFile();
        }
        protected void ActionRun()
        {
            ViewBag.area = this.getArea;
            ViewBag.controller = this.getController;
            using (db0 = getDB0())
            {
                ViewBag.Langs = db0.i_Lang.Where(x => x.isuse == true).OrderBy(x => x.sort).ToList();
            }
        }
        public int GetNewId()
        {
            return GetNewId(ProcCore.Business.CodeTable.Base);
        }
        public int GetNewId(ProcCore.Business.CodeTable tab)
        {

            //using (TransactionScope tx = new TransactionScope())
            //{
            var db = getDB0();
            try
            {
                string tab_name = Enum.GetName(typeof(ProcCore.Business.CodeTable), tab);
                var items = db.i_IDX.Where(x => x.table_name == tab_name).FirstOrDefault();

                if (items == null)
                {
                    return 0;
                }
                else
                {
                    //var item = items.FirstOrDefault();
                    items.IDX++;
                    db.SaveChanges();
                    //tx.Complete();
                    return items.IDX;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
            finally
            {
                db.Dispose();
            }
            //}
        }
        protected List<Options> ngDicToOption(Dictionary<String, String> OptionData)
        {
            List<Options> r = new List<Options>();
            foreach (var a in OptionData)
            {
                Options s = new Options();
                s.value = a.Key;
                s.label = a.Value;
                r.Add(s);
            }
            return r;
        }
        protected List<Options> ngCodeToOption(List<i_Code> OptionData)
        {
            List<Options> r = new List<Options>();

            foreach (var a in OptionData)
            {
                String v = getRecMessage(a.LangCode);

                Options sItem = new Options();
                sItem.value = a.Code;
                sItem.label = v.Equals(a.LangCode) ? a.Value : v;
                r.Add(sItem);
            }
            return r;
        }
        protected List<SelectListItem> MakeNumOptions(Int32 num, Boolean FirstIsBlank)
        {

            List<SelectListItem> r = new List<SelectListItem>();
            if (FirstIsBlank)
            {
                SelectListItem sItem = new SelectListItem();
                sItem.Value = "";
                sItem.Text = "";
                r.Add(sItem);
            }

            for (int n = 1; n <= num; n++)
            {
                SelectListItem s = new SelectListItem();
                s.Value = n.ToString();
                s.Text = n.ToString();
                r.Add(s);
            }
            return r;
        }
        protected List<Options> CodeValueByLang(List<i_Code> codeData)
        {
            List<Options> r = new List<Options>();
            foreach (var a in codeData)
            {
                String v = Resources.Res.ResourceManager.GetString(a.LangCode);
                r.Add(new Options()
                {
                    value = a.Code,
                    label = String.IsNullOrEmpty(v) ? a.Value : v
                });
            }
            return r;
        }
        protected void HandFineSave(String FileName, int Id, FilesUpScope fp, String FilesKind, Boolean pdfConvertImage)
        {
            Stream upFileStream = Request.InputStream;
            BinaryReader BinRead = new BinaryReader(upFileStream);
            String FileExt = System.IO.Path.GetExtension(FileName);

            #region IE file stream handle

            String[] IEOlderVer = new string[] { "6.0", "7.0", "8.0", "9.0" };
            System.Web.HttpPostedFile GetPostFile = null;
            if (Request.Browser.Browser == "IE" && IEOlderVer.Any(x => x == Request.Browser.Version))
            {
                System.Web.HttpFileCollection collectFiles = System.Web.HttpContext.Current.Request.Files;
                GetPostFile = collectFiles[0];
                if (!GetPostFile.FileName.Equals(""))
                {
                    //GetFileName = System.IO.Path.GetFileName(GetPostFile.FileName);
                    BinRead = new BinaryReader(GetPostFile.InputStream);
                }
            }

            Byte[] fileContents = { };
            //const int bufferSize = 1024; //set 1k buffer
            while (BinRead.BaseStream.Position < BinRead.BaseStream.Length - 1)
            {
                Byte[] buffer = new Byte[BinRead.BaseStream.Length - 1];
                int ReadLen = BinRead.Read(buffer, 0, buffer.Length);
                Byte[] dummy = fileContents.Concat(buffer).ToArray();
                fileContents = dummy;
                dummy = null;
            }
            #endregion

            String tpl_Org_FolderPath = String.Format(sysUpFilePathTpl, getArea, getController, Id, FilesKind, "OriginFile");
            String Org_Path = Server.MapPath(tpl_Org_FolderPath);

            #region 檔案上傳前檢查
            if (fp.LimitSize > 0)
                //if (GetPostFile.InputStream.Length > fp.LimitSize)
                if (BinRead.BaseStream.Length > fp.LimitSize)
                    throw new LogicError("Log_Err_FileSizeOver");

            if (fp.LimitCount > 0 && Directory.Exists(Org_Path))
            {
                String[] Files = Directory.GetFiles(Org_Path);
                if (Files.Count() >= fp.LimitCount) //還沒存檔，因此Selet到等於的數量，再加上現在要存的檔案即算超過
                    throw new LogicError("Log_Err_FileCountOver");
            }

            if (fp.AllowExtType != null)
                if (!fp.AllowExtType.Contains(FileExt.ToLower()))
                    throw new LogicError("Log_Err_AllowFileType");

            if (fp.LimitExtType != null)
                if (fp.LimitExtType.Contains(FileExt))
                    throw new LogicError("Log_Err_LimitedFileType");
            #endregion

            #region 存檔區

            if (!System.IO.Directory.Exists(Org_Path)) { System.IO.Directory.CreateDirectory(Org_Path); }

            //LogWrite.Write("Save File Start"+ Org_Path + "\\" + FileName);

            FileStream writeStream = new FileStream(Org_Path + "\\" + FileName, FileMode.Create);
            BinaryWriter BinWrite = new BinaryWriter(writeStream);
            BinWrite.Write(fileContents);
            //GetPostFile.SaveAs(Org_Path + "\\" + FileName);

            upFileStream.Close();
            writeStream.Close();
            BinWrite.Close();
            //LogWrite.Write("Save File End"+ Org_Path + "\\" + FileName);
            #endregion

            #region PDF轉圖檔
            if (pdfConvertImage)
            {
                FileInfo fi = new FileInfo(Org_Path + "\\" + FileName);
                if (fi.Extension == ".pdf")
                {
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.FileName = @"C:\Program Files\Boxoft PDF to JPG (freeware)\pdftojpg.exe";
                    proc.StartInfo.Arguments = Org_Path + "\\" + FileName + " " + Org_Path;
                    proc.StartInfo.RedirectStandardInput = true;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.Start();
                    proc.WaitForExit();
                    proc.Close();
                    proc.Dispose();
                }
            }
            #endregion
        }
        protected void HandImageSave(String FileName, int Id, ImageUpScope fp, string fileKnd)
        {

            BinaryReader binaryRead = null;
            string FileExt = System.IO.Path.GetExtension(FileName);

            #region IE file stream handle

            string[] IEOlderVer = new string[] { "6.0", "7.0", "8.0", "9.0" };
            System.Web.HttpPostedFile getPostFile = null;

            if (Request.Browser.Browser == "IE" && IEOlderVer.Any(x => x == Request.Browser.Version))
            {
                System.Web.HttpFileCollection collectFiles = System.Web.HttpContext.Current.Request.Files;
                getPostFile = collectFiles[0];
                if (!getPostFile.FileName.Equals(""))
                {
                    binaryRead = new BinaryReader(getPostFile.InputStream);
                }
            }
            else
            {
                binaryRead = new BinaryReader(Request.InputStream);
            }

            byte[] uploadFile = binaryRead.ReadBytes(System.Convert.ToInt32(binaryRead.BaseStream.Length));

            byte[] fileContents = { };
            //const int bufferSize = 1024 * 16; //set 16K buffer

            while (binaryRead.BaseStream.Position < binaryRead.BaseStream.Length - 1)
            {
                //Byte[] buffer = new Byte[bufferSize];
                byte[] buffer = new byte[binaryRead.BaseStream.Length - 1];
                int readBufferLength = binaryRead.Read(buffer, 0, buffer.Length);
                byte[] dummy = fileContents.Concat(buffer).ToArray();
                fileContents = dummy;
                dummy = null;
            }
            #endregion

            string tpl_Org_FolderPath = String.Format(sysUpFilePathTpl, getArea, getController, Id, fileKnd, "OriginFile");
            string Org_Path = Server.MapPath(tpl_Org_FolderPath);

            #region 檔案上傳前檢查
            if (fp.LimitSize > 0)
                //if (GetPostFile.InputStream.Length > fp.LimitSize)
                if (binaryRead.BaseStream.Length > fp.LimitSize)
                    throw new LogicError("Log_Err_FileSizeOver");

            if (fp.LimitCount > 0 && Directory.Exists(Org_Path))
            {
                String[] Files = Directory.GetFiles(Org_Path);
                if (Files.Count() >= fp.LimitCount) //還沒存檔，因此Selet到等於的數量，再加上現在要存的檔案即算超過
                    throw new LogicError("Log_Err_FileCountOver");
            }

            if (fp.AllowExtType != null)
                if (!fp.AllowExtType.Contains(FileExt.ToLower()))
                    throw new LogicError("Log_Err_AllowFileType");

            if (fp.LimitExtType != null)
                if (fp.LimitExtType.Contains(FileExt))
                    throw new LogicError("Log_Err_LimitedFileType");
            #endregion

            #region 存檔區

            if (fp.KeepOriginImage)
            {
                //原始檔
                //tpl_Org_FolderPath = String.Format(SystemUpFilePathTpl, GetArea, GetController, Id, FilesKind, "OriginFile");
                Org_Path = Server.MapPath(tpl_Org_FolderPath);
                if (!System.IO.Directory.Exists(Org_Path)) { System.IO.Directory.CreateDirectory(Org_Path); }

                FileStream writeStream = new FileStream(Org_Path + "\\" + FileName, FileMode.Create);
                BinaryWriter BinWrite = new BinaryWriter(writeStream);
                BinWrite.Write(fileContents);


                writeStream.Close();
                BinWrite.Close();
                //FileName.SaveAs(Org_Path + "\\" + FileName.FileName.GetFileName());
            }

            //後台管理的代表小圖
            String tpl_Rep_FolderPath = String.Format(sysUpFilePathTpl, getArea, getController, Id, fileKnd, "RepresentICON");
            String Rep_Path = Server.MapPath(tpl_Rep_FolderPath);
            if (!System.IO.Directory.Exists(Rep_Path)) { System.IO.Directory.CreateDirectory(Rep_Path); }
            MemoryStream smr = UpFileReSizeImage(fileContents, 0, 90);
            System.IO.File.WriteAllBytes(Rep_Path + "\\" + FileName.GetFileName(), smr.ToArray());
            smr.Dispose();

            if (fp.Parm.Count() > 0)
            {
                foreach (ImageSizeParm imSize in fp.Parm)
                {
                    tpl_Rep_FolderPath = String.Format(sysUpFilePathTpl, getArea, getController, Id, fileKnd, "s_" + imSize.SizeFolder);
                    Rep_Path = Server.MapPath(tpl_Rep_FolderPath);
                    if (!System.IO.Directory.Exists(Rep_Path)) { System.IO.Directory.CreateDirectory(Rep_Path); }
                    MemoryStream sm = UpFileReSizeImage(fileContents, imSize.width, imSize.heigh);
                    System.IO.File.WriteAllBytes(Rep_Path + "\\" + FileName.GetFileName(), sm.ToArray());
                    sm.Dispose();
                }
            }
            #endregion
        }
        protected void ImageFile(HttpPostedFileBase file, int Id, ImageUpScope fp, string fileKind)
        {
            string tpl_Org_FolderPath = string.Format(sysUpFilePathTpl, getArea, getController, Id, fileKind, "OriginFile");
            string originPath = Server.MapPath(tpl_Org_FolderPath);
            string fileExt = System.IO.Path.GetExtension(file.FileName);

            #region 檔案上傳前檢查
            if (fp.LimitSize > 0)
                if (file.ContentLength > fp.LimitSize)
                    throw new LogicError("Log_Err_FileSizeOver");

            if (fp.LimitCount > 0 && Directory.Exists(originPath))
            {
                string[] Files = Directory.GetFiles(originPath);
                if (Files.Count() >= fp.LimitCount) //還沒存檔，因此Selet到等於的數量，再加上現在要存的檔案即算超過
                    throw new LogicError("Log_Err_FileCountOver");
            }

            if (fp.AllowExtType != null)
                if (!fp.AllowExtType.Contains(fileExt.ToLower()))
                    throw new LogicError("Log_Err_AllowFileType");

            if (fp.LimitExtType != null)
                if (fp.LimitExtType.Contains(fileExt))
                    throw new LogicError("Log_Err_LimitedFileType");
            #endregion

            #region 存檔區

            if (fp.KeepOriginImage)
            {
                originPath = Server.MapPath(tpl_Org_FolderPath);
                if (!System.IO.Directory.Exists(originPath)) { System.IO.Directory.CreateDirectory(originPath); }
                file.SaveAs(originPath + "\\" + file.FileName);
            }

            //後台管理的代表小圖

            var fileByte = new Byte[file.ContentLength];
            file.InputStream.Position = 0;
            file.InputStream.Read(fileByte, 0, file.ContentLength);

            string tpl_Rep_FolderPath = string.Format(sysUpFilePathTpl, getArea, getController, Id, fileKind, "RepresentICON");
            string Rep_Path = Server.MapPath(tpl_Rep_FolderPath);
            if (!System.IO.Directory.Exists(Rep_Path)) { System.IO.Directory.CreateDirectory(Rep_Path); }
            MemoryStream smr = UpFileReSizeImage(fileByte, 0, 32);
            System.IO.File.WriteAllBytes(Rep_Path + "\\" + file.FileName, smr.ToArray());
            smr.Dispose();

            if (fp.Parm.Count() > 0)
            {
                foreach (ImageSizeParm imSize in fp.Parm)
                {
                    tpl_Rep_FolderPath = String.Format(sysUpFilePathTpl, getArea, getController, Id, fileKind, "s_" + imSize.SizeFolder);
                    Rep_Path = Server.MapPath(tpl_Rep_FolderPath);
                    if (!System.IO.Directory.Exists(Rep_Path)) { System.IO.Directory.CreateDirectory(Rep_Path); }
                    MemoryStream sm = UpFileReSizeImage(fileByte, imSize.width, imSize.heigh);
                    System.IO.File.WriteAllBytes(Rep_Path + "\\" + file.FileName, sm.ToArray());
                    sm.Dispose();
                }
            }
            #endregion
        }
        protected MemoryStream UpFileReSizeImage(Byte[] s, int newWidth, int newHight)
        {
            try
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
                Bitmap im = (Bitmap)tc.ConvertFrom(s);

                if (newHight == 0)
                    newHight = (im.Height * newWidth) / im.Width;

                if (newWidth == 0)
                    newWidth = (im.Width * newHight) / im.Height;

                if (im.Width < newWidth)
                    newWidth = im.Width;

                if (im.Height < newHight)
                    newHight = im.Height;

                EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                EncoderParameters myEncoderParameter = new EncoderParameters(1);
                myEncoderParameter.Param[0] = qualityParam;

                ImageCodecInfo myImageCodecInfo = GetEncoder(im.RawFormat);

                Bitmap ImgOutput = new Bitmap(im, newWidth, newHight);

                //ImgOutput.Save();
                MemoryStream ss = new MemoryStream();

                ImgOutput.Save(ss, myImageCodecInfo, myEncoderParameter);
                im.Dispose();
                return ss;
            }
            catch (Exception ex)
            {
                Log.Write("Image Handle Error:" + ex.Message);
                return null;
            }
            //ImgOutput.Dispose(); 
        }
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        protected MemoryStream UpFileCropCenterImage(Byte[] s, int width, int heigh)
        {
            try
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));

                Bitmap ImgSource = (Bitmap)tc.ConvertFrom(s);
                Bitmap ImgOutput = new Bitmap(width, heigh);

                int x = (ImgSource.Width - width) / 2;
                int y = (ImgSource.Height - heigh) / 2;
                Rectangle cropRect = new Rectangle(x, y, width, heigh);

                using (Graphics g = Graphics.FromImage(ImgOutput))
                {
                    g.DrawImage(ImgSource, new Rectangle() { Height = heigh, Width = width, X = 0, Y = 0 }, cropRect, GraphicsUnit.Pixel);
                }

                MemoryStream ss = new MemoryStream();
                ImgOutput.Save(ss, ImgSource.RawFormat);
                ImgSource.Dispose();
                return ss;
            }
            catch (Exception ex)
            {
                Log.Write("Image Handle Error:" + ex.Message);
                return null;
            }
            //ImgOutput.Dispose(); 
        }
        protected FilesObject[] ListSysFiles(int Id, String FilesKind)
        {
            return ListSysFiles(Id, FilesKind, true);
        }
        protected FilesObject[] ListSysFiles(int Id, String FilesKind, Boolean IsImageList)
        {
            String tpl_FolderPath = String.Empty;
            String Path = String.Empty;

            String AccessFilesKind = FilesKind == "" ? "DocFiles" : FilesKind;
            tpl_FolderPath = String.Format(sysUpFilePathTpl, getArea, getController, Id, AccessFilesKind, "OriginFile");
            Path = Server.MapPath(tpl_FolderPath);
            IsImageList = false;

            if (Directory.Exists(Path))
            {
                String[] CheckFiles = Directory.GetFiles(Path);

                if (CheckFiles.Count() > 0)
                {
                    String FileListTypeCheck = CheckFiles.FirstOrDefault();
                    FileInfo GetFirstFileToCheck = new FileInfo(FileListTypeCheck);

                    if (GetFirstFileToCheck.Extension.ToLower().Contains("jpg") || GetFirstFileToCheck.Extension.ToLower().Contains("jpeg") ||
                    GetFirstFileToCheck.Extension.ToLower().Contains("png") || GetFirstFileToCheck.Extension.ToLower().Contains("gif") ||
                    GetFirstFileToCheck.Extension.ToLower().Contains("bmp"))
                    {
                        tpl_FolderPath = String.Format(sysUpFilePathTpl, getArea, getController, Id, AccessFilesKind, "RepresentICON");
                        Path = Server.MapPath(tpl_FolderPath);
                        IsImageList = true;
                    }
                }
            }

            //LogWrite.Write("File List"+ Path);

            List<FilesObject> ls_Files = new List<FilesObject>();

            if (Directory.Exists(Path))
            {
                foreach (String fileString in Directory.GetFiles(Path))
                {
                    FileInfo fi = new FileInfo(fileString);
                    FilesObject fo = new FilesObject() { FileName = fi.Name, FilesKind = FilesKind, RepresentFilePath = Url.Content(tpl_FolderPath + "/" + fi.Name) };

                    if (fi.Extension.ToLower().Contains("jpg") || fi.Extension.ToLower().Contains("jpeg") ||
                        fi.Extension.ToLower().Contains("png") || fi.Extension.ToLower().Contains("gif") ||
                        fi.Extension.ToLower().Contains("bmp"))
                        fo.IsImage = true;

                    if (IsImageList)
                    {
                        String org_tpl_FolderPath = String.Format(sysUpFilePathTpl, getArea, getController, Id, AccessFilesKind, "OriginFile");
                        Path = Server.MapPath(org_tpl_FolderPath);
                        fi = new FileInfo(Path + "/" + fi.Name);
                        fo.OriginFilePath = Url.Content(org_tpl_FolderPath + "/" + fi.Name);
                        fo.Size = fi.Length;
                    }
                    else
                    {
                        fo.OriginFilePath = Url.Content(tpl_FolderPath + "/" + fi.Name);
                        fo.Size = fi.Length;
                    }

                    ls_Files.Add(fo);
                }
            }
            return ls_Files.ToArray();
        }
        protected void DeleteSysFile(int Id, String FilesKind, String FileName, ImageUpScope im)
        {
            String SystemDelSysIdKind = "~/_Code/SysUpFiles/{0}.{1}/{2}/{3}";
            String tpl_FolderPath = Server.MapPath(String.Format(SystemDelSysIdKind, getArea, getController, Id, FilesKind));
            #region Delete Run
            if (Directory.Exists(tpl_FolderPath))
            {
                var folders = Directory.GetDirectories(tpl_FolderPath);
                foreach (var folder in folders)
                {
                    String herefile = folder + "\\" + FileName;
                    if (System.IO.File.Exists(herefile))
                        System.IO.File.Delete(herefile);
                }
            }
            #endregion
        }
        protected void DeleteIdFiles(int Id)
        {
            String tpl_FolderPath = String.Empty;
            tpl_FolderPath = String.Format(sysDelSysId, getArea, getController, Id);
            String Path = Server.MapPath(tpl_FolderPath);
            if (Directory.Exists(Path))
                Directory.Delete(Path, true);
        }
        public FileResult DownLoadFile(Int32 Id, String FilesKind, String FileName)
        {
            String SearchPath = String.Format(sysUpFilePathTpl + "\\" + FileName, getArea, getController, Id, FilesKind, "OriginFile");
            String DownFilePath = Server.MapPath(SearchPath);

            FileInfo fi = null;
            if (System.IO.File.Exists(DownFilePath))
                fi = new FileInfo(DownFilePath);

            return File(DownFilePath, "application/" + fi.Extension.Replace(".", ""), Url.Encode(fi.Name));
        }
        public String ImgSrc(String AreaName, String ContorllerName, Int32 Id, String FilesKind, String ImageSize)
        {
            String ImgSizeString = ImageSize;
            String SearchPath = String.Format(sysUpFilePathTpl, AreaName, ContorllerName, Id, FilesKind, ImgSizeString);
            String FolderPth = Server.MapPath(SearchPath);

            if (Directory.Exists(FolderPth))
            {
                String[] SFiles = Directory.GetFiles(FolderPth);

                if (SFiles.Length > 0)
                {
                    FileInfo f = new FileInfo(SFiles[0]);
                    return Url.Content(SearchPath) + "/" + f.Name;
                }
                else
                    return null;
            }
            else
                return null;
        }
        public PageImgShow[] ImgSrcApp(String AreaName, String ContorllerName, Int32 Id, String FilesKind, String ImageSize)
        {
            String ImgSizeString = ImageSize;
            String ICON_Path = String.Format(sysUpFilePathTpl, AreaName, ContorllerName, Id, FilesKind, "RepresentICON");
            String Link_Path = String.Format(sysUpFilePathTpl, AreaName, ContorllerName, Id, FilesKind, ImgSizeString);

            String FolderPth = Server.MapPath(ICON_Path);

            if (Directory.Exists(FolderPth))
            {
                String[] SFiles = Directory.GetFiles(FolderPth);
                List<PageImgShow> web_path = new List<PageImgShow>();
                foreach (var file_path in SFiles)
                {
                    FileInfo f = new FileInfo(file_path);
                    web_path.Add(new PageImgShow()
                    {
                        icon_path = Url.Content(ICON_Path) + "/" + f.Name,
                        link_path = Url.Content(Link_Path) + "/" + f.Name
                    });
                }
                return web_path.ToArray();
            }
            else
                return null;
        }
        public RedirectResult SetLanguage(String L, String A)
        {
            HttpCookie WebLang = new HttpCookie(DotWeb.CommSetup.CommWebSetup.WebCookiesId + ".Lang", L);
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(WebLang.Value);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(WebLang.Value);
            ViewBag.Lang = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            Response.Cookies.Add(WebLang);
            return Redirect(A);
        }
        public string ModelStateErrorPack()
        {
            List<string> errMessage = new List<string>();
            foreach (ModelState modelState in ModelState.Values)
                foreach (ModelError error in modelState.Errors)
                    errMessage.Add(error.ErrorMessage);

            return string.Join(":", errMessage);
        }
        protected override LogicCenter openLogic()
        {
            var o = base.openLogic();
            o.AspUserID = aspUserId;
            o.DepartmentId = departmentId;
            o.Lang = getNowLnag();
            return o;
        }
    }
    public abstract class WebFrontController : SourceController
    {
        protected Int32 visitCount = 0;
        //protected Log.LogPlamInfo plamInfo = new Log.LogPlamInfo() { AllowWrite = true };
        //protected readonly string sessionShoppingString = "CestLaVie.Shopping";
        //protected readonly string sessionMemberLoginString = "CestLaVie.loginMail";
        private readonly string sysUpFilePathTpl = "~/_Code/SysUpFiles/{0}.{1}/{2}/{3}/{4}";
        protected WebInfo wi;

        protected WebFrontController()
        {
            ViewBag.NowHeadMenu = "";
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            //plamInfo.BroswerInfo = System.Web.HttpContext.Current.Request.Browser.Browser + "." + System.Web.HttpContext.Current.Request.Browser.Version;
            //plamInfo.IP = System.Web.HttpContext.Current.Request.UserHostAddress;
            //plamInfo.UserId = 0;
            //plamInfo.UnitId = 0;

            Log.SetupBasePath = System.Web.HttpContext.Current.Server.MapPath("~\\_Code\\Log\\");
            Log.Enabled = true;

            try
            {
                var db = getDB0();

                var Async = db.SaveChangesAsync();
                Async.Wait();

                ViewBag.VisitCount = visitCount;
                ViewBag.IsFirstPage = false; //是否為首頁，請在首頁的Action此值設為True
                ajax_GetSidebarData();

                wi = new WebInfo();
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message);
            }
        }
        public void ajax_GetSidebarData()
        {
            db0 = getDB0();

            var typeData = db0.ProductType.Where(x=>x.i_Hide==false).Select(x => new { x.type_name, x.id, x.sort, x.is_second }).OrderByDescending(x => x.sort).ToList();

            List<PType> types = new List<PType>();
            foreach (var a in typeData)
            {
                PType t = new PType();
                t.id = a.id;
                t.type_name = a.type_name;
                t.is_second = a.is_second;
                List<PData> ps = new List<PData>();
                var productData = db0.ProductData.Where(x => x.type_id == a.id && x.i_Hide==false).Select(x => new { x.product_name, x.id, x.sort, x.type_id }).OrderByDescending(x => x.sort).ToList();
                foreach (var b in productData)
                {
                    PData p = new PData();
                    p.id = b.id;
                    p.name = b.product_name;

                    ps.Add(p);
                }
                t.PDatas = ps;
                types.Add(t);
            }

            ViewBag.Sidebar = types;
        }
        public int GetNewId()
        {
            return GetNewId(ProcCore.Business.CodeTable.Base);
        }
        public int GetNewId(ProcCore.Business.CodeTable tab)
        {
            using (var db = getDB0())
            {
                using (TransactionScope tx = new TransactionScope())
                {
                    try
                    {
                        String tab_name = Enum.GetName(typeof(ProcCore.Business.CodeTable), tab);
                        var items = from x in db.i_IDX where x.table_name == tab_name select x;

                        if (items.Count() == 0)
                        {
                            return 0;
                        }
                        else
                        {
                            var item = items.FirstOrDefault();
                            item.IDX++;
                            db.SaveChanges();
                            tx.Complete();
                            return item.IDX;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return 0;
                    }
                }
            }
        }
        private snObject GetSN(ProcCore.Business.SNType tab)
        {

            snObject sn = new snObject();

            using (var db = getDB0())
            {
                using (TransactionScope tx = new TransactionScope())
                {
                    try
                    {
                        String tab_name = Enum.GetName(typeof(ProcCore.Business.SNType), tab);
                        var items = db.i_SN.Single(x => x.sn_type == tab_name);

                        if (items.y == DateTime.Now.Year &&
                            items.m == DateTime.Now.Month &&
                            items.d == DateTime.Now.Day
                            )
                        {
                            int now_max = items.sn_max;
                            now_max++;
                            items.sn_max = now_max;
                        }
                        else
                        {
                            items.y = DateTime.Now.Year;
                            items.m = DateTime.Now.Month;
                            items.d = DateTime.Now.Day;
                            items.sn_max = 1;
                        }

                        db.SaveChanges();
                        tx.Complete();

                        sn.y = items.y;
                        sn.m = items.m;
                        sn.d = items.d;
                        sn.sn_max = items.sn_max;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return sn;
        }
        public string Get_Orders_SN()
        {
            String tpl = "SN{0}{1:00}{2:00}-{3:00}{4:00}";
            snObject sn = GetSN(ProcCore.Business.SNType.Orders);
            return String.Format(tpl, sn.y.ToString().Right(2), sn.m, sn.d, sn.sn_max, (new Random()).Next(99));
        }
        public FileResult DownLoadFile(Int32 Id, String GetArea, String GetController, String FileName, String FilesKind)
        {
            if (FilesKind == null)
                FilesKind = "DocFiles";

            String SystemUpFilePathTpl = "~/_Code/SysUpFiles/{0}.{1}/{2}/{3}/{4}";
            String SearchPath = String.Format(SystemUpFilePathTpl + "\\" + FileName, GetArea, GetController, Id, FilesKind, "OriginFile");
            String DownFilePath = Server.MapPath(SearchPath);

            FileInfo fi = null;
            if (System.IO.File.Exists(DownFilePath))
            {
                fi = new FileInfo(DownFilePath);
            }
            return File(DownFilePath, "application/" + fi.Extension.Replace(".", ""), Url.Encode(fi.Name));
        }
        public String ImgSrc(String AreaName, String ContorllerName, Int32 Id, String FilesKind, Int32 ImageSizeTRype)
        {
            String ImgSizeString = "s_" + ImageSizeTRype;
            String SearchPath = String.Format(sysUpFilePathTpl, AreaName, ContorllerName, Id, FilesKind, ImgSizeString);
            String FolderPth = Server.MapPath(SearchPath);

            if (Directory.Exists(FolderPth))
            {
                String[] SFiles = Directory.GetFiles(FolderPth);

                if (SFiles.Length > 0)
                {
                    FileInfo f = new FileInfo(SFiles[0]);
                    return Url.Content(SearchPath) + "/" + f.Name;
                }
                else
                {
                    return Url.Content("~/Content/images/nopic.png");
                }

            }
            else
                return Url.Content("~/Content/images/nopic.png");
        }
        public String[] AllImgSrc(String AreaName, String ContorllerName, Int32 Id, String FilesKind, Int32 ImageSizeTRype)
        {
            String ImgSizeString = "s_" + ImageSizeTRype;
            String SearchPath = String.Format(sysUpFilePathTpl, AreaName, ContorllerName, Id, FilesKind, ImgSizeString);
            String FolderPth = Server.MapPath(SearchPath);

            List<String> I = new List<String>();
            if (Directory.Exists(FolderPth))
            {
                String[] fs = Directory.GetFiles(FolderPth).Where(x => (new FileInfo(x)).Name != "Thumbs.db").ToArray();

                foreach (String f in fs)
                {
                    FileInfo fi = new FileInfo(f);
                    String P = UrlHelper.GenerateContentUrl(SearchPath + "/" + fi.Name, this.HttpContext);
                    I.Add(P);
                }
            }
            return I.ToArray();
        }

        public List<imgs> TwoSizeAllImgSrc(String AreaName, String ContorllerName, Int32 Id, String FilesKind, Int32 ImageSizeSmall, Int32 ImageSizeBig)
        {
            string[] small = AllImgSrc(AreaName, ContorllerName, Id, FilesKind, ImageSizeSmall);//小圖
            string[] big = AllImgSrc(AreaName, ContorllerName, Id, FilesKind, ImageSizeBig);//大圖
            List<imgs> imgs = new List<imgs>();
            int i = 0;
            foreach (var f in small)
            {
                imgs img = new imgs();
                img.small_size = f;
                img.big_size = big[i];
                imgs.Add(img);
                i++;
            }
            return imgs;
        }
        public FileResult AudioFile(String FilePath)
        {
            String S = Url.Content(FilePath);
            String DownFilePath = Server.MapPath(S);

            FileInfo fi = null;
            if (System.IO.File.Exists(DownFilePath))
                fi = new FileInfo(DownFilePath);

            return File(DownFilePath, "audio/mp3", Url.Encode(fi.Name));
        }
        public String GetSYSImage(Int32 Id, String GetArea, String GetController)
        {
            String SystemUpFilePathTpl = "~/_Code/SysUpFiles/{0}.{1}/{2}/{3}/{4}";
            String SearchPath = String.Format(SystemUpFilePathTpl, GetArea, GetController, Id, "DefaultKind", "OriginFile");
            String GetFolderPath = Server.MapPath(SearchPath);

            if (System.IO.Directory.Exists(GetFolderPath))
            {
                String fs = Directory.GetFiles(GetFolderPath).FirstOrDefault();
                FileInfo f = new FileInfo(fs);
                return SearchPath + "/" + f.Name;
            }
            else
            {
                return null;
            }
        }
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            Log.WriteToFile();
        }
        public RedirectResult SetLanguage(String L, String A)
        {
            HttpCookie WebLang = new HttpCookie(DotWeb.CommSetup.CommWebSetup.WebCookiesId + ".Lang", L);
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(WebLang.Value);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(WebLang.Value);
            Response.Cookies.Add(WebLang);
            return Redirect(Url.Action(A));
        }
        protected override string getRecMessage(string MsgId)
        {
            return Resources.Res.ResourceManager.GetString(MsgId);
        }
        protected List<SelectListItem> MakeNumOptions(Int32 num, Boolean FirstIsBlank)
        {

            List<SelectListItem> r = new List<SelectListItem>();
            if (FirstIsBlank)
            {
                SelectListItem sItem = new SelectListItem();
                sItem.Value = "";
                sItem.Text = "";
                r.Add(sItem);
            }

            for (int n = 1; n <= num; n++)
            {
                SelectListItem s = new SelectListItem();
                s.Value = n.ToString();
                s.Text = n.ToString();
                r.Add(s);
            }
            return r;
        }

        public class StringResult : ViewResult
        {
            public String ToHtmlString { get; set; }
            public override void ExecuteResult(ControllerContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }

                if (string.IsNullOrEmpty(this.ViewName))
                {
                    this.ViewName = context.RouteData.GetRequiredString("action");
                }

                ViewEngineResult result = null;

                if (this.View == null)
                {
                    result = this.FindView(context);
                    this.View = result.View;
                }

                MemoryStream stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(stream);

                ViewContext viewContext = new ViewContext(context, this.View, this.ViewData, this.TempData, writer);

                this.View.Render(viewContext, writer);

                writer.Flush();

                ToHtmlString = Encoding.UTF8.GetString(stream.ToArray());

                if (result != null)
                    result.ViewEngine.ReleaseView(context, this.View);
            }
        }

        #region 寄信相關
        //將變數套用至信件版面
        public string getMailBody(string EmailView, object md)
        {
            ViewResult resultView = View(EmailView, md);

            StringResult sr = new StringResult();
            sr.ViewName = resultView.ViewName;
            sr.MasterName = resultView.MasterName;
            sr.ViewData = resultView.ViewData;
            sr.TempData = resultView.TempData;
            sr.ExecuteResult(this.ControllerContext);

            return sr.ToHtmlString;
        }

        public bool Mail_Send(string MailFrom, string[] MailTos, string[] Bcc, string MailSub, string MailBody, bool isBodyHtml)
        {
            try
            {
                //建立MailMessage物件
                MailMessage mms = new MailMessage();
                mms.From = new MailAddress(MailFrom);//寄件人
                mms.Subject = MailSub;//信件主旨
                mms.Body = MailBody;//信件內容
                mms.IsBodyHtml = isBodyHtml;//判斷是否採用html格式

                if (MailTos != null)//防呆
                {
                    for (int i = 0; i < MailTos.Length; i++)
                    {
                        //加入信件的收信人(們)address
                        if (!string.IsNullOrEmpty(MailTos[i].Trim()))
                        {
                            mms.To.Add(new MailAddress(MailTos[i].Trim()));
                        }

                    }
                }//End if (MailTos !=null)//防呆

                if (Bcc != null) //防呆
                {
                    for (int i = 0; i < Bcc.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(Bcc[i].Trim()))
                        {
                            //加入信件的密件副本(們)address
                            mms.Bcc.Add(new MailAddress(Bcc[i].Trim()));
                        }

                    }
                }//End if (Ccs!=null) //防呆



                using (SmtpClient client = new SmtpClient(CommWebSetup.smtpServer, CommWebSetup.smtpPort))//或公司、客戶的smtp_server

                    client.Send(mms);//寄出一封信

                //釋放每個附件，才不會Lock住
                if (mms.Attachments != null && mms.Attachments.Count > 0)
                {
                    for (int i = 0; i < mms.Attachments.Count; i++)
                    {
                        mms.Attachments[i].Dispose();
                        mms.Attachments[i] = null;
                    }
                }

                return true;//寄信成功
            }
            catch (Exception)
            {
                return false;//寄信失敗
            }
        }

        #endregion
    }
    #endregion

    #region 泛型控制器擴充

    public abstract class CtrlId<M, Q> : BaseController
        where M : new()
        where Q : QueryBase
    {
        protected rAjaxGetData<M> r;
        protected M item;
        public abstract String aj_Init();
        public abstract String aj_MasterDel(Int32[] ids);
        public abstract String aj_MasterSearch(Q sh);
        public abstract String aj_MasterInsert(M md);
        public abstract String aj_MasterUpdate(M md);
        public abstract String aj_MasterGet(Int32 id);
    }
    public abstract class CtrlTId<M, Q> : BaseController
        where M : new()
        where Q : QueryBase
    {
        protected rAjaxGetData<M> r;
        protected M item;
        public abstract String aj_Init();
        public abstract Task<string> aj_MasterDel(Int32[] ids);
        public abstract string aj_MasterSearch(Q sh);
        public abstract Task<string> aj_MasterInsert(M md);
        public abstract Task<string> aj_MasterUpdate(M md);
        public abstract Task<string> aj_MasterGet(Int32 id);
    }

    public abstract class CtrlSN<M, Q> : BaseController
        where M : new()
        where Q : QueryBase
    {
        protected rAjaxGetData<M> r;
        protected M item;
        public abstract String aj_Init();
        public abstract String aj_MasterDel(string[] sns);
        public abstract String aj_MasterSearch(Q sh);
        public abstract String aj_MasterInsert(M md);
        public abstract String aj_MasterUpdate(M md);
        public abstract String aj_MasterGet(string sn);
    }

    public abstract class CtrlTSN<M, Q> : BaseController
        where M : new()
        where Q : QueryBase
    {
        protected rAjaxGetData<M> r;
        protected M item;
        public abstract string aj_Init();
        public abstract Task<string> aj_MasterDel(string[] sns);
        public abstract string aj_MasterSearch(Q sh);
        public abstract Task<string> aj_MasterInsert(M md);
        public abstract Task<string> aj_MasterUpdate(M md);
        public abstract Task<string> aj_MasterGet(string sn);
    }

    public abstract class CtrlIdId<M, Q, Ms, Qs> : CtrlId<M, Q>
        where M : new()
        where Q : QueryBase
        where Ms : new()
        where Qs : QueryBase
    {
        protected Ms item_sub;
        protected IEnumerable<Ms> items_sub;

        protected rAjaxGetData<Ms> rd;
        public abstract string aj_DetailGet(Int32 DetailId);
        public abstract string aj_DetailSearchBySN(String MasterSerial);
        public abstract string aj_DetailInsert(Ms md);
        public abstract string aj_DetailUpdate(Ms md);
        public abstract string aj_DetailDelete(Int32[] ids);
    }

    public abstract class CtrlIdSN<M, Q, Ms, Qs> : CtrlId<M, Q>
        where M : new()
        where Q : QueryBase
        where Ms : new()
        where Qs : QueryBase
    {
        protected Ms item_sub;
        protected IEnumerable<Ms> items_sub;

        protected rAjaxGetData<Ms> rd;
        public abstract string aj_DetailGet(Int32 DetailId);
        public abstract string aj_DetailSearchBySN(String MasterSerial);
        public abstract string aj_DetailInsert(Ms md);
        public abstract string aj_DetailUpdate(Ms md);
        public abstract string aj_DetailDelete(Int32[] ids);
    }

    public abstract class CtrlSNSN<M, Q, Ms, Qs> : CtrlSN<M, Q>
        where M : new()
        where Q : QueryBase
        where Ms : new()
        where Qs : QueryBase
    {
        protected Ms item_sub;
        protected IEnumerable<Ms> items_sub;

        protected rAjaxGetData<Ms> rd;
        public abstract string aj_DetailGet(Int32 DetailId);
        public abstract string aj_DetailSearchBySN(String MasterSerial);
        public abstract string aj_DetailInsert(Ms md);
        public abstract string aj_DetailUpdate(Ms md);
        public abstract string aj_DetailDelete(int id);
    }


    public abstract class CtrlTSNTSN<M, Q, Ms, Qs> : CtrlTSN<M, Q>
        where M : new()
        where Q : QueryBase
        where Ms : new()
        where Qs : QueryBase
    {
        protected Ms item_sub;
        protected IEnumerable<Ms> items_sub;

        protected rAjaxGetData<Ms> rd;
        public abstract Task<string> aj_DetailGet(Int32 DetailId);
        public abstract Task<string> aj_DetailSearchBySN(String MasterSerial);
        public abstract Task<string> aj_DetailInsert(Ms md);
        public abstract Task<string> aj_DetailUpdate(Ms md);
        public abstract Task<string> aj_DetailDelete(int id);
    }

    #endregion
    public class DocInfo
    {
        public String Name { get; set; }
        public int Sort { get; set; }
        public String Momo { get; set; }
        public String Link { get; set; }
    }
    public class rAjaxGetData<T> : ResultInfo
    {
        public T data { get; set; }
        public bool hasData { get; set; }
    }
    public class rAjaxGetItems<T> : ResultInfo
    {
        //有大問題
        public object data { get; set; }
    }
    public class Options
    {
        public String value { get; set; }
        public String label { get; set; }
    }
    public class ReportData
    {
        public string ReportName { get; set; }
        public object[] Parms { get; set; }
        public object[] Data { get; set; }
    }
    public class PageImgShow
    {

        public String icon_path { get; set; }
        public String link_path { get; set; }
    }
    public class WebInfo
    {
        //public IList<最新消息> News { get; set; }
        //public IList<活動花絮主檔> Active { get; set; }
        //public IList<文件管理> Document { get; set; }
    }
    //sidebar class
    public class PType
    {
        public string type_name { get; set; }

        public bool is_second { get; set; }
        public int id { get; set; }
        public List<PData> PDatas { get; set; }
    }
    public class PData
    {
        public string name { get; set; }
        public int id { get; set; }
    }

}