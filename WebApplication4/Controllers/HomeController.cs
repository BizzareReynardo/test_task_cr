using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using WebApplication4.Models;
using System.Drawing;

namespace WebApplication4.Controllers
{
    //Основной контроллер тестового задания. В нём происходят некоторые вычисления для формирования представлений, а также
    //осуществлены переходы между различными страницами веб-приложения.
    public class HomeController : Controller
    {
        
        ImageContext db = new ImageContext(); //объявляем переменную, в которой содержится созданная ранее БД Изображений (Imag)
        //Контроллер Index - контроллер главной страницы веб-приложения.
        public ActionResult Index()
        {

            IEnumerable<Imag> images = db.Images; //формируем переменную типа IEnumerable (Коллекция полученных из БД изображений)
            ViewBag.Images = images; //эта коллекция передаётся во ViewBag. Это один из способов вывода полученных данных в представлении

            return View();

        }

        //Post-метод в основном представлении используется для изменения состояния БД (при помощи checkbox'ов из представления Index
        //мы узнаём, какие изображения должны быть показаны на странице Gallery), после чего происходит перенаправление
        //к контроллеру представления Gallery
        [HttpPost]
        public ActionResult Index(string checked_images)
        {
            int index = 1; //для отслеживания индекса внутри foreach (как один из возможных методов) 
            Imag image; //инициализация объекта Изображения (Imag) для работы внутри foreach
            foreach (char symb in checked_images) {
                image = db.Images.Find(index); //загружаем в созданный ранее объект элемент БД с указанным индексом index

                //Схема работы foreach - в неё из представления Index передаётся созданная в процессе предварительной обработки
                //формы строка, состоящая из единиц и нулей - эквивалент true и false соответственно при проверке истинности
                //checkbox'ов в представлении Index. В этом foreach строка посимвольно разбирается.
                //Если символ (symb) равен 1, то это изображение должно быть показано в галерее, поэтому меняется значение
                //свойства isCheckd - оно становится равным единице. Соответственно, если symb равен 0 (другие варианты не
                //предполагаются), то и в isCheckd оказывается ноль.

                if (symb == '1')
                {
                    image.isCheckd = 1;
                }
                else {
                    image.isCheckd = 0;
                }
                db.SaveChanges(); //cохраняются изменения, внесённые в БД изображений (Imag)
                index++; //увеличивается индекс для дальнейшего осмотра изображений
            }
            return RedirectToAction("Galery"); //перенаправление к контроллеру представления Gallery
        }

        //При создании представления была допущена ошибка в названии, сразу она замечена не была,
        //поэтому файл в Controller'e называется Galery, как и само представление. Однако в самом
        //представлении, в Title, стоит Gallery.
        [HttpGet]
        public ActionResult Galery()
        {
            IEnumerable<Imag> images = db.Images.Where(b => b.isCheckd == 1).ToList(); //берём все элементы БД изображений (Imag) со значением свойства isCheckd == 1
            foreach(var b in images){

                //Схема работы данного foreach - все отобранные ранее изображения имеют в себе свойство File формата byte[].
                //Это свойство, при помощи класса MemoryStream, можно преобразовать в изображение Image (не путать с классом Imag),
                //и обратно, в byte[]. Здесь, в инициализации данного представления, это делается с целью создания "сжатой копии"
                //файла - так сказать, его "превью-версии". Берётся массив байтов, переводится при помощи MemoryStream в изображение,
                //и при помощи самописной функции меняется с жёстко заданным размером - 320х260. После этого, полученное небольшое
                //изображение переводится обратно в byte[] при помощи нового экземпляра MemoryStream и заменяет значение в значении
                //свойства File.
                //Внимание, в конце преобразований нет строки db.SaveChanges(), это значит, что изменения, производимые в инициализации
                //контроллера представления Gallery не влияют на элементы самой БД изображений (Imag).

                MemoryStream ms = new MemoryStream(b.File, 0, b.File.Length); //инициализация переменной класса MemoryStream
                ms.Write(b.File, 0, b.File.Length); //запись в переменную содержимого свойства File
                Image returnImage = Image.FromStream(ms); //содержимое из ms записывается в объект класса Image (не путать с Imag)
                Image thumb_img = resizeImage(returnImage, new Size(320, 260)); //вызов самописной функции, ресайз изображения до 320х260
                ms.Dispose(); //удаление ранее инициализированной переменной из памяти для очистки занимаемой памяти
                MemoryStream ms1 = new MemoryStream(); //инициализация второй переменной класса MemoryStream
                thumb_img.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg); //запись в переменную содержимого уменьшенного изображения в формате Jpeg
                b.File = ms1.ToArray(); //сохранение содержимого уменьшенного изображения в свойстве File
            }
            
