using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAWEBJollybee.Areas.User.Controllers
{
    public class ContactController : Controller
    {
        // GET: User/Contact
        public ActionResult Index()
        {
            Session["CurrentController"] = "Contact";
            Session["CurrentAction"] = "Index";
            Session["idpage"] = 0;
            return View();
        }
    }
}