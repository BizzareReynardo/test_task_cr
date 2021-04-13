using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Drawing;

namespace WebApplication4.Models
{
    //Инициализация созданной ранее базы (закомментированный код использовался для изначального заполнения базы файлами из папки "Content". В данный момент он не нужен, остался 
    //лишь как напоминание в тестовом задании
    public class ImageInitializater : DropCreateDatabaseAlways<ImageContext>
    {
        protected override void Seed(ImageContext db)
        {
            /*string PathToFolder = "/Users/User/Documents/Visual Studio 2013/Projects/WebApplication4/WebApplication4/Content/Upload/";
            IEnumerable<string> allImages = Directory.EnumerateFiles(PathToFolder);

            Image filecontent;
            int id = 0;

            foreach (var img in allImages) {
                filecontent = Image.FromFile(img);
                var filenm = Path.GetFileName(img);
                filenm = filenm.Remove(filenm.IndexOf('.'));
                MemoryStream ms = new MemoryStream();
                filecontent.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] byteImage = ms.ToArray();

                
                db.Images.Add(new Imag { Id = id, FileName = filenm, Author = "Yaz", Cmnt = "Рандомно написанное сообщение", FileDateTime = DateTime.Now.Date, File = byteImage });
                id++;
            }*/

            base.Seed(db);
        }
    }
}