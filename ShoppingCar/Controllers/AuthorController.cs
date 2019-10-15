using Kendo.Mvc.UI;
using Newtonsoft.Json;
using ShoppingCar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace ShoppingCar.Controllers
{
    public class AuthorController : Controller
    {
        // GET: Author
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Author_Get()
        {
            List<Author> authors = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58976/api/");
                //HTTP GET
                var responseTask = client.GetAsync("Authors");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<Author>>();
                    readTask.Wait();

                    authors = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    authors = null;

                    //ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return Content(JsonConvert.SerializeObject(authors), "application/json");
        }
    }
}