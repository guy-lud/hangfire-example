using System;
using ClassLibrary1;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace hangfire_example.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public HomeController(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public IActionResult Index(string message = null)
        {
	        ViewBag.Message = message;

			return View();
        }

        public IActionResult AddJobToHangfire()
        {

            _backgroundJobClient.Enqueue<IX<Data>>(x => x.Do(new Data(){Name = "hello"}));

	        return RedirectToAction("Index", new { message = "Hangfire job added" });
        }

        public IActionResult Error()
        {
            return View();
        }

	    public void JobToAdd()
	    {
			Console.WriteLine("Some line in console...");
	    }
    }
}
