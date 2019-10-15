using Kendo.Mvc.UI;
using ShoppingCar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace ShoppingCar.Controllers
{
    public class BookController : Controller
    {
        string apiUrl = "http://localhost:58976/api/";
        // GET: Book
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Book_Get([DataSourceRequest] DataSourceRequest request)
        {
            //https://www.tutorialsteacher.com/webapi/consume-web-api-get-method-in-aspnet-mvc
            //string apiUrl = "http://localhost:58976/api/";
            List<Book> books = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58976/api/");
                //HTTP GET
                var responseTask = client.GetAsync("Books");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<Book>>();
                    readTask.Wait();

                    books = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    books = null;

                    //ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            var datasource = new DataSourceResult()
            {
                Data = books,
            };
            return Json(datasource, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Book_Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Book_Create(Book book)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);

                //HTTP POST
                var postTask = client.PostAsJsonAsync<Book>("Books", book);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View(book);
        }

        public ActionResult Book_Edit(int id)
        {
            Book book = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                //HTTP GET
                var responseTask = client.GetAsync("Books?id=" + id.ToString());
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Book>();
                    readTask.Wait();

                    book = readTask.Result;
                }
            }
            return View(book);
        }

        [HttpPost]
        public ActionResult Book_Edit(Book book)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);

                //HTTP POST
                var putTask = client.PutAsJsonAsync<Book>("Books?Id="+book.Id.ToString(), book);
                putTask.Wait();

                var result = putTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    return RedirectToAction("Index");
                }
            }
            return View(book);
        }

        public ActionResult Book_Delete(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);

                //HTTP DELETE
                var deleteTask = client.DeleteAsync("Books/" + id.ToString());
                deleteTask.Wait();

                var result = deleteTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }
    }
}