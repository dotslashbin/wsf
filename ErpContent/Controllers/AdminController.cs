﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ErpContent.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        public ActionResult Index()
        {
            return View("AdminDashboard");
        }


        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAdmin)]
        public ActionResult Reviewers()
        {
            return View("Reviewers");
        }



        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAdmin)]
        public ActionResult VintageProfile()
        {
            return View("VintageProfile");
        }


        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAdmin)]
        public ActionResult Wines()
        {
            return View("Wines");
        }


        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAdmin)]
        public ActionResult SimilarWines()
        {
            return View("SimilarWines");
        }


        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAdmin)]
        public ActionResult Producers()
        {
            return View("Producers");
        }

        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
        public ActionResult Importers()
        {
            return View("Importers");
        }


        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
        public ActionResult ProducersImporters()
        {
            return View("ProducersImporters");
        }



        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAdmin)]
        public ActionResult Articles()
        {
            return View("Articles");
        }

        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAdmin)]
        public ActionResult Status()
        {
            return View("Status");
        }


        [System.Web.Mvc.Authorize(Roles =  EditorsCommon.Constants.roleNameAdmin)]
        public ActionResult Users()
        {
            return View("Users");
        }
    }

}