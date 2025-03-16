using Microsoft.AspNetCore.Mvc;
using IMEAutomationDBOperations.Models;
using IMEAutomationDBOperations.Services;
using System.Collections.Generic;
using IMEAutomationDBOperations.Data;

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
            List<Student> students = _databaseService.GetStudentsData();
            return View(students);
        }

        public IActionResult KonyaTecnicalUnivercityIMEAutomation()
        {
            return View("KonyaTecnicalUnivercity-IMEAutomation");
        }
    }
}
