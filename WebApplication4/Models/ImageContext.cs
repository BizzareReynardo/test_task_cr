using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    //Класс для объявления в БД, созданной при помощи EntityFramework, базы, завязанной на созданном ранее классе изображений - Imag
    public class ImageContext : DbContext
    {
        public DbSet<Imag> Images { get; set; }

    }
}