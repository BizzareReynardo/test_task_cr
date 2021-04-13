using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    //Файл основной модели тестового задания - Изображений
    public class Imag
    {
        public int Id { get; set; } //id модели
        [Required(ErrorMessage="Необходимо указать название файла!")]
        public string FileName { get; set; } //наименование файла модели в базе
        public DateTime FileDateTime { get; set; } //время загрузки изображения в БД
        public byte[] File { get; set; } //собственно говоря, само изображение, сохранённое в формате byte[]
        [Required(ErrorMessage = "Необходимо указать имя автора!")]
        public string Author { get; set; } //имя автора
        public string Cmnt { get; set; } //комментарий к изображению, если таковой имеется
        public int isCheckd { get; set; } //поле для обозначения, нужно ли отображать данный файл в галерее, или нет

    }
}