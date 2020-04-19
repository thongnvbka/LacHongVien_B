using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SD = System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace ViELearn.BackEnd.Controllers.NhapDuLieu
{
    public class QuanLyHinhAnhController : Controller
    {
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        // GET: QuanLyHinhAnh
        public ActionResult Index()
        {
            var sql = $@"SELECT * FROM dbo.News ORDER BY PublishAt DESC, UpdatedAt DESC, CreatedAt DESC, id DESC";
            var dtHinhAnh = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsHinhAnh = dtHinhAnh;
            return View();
        }
        private int ConvertToInt(string value)
        {
            int result = 0;
            decimal d1 = Convert.ToDecimal(value);
            result = Convert.ToInt32(Math.Round(d1, 0));
            return result;
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SaveImageCrop(int id = 0, string path = "", string x = "", string y = "", string w = "", string h = "")
        {
            int x1 = ConvertToInt(x); int y1 = ConvertToInt(y); int w1 = ConvertToInt(w); int h1 = ConvertToInt(h);
            string fullPath = Server.MapPath("~") + path;
            byte[] CropImage = Crop(fullPath, w1, h1, x1, y1);
            using (MemoryStream ms = new MemoryStream(CropImage, 0, CropImage.Length))

            {
                ms.Write(CropImage, 0, CropImage.Length);
                var ext = Path.GetFileNameWithoutExtension(fullPath) + Path.GetExtension(fullPath);
                ext = ext.Replace(Path.GetFileName(ext), string.Format("{0}{1}{2}{3}_{4}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Millisecond,
               Path.GetFileNameWithoutExtension(ext) + Path.GetExtension(ext)));
                string SaveTo = "UserData/Share/Crop/";
                SaveTo = Server.MapPath("~") + SaveTo + ext;
                using (Image image = Image.FromStream(ms))
                {
                    Bitmap map =new Bitmap(image);
                    map.Save(SaveTo);
                }
            }
            return Json("Ok");
        }
        public ActionResult Edit()
        {
          
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string avatarCropped)
        {
            string filePath = ProcessImage(avatarCropped);

            if (ModelState.IsValid)
            {
               
              
                return RedirectToAction("Index");
            }
            return View();
        }
        private string ProcessImage(string croppedImage)
        {
            string filePath = String.Empty;
            try
            {
                string base64 = croppedImage;
                byte[] bytes = Convert.FromBase64String(base64.Split(',')[1]);
                filePath = "/Images/Photo/Emp-" + Guid.NewGuid() + ".png";
                using (FileStream stream = new FileStream(Server.MapPath(filePath), FileMode.Create))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                }
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }

            return filePath;
        }
        static byte[] Crop(string Img, int Width, int Height, int X, int Y)

        {

            try

            {

                using (SD.Image OriginalImage = SD.Image.FromFile(Img))

                {

                    using (SD.Bitmap bmp = new SD.Bitmap(Width, Height))

                    {

                        bmp.SetResolution(OriginalImage.HorizontalResolution, OriginalImage.VerticalResolution);

                        using (SD.Graphics Graphic = SD.Graphics.FromImage(bmp))

                        {

                            Graphic.SmoothingMode = SmoothingMode.AntiAlias;

                            Graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;

                            Graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;

                            Graphic.DrawImage(OriginalImage, new SD.Rectangle(0, 0, Width, Height), X, Y, Width, Height, SD.GraphicsUnit.Pixel);

                            MemoryStream ms = new MemoryStream();

                            bmp.Save(ms, OriginalImage.RawFormat);

                            return ms.GetBuffer();

                        }

                    }

                }

            }

            catch (Exception Ex)

            {

                throw (Ex);

            }

        }


    }
}