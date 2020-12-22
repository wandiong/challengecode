using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CSV_searcher.Controllers
{
    public class ModuleController : Controller
    {
        //to generate character
        public string getRandomCharacter(int contentNumber)
        {
            string texts = " abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(texts, contentNumber).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}