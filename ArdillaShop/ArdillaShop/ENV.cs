using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArdillaShop
{
    public class ENV
    {
        //Logo pathes
        public const string LogoImagePath = @"\images\logos\";
        public const string LongLogoImagePath = LogoImagePath + "logo_long.png";
        public const string BigLogoImagePath = LogoImagePath + "ready_logo_black.png";

        public const string ImagePath = @"\images\products\";
        public const string NoImagePath = @"\images\no_image.jpg";
        public const string NoImageName = @"\no_image.jpg";

        public const string SessionCart = "N*5j4vC4mgY*h%3%7@Qfrd8v_28BUP8B";

        public static string AdminRole = "Admin";
        public static string CustomerRole= "Customer";

        public const string AdminEmail = "kovalkola2@gmail.com";
    }
}
