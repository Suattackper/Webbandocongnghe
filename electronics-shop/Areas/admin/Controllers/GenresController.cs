﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace electronics_shop.Areas.Admin.Controllers
{
    public class GenresController : Controller
    {
        // GET: Admin/Genres
        public ActionResult Index()
        {
            return View();
        }
    }
}