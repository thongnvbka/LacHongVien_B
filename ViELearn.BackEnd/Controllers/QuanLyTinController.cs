using System;
using System.Configuration;
using System.Data;
using System.Web.Mvc;

namespace ViELearn.BackEnd.Controllers
{
    public class QuanLyTinController : Controller
    {
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: QuanLyTin
        public ActionResult Index()
        {
            var sql = $@"SELECT * FROM dbo.News ORDER BY PublishAt DESC, UpdatedAt DESC, CreatedAt DESC, id DESC";
            var dtBaiViet = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsBaiViet = dtBaiViet;

            sql = $@"SELECT * FROM dbo.Tags";
            var dtTags = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsTags = dtTags;

            return View();
        }



        #region them moi bai viet
        [HttpGet]
        public ActionResult Themmoi()
        {
            var dtDsTags = DBLibs.GetDataBy_DataAdapter($@"SELECT * FROM Tags", _cnn);
            ViewBag.DsTags = dtDsTags != null ? dtDsTags : new DataTable();
            return View();
        }

        [HttpPost]
        public ActionResult Themmoi(FormCollection frm)
        {
         
            

            DBLibs.ExecuteScalar($@"
            INSERT INTO dbo.News
            (
                Slug ,
                UnitId ,
                Title ,
                Thumbnail ,
                Summary ,
                FullContent ,
                related_news ,
                Cates ,
                Tags ,
                Type ,
                Status ,
                [Read] ,
                CreatedAt ,
                UpdatedBy ,
                PublishAt ,
                meta_keywords ,
                meta_descriptions ,
                show_athome ,
                view_total ,
                view_temp ,
                last_visisted ,
                last_sync_time ,
                vb_coquan ,
                vb_loaivb ,
                vb_linhvuc ,
                vb_tinhtrang ,
                vb_sovb ,
                vb_kyhieu ,
                vb_ngaybanhanh ,
                vb_nguoiky
            )
            VALUES
            (
                '{frm["slug"].Replace("'", "''").Trim()}' , -- Slug - varchar(250)
                0, -- UnitId - int
                N'{frm["title"].Replace("'", "''").Trim()}' , -- Title - nvarchar(200)
                N'{frm["url"].Replace("'", "''").Trim()}' , -- Thumbnail - nvarchar(500)
                N'{frm["noidung"].Replace("'", "''").Trim()}' , -- Summary - nvarchar(1000)
                N'{frm["sapo"].Replace("'", "''").Trim()}' , -- FullContent - ntext
                '' , -- related_news - varchar(45)
                N'{frm["cates"].Replace("'", "''").Trim()}' , -- Cates - nvarchar(1000)
                N'{frm["tag"].Replace("'", "''").Trim()}' , -- Tags - nvarchar(1000)
                0 , -- Type - tinyint
                1 , -- Status - tinyint
                0 , -- Read - int
                {CLibs.DatetimeToTimestampOrgin(DateTime.Now)}  , -- CreatedAt - int

                0 , -- CreatedBy - int
                0 , -- UpdatedBy - int
                0 , -- PublishAt - int
                N'{frm["meta_keywords"].Replace("'", "''").Trim()}' , -- meta_keywords - nvarchar(250)
                N'{frm["meta_descriptions"].Replace("'", "''").Trim()}' , -- meta_descriptions - nvarchar(550)
                NULL , -- show_athome - bit
                0 , -- view_total - bigint
                0 , -- view_temp - int
                0 , -- last_visisted - int
                0 , -- last_sync_time - int
                0 , -- vb_coquan - int
                0 , -- vb_loaivb - int
                0 , -- vb_linhvuc - int
                0 , -- vb_tinhtrang - int
                N'' , -- vb_sovb - nvarchar(150)
                N'' , -- vb_kyhieu - nvarchar(150)
                0 , -- vb_ngaybanhanh - int
                N''  -- vb_nguoiky - nvarchar(250)
            )  SELECT SCOPE_IDENTITY() ", _cnn);
            return RedirectToAction("Index");
        }
        #endregion

        #region cap nhat bai viet
        [HttpGet]
        public ActionResult Sua(int id)
        {
            var sql = $@"SELECT * FROM dbo.Tags";
            var dtTags = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsTags = dtTags;

            sql = $@"SELECT id, title FROM News WHERE id <> {id} AND id NOT IN (SELECT related_news FROM News WHERE id = {id}) AND PublishAt BETWEEN {CLibs.DatetimeToTimestampOrgin(DateTime.Now.AddMonths(-1))} AND {CLibs.DatetimeToTimestampOrgin(DateTime.Now)}";
            var dtDsBaiViet = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsBaiViet = dtDsBaiViet;

            sql = $@"SELECT id, title FROM News WHERE id IN (SELECT related_news FROM News WHERE id = {id})";
            var dtRelatedNews = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsBaiVietLienQuan = dtRelatedNews;

            #region Get item infos

            var dtInfos = DBLibs.GetDataBy_DataAdapter($@"SELECT TOP 1 * FROM News WHERE id = {id}", _cnn);
            ViewBag.Infos = dtInfos.Rows[0];

            #endregion Get item infos

            ViewBag.Cates = Request["_cates"];
            ViewBag.Id = id;

            return View();
        }

       

      

        [HttpPost]
        public ActionResult Sua(FormCollection frm, int id)
        {
            var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM News WHERE id <> {id} AND (slug = N'{frm["slug"]}' OR title = N'{frm["title"]}')", _cnn);
            if (exited != null && int.Parse(exited.ToString()) > 0)
            {
                return Json(new
                {
                    status = false,
                    message = "Đã có bài viết có cùng tiêu đề hoặc url!"
                });
            }
            var eff = DBLibs.ExecuteNonQuery($@"
                    UPDATE News
                    SET
                        Slug = '{frm["slug"].Trim()}' , -- Slug - varchar(250)
                        Title = N'{frm["title"].Trim()}' , -- Title - nvarchar(200)
                        Thumbnail = N'{frm["url"]}' , -- Thumbnail - nvarchar(500)
                        Summary = N'{frm["noidung"].Trim()}' , -- Summary - nvarchar(1000)
                        FullContent = N'{frm["sapo"].Replace("'", "''").Trim()}' , -- FullContent - ntext
                        related_news = N'' , -- related_news - nvarchar(45)
                        Cates = N'{frm["cates"].Replace("'", "''").Trim()}' , -- Cates - nvarchar(1000)
                        Tags = N'{frm["tag"].Replace("'", "''").Trim()}' , -- Tags - nvarchar(1000)
                        Status = 1, -- Status - tinyint
                        UpdatedAt = {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} , -- UpdatedAt - int
                        PublishAt = {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} , -- PublishAt - int
                        UpdatedBy = 0 , -- UpdatedBy - int
                        meta_keywords = N'{frm["meta_keywords"].Replace("'", "''").Trim()}' , -- meta_keywords - nvarchar(50)
                        meta_descriptions = N'{frm["meta_descriptions"].Replace("'", "''").Trim()}'  -- meta_descriptions - nvarchar(550)
                    WHERE
                        id = {id}", _cnn);

            return RedirectToAction("Index");
        }

        #endregion 

        #region xoa 1 ban ghi

        public ActionResult Xoa(int id)
        {
            DBLibs.ExecuteNonQuery($@"delete News where id={id}", _cnn);

            return RedirectToAction("Index");
        }

        #endregion
    }
}