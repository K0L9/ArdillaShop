using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArdillaShop.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public uint Age { get; set; }
        public string Gender { get; set; }
        public string TelegramName { get; set; }
        public string FacebookProfile { get; set; }
    }
}