            ViewBag.Images = images; //Во ViewBag помещается преобразованная коллекция изображений из БД
            return View();
        }

        //Вызов частичного представления вывода комментария к изображению, если таковой имеется (проверка наличия комментария выполняется на стороне клиента)
        public ActionResult FullComment (int id){

            IEnumerable<Imag> images = db.Images; //берём данные из БД изображений (Imag)
            Imag need_img = images.ElementAt(id - 1); //выбираем единственное нужное нам изображение по индексу
            if (need_img.Cmnt != null)
                return PartialView(need_img); //если есть комментарий у выбранного изображения, то мы выводим его на сайте
            return HttpNotFound(); //на случай непредвиденной возможности запуска представления при отсутствии комментария вызывается HttpNotFound()
            
       }

        //Вызов частичного представления добавления новых изображений в БД Изображений (Imag)
        [HttpGet]
        public ActionResult AddImage()
        {
            return PartialView();
        }

        //В данном контроллере отображены действия, производимые при отправке формы из представления AddImage
        [HttpPost]
        public ActionResult AddImage(Imag image, HttpPostedFileBase neededFile)
        {
            //Проверка заполненности всех необходимых полей производится со стороны клиента, без перегрузки страницы
            //однако, проверку на формат загружаемого файла удобнее разместить на стороне сервера.
            var WorkingImageExtension = Path.GetExtension(neededFile.FileName).ToLower(); //получаем расширение загружаемого файла
            string[] allowedExtensions = { ".png", ".jpeg", ".jpg", ".gif" }; //допустимые расширения для загрузки в БД изображений (Imag)

            //В данном if проверяется валидность заполнения полей(также включена проверка со стороны сервера),
            //наличие файла в передаваемой переменной neededFile (такая же проверка осуществялется и на стороне клиента, здесь
            //эта проверка используется для дополнительной надёжности) и проверка расширения загружаемого файла, вернее,
            //его наличия в обозначенном массиве допустимых расширений. Если все условия соблюдены, то мы можем
            //загрузить файл в нашу БД.

            if (ModelState.IsValid && neededFile != null && allowedExtensions.Contains(WorkingImageExtension))
            {
                byte[] imageData = null; //формируется переменная, в которую будет помещено содержимое загруженного изображения
                BinaryReader binaryReader = new BinaryReader(neededFile.InputStream); //инициализация ридера для считывания содержимого изображения

                imageData = binaryReader.ReadBytes(neededFile.ContentLength); //побайтовое считывание изображения



                image.File = imageData; //в свойство File загружается получившийся массив byte[]
                image.FileDateTime = DateTime.Now; //в свойство FileDateTime загружается текущая дата, соответствующая дате загрузки изображения в БД

                db.Images.Add(image); //инициализируем запрос на добавление нового объекта Imag в БД изображений
                db.SaveChanges(); //сохраняем изменения в БД изображений
                return RedirectToAction("Index"); //после успешной итерации возвращаемся к представлению Index
            }
            else {
                //Если вышеобозначенное условие не выполняется, то мы предупреждаем об этом клиента выводом сообщения
                //ErrorMessage
                ViewBag.ErrorMessage = "Вы можете загружать только изображения формата *.jpeg, *.png, *.jpg, *.gif!"; //Во ViewBag загружается переменная ErrorMessage
                return View("Index"); //Выводим представление с представленной переменной ErrorMessage.
            }
            
        }

        //Самописная функция ресайза изображений
        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size)); //создаёт объект класса Bitmap на основании указанного изображения и присланных размеров
                                                           //после чего конвертирует полученный объект в формат Image
        }


        //Контроллеры из стартовой версии проекта, не используются
       /* public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }*/
    }
}