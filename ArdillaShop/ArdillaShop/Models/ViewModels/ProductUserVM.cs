﻿using ArdillaShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArdillaShop.Models.ViewModels
{
    public class ProductUserVM
    {

        public ProductUserVM()
        {
            ProductList = new List<Product>();
        }

        public AppUser AppUser { get; set; }
        public List<Product> ProductList { get; set; }

    }

}