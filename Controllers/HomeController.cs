using Microsoft.AspNetCore.Mvc;
using IMEAutomationDBOperations.Models;
using IMEAutomationDBOperations.Services;
using System.Collections.Generic;

namespace IMEAutomationDBOperations.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseService _databaseService;

        public HomeController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public IActionResult Index()
        {
            List<User> users = _databaseService.GetUsersData();
            return View(users);
        }
    }
}
