using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web.Services;
using System.Web;
using System.Drawing;


namespace iTR.Lib
{
    public class FileHelper
    {
        public static bool CheckFileType(string fileName, String[] allowedExtensions)
        {
            string ext = Path.GetExtension(fileName);
            bool fileOK=false;

           
            //判断用户选择的文件类型是否受限
            for (int i = 0; i < allowedExtensions.Length; i++)
            {
                if (ext == allowedExtensions[i])
                {
                    fileOK = true;
                    break;
                }
            }
            return fileOK;

        }

        public static bool UploadImage(string base64String, string path, string fileName, string pageID, string ownerID, Boolean thumbnail=true)
        {
            bool flag = false;
            try
            {
               
                byte[] fs    = Convert.FromBase64String(base64String);
                //fileName = Guid.NewGuid().ToString().Replace("-", "") + "." + fileName.Split('.')[1];

                //获取上传案例图片路径
                string fullpath = System.Web.HttpContext.Current.Server.MapPath(path);
                //string fullpath = @"D:\Work\yaodaibao\Code\Release\Files";/// 调试用
                if (!Directory.Exists(fullpath))
                {
                    Directory.CreateDirectory(fullpath);
                }
                //定义并实例化一个内存流，以存放提交上来的字节数组。
                MemoryStream m = new MemoryStream(fs);
                //定义实际文件对象，保存上载的文件。
                FileStream f = new FileStream(fullpath + "\\" + fileName, FileMode.Create);
                //把内内存里的数据写入物理文件
                m.WriteTo(f);
               
               if(thumbnail)
               {
                   System.Drawing.Image originalImage = System.Drawing.Image.FromStream(m); 
                   System.Drawing.Image img = GetThumbnail(originalImage, 80, 50);
                   img.Save(fullpath + "\\" + "T_" + fileName);
                   img = null;
                   originalImage = null;
               }
                m.Close();
                f.Close();
                f = null;
                m = null;

                string sql = "";

                sql = "INSERT INTO Attachments(FPageID,FOwnerID,FFileName,FPath)VALUES('" + pageID + "','" + ownerID + "','" + fileName + "','" + path + "')";
                SQLServerHelper runner = new SQLServerHelper();
                runner.ExecuteSqlNone(sql);
                runner = null;
                flag = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return flag;
        }
        private static System.Drawing.Image GetThumbnail(System.Drawing.Image image, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            //从Bitmap创建一个System.Drawing.Graphics
            System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(bmp);
            //设置 
            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //下面这个也设成高质量
            gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            //下面这个设成High
            gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //把原始图像绘制成上面所设置宽高的缩小图
            System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, width, height);

            gr.DrawImage(image, rectDestination, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
            return bmp;
        }

        //根据base64传过来的data判断
        public static bool UploadFile(string base64String, string path,long fileID,string Name,string EmployeeID)
        {
            bool flag = false;
            try
            {
                byte[] fs = Convert.FromBase64String(base64String);
               
                string fileSize = ((base64String.Replace("=", "").Length/8)*2).ToString();

                //获取上传案例图片路径
                 string fullpath = System.Web.HttpContext.Current.Server.MapPath(path);
                //string fullpath = @"D:\Work\yaodaibao\Code\Release\Files";/// 调试用
                if (!Directory.Exists(fullpath))
                {
                    Directory.CreateDirectory(fullpath);
                }
                //定义并实例化一个内存流，以存放提交上来的字节数组。
                MemoryStream m = new MemoryStream(fs);
                //定义实际文件对象，保存上载的文件。
                FileStream f = new FileStream(fullpath + "\\" + fileID, FileMode.Create);
                //把内内存里的数据写入物理文件
                m.WriteTo(f);
                m.Close();
                f.Close();
                f = null;
                m = null;           
                SQLServerHelper runner = new SQLServerHelper();
                string sql = $"insert into [yaodaibao].[dbo].[CTP_FILE](ID,FILENAME,MIME_TYPE,CREATE_DATE,CREATE_MEMBER,FILE_SIZE) values('{fileID}','{Name}','{"text/plain"}','{DateTime.Now.ToString()}','{EmployeeID}','{fileSize}')";
                runner.ExecuteSqlNone(sql);
                runner = null;
                flag = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return flag;
        }


        private static string GetFileExtension(string base64String)
        {
            var data = base64String.Substring(0, 5);
            switch (data.ToUpper())
            {
                case "IVBOR":
                    return "png";
                case "/9J/4":
                    return "jpg";
                case "JVBER":
                    return "pdf";
                case "AAABA":
                    return "ico";
                case "UMFYI":
                    return "rar";
                case "E1XYD":
                    return "rtf";
                case "U1PKC":
                    return "txt";
                case "MQOWM":
                case "77U/M":
                    return "srt";
                default:
                    return string.Empty;
            }
        }
        /// <summary>
        /// 返回真实的文件类型
        /// </summary>
        /// <param name="path">文件路径</param>

        public static string CheckTrueFileName(string path)
        {
            string bx = "";
            try
            {
                System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                System.IO.BinaryReader r = new System.IO.BinaryReader(fs);
                byte buffer;
                buffer = r.ReadByte();
                bx += buffer.ToString();
                r.Close();
                fs.Close();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
;
            return bx;
            ////
            //Console.WriteLine(bx);
            ////文件名，包括格式
            //Console.WriteLine(System.IO.Path.GetFileName(path));
            ////文件名， 不包括格式
            //Console.WriteLine(System.IO.Path.GetFileNameWithoutExtension(path));
            ////文件格式
            //Console.WriteLine(System.IO.Path.GetExtension(path));
            //Console.ReadLine();
        }

        public enum FileExtension
        {
            JPG = 255216,
            GIF = 7173,
            BMP = 6677,
            PNG = 13780,
            COM = 7790,
            EXE = 7790,
            DLL = 7790,
            RAR = 8297,
            ZIP = 8075,
            XML = 6063,
            HTML = 6033,
            ASPX = 239187,
            CS = 117115,
            JS = 119105,
            TXT = 210187,
            SQL = 255254,
            BAT = 64101,
            BTSEED = 10056,
            RDP = 255254,
            PSD = 5666,
            PDF = 3780,
            CHM = 7384,
            LOG = 70105,
            REG = 8269,
            HLP = 6395,
            DOC = 208207,
            XLS = 208207,
            DOCX = 208207,
            XLSX = 208207,
        }
    }
}

