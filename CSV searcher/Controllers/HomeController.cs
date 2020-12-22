using System;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.Collections.Generic;
using System.Configuration;
using System.Collections;
using System.IO;

namespace CSV_searcher.Controllers
{
    public class HomeController : Controller
    {
        #region init
        ModuleController modules = new ModuleController();
        #endregion
        #region variables
        string fileLocation = ConfigurationManager.AppSettings["location"];
        #endregion
        #region classes

        //for return as AJAX
        public class csvData
        {
            public string id { get; set; }
            public string content { get; set; }
            public int matchTimes { get; set; }
        }
        #endregion
        public class insert
        {
            public List<csvData> list { get; set; }
            public string searchVal { get; set; }
            public int count { get; set; }
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Search(string value)
        {
            insert list = new insert();
            list.list = GetCSVAsync(value);
            list.count = list.list.Count();
            list.searchVal = value;
            Dispose();
            return View(list);
        }

        public ActionResult Generate()
        {
            ViewBag.isSuccess = false;
            try
            {
                #region variables
                
                int rows = 100000;

                int lengths = 1000; //UTF 8 1 KB means 1000 characters


                #endregion

                #region generateCsv
                //create title first, or else we wont know which one is the ID and which one is the content
                string csvcontent = "";

                for (var i = 0; i < rows; i++)
                {
                    csvcontent += Guid.NewGuid() + "," + modules.getRandomCharacter(lengths) + System.Environment.NewLine;
                    Console.WriteLine(i);
                    Dispose();
                }
                System.IO.File.WriteAllText(fileLocation, csvcontent);
                #endregion
                ViewBag.Updated = "Yes";
            }
            catch (Exception err)
            {
                ViewBag.isSuccess = false;
            }

            return RedirectToAction("Index", "Home");
        }


        private int match(string stringContent,string searchcontent)
        {
            #region variables
            int matchCount = 0;
            int contentLength = stringContent.Length;
            int position = 0;
            string remaining = "";
            #endregion
            #region logic

            while (contentLength >= searchcontent.Length)
            {
                remaining = stringContent.Substring(position, searchcontent.Length);
                if (remaining.Equals(searchcontent))
                {
                    matchCount += 1;
                }
                position += 1;
                contentLength -= searchcontent.Length;
                //clean memory again. make sure things keep gone smoothly despite of the slow performance
                Dispose();
            }
            #endregion
            return matchCount;
        }
        public List<csvData> GetCSVAsync(string value)
        {
            #region variables
            List<csvData> res = new List<csvData>();
            #endregion
            #region logic
            using(StreamReader reader = new StreamReader(fileLocation))
            {
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        //if the next line is empty, then time to break it;
                        break;
                    }
                    
                    var arrContent = line.Split(',');
                    int matchRes = match(arrContent[1], value);
                    if (matchRes > 0)
                    {
                        csvData tempCsvData = new csvData();
                        tempCsvData.id = arrContent[0];
                        tempCsvData.content = arrContent[1];
                        tempCsvData.matchTimes = matchRes;
                        res.Add(tempCsvData);
                    }

                    //everytime the looping done, dispose the resources. or else, it will be pilled to 4GB then boom! crash.
                    Dispose();
                }
                Dispose();
            }
            #endregion
            return res;
        }

    }
}