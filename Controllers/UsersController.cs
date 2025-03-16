using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMEAutomationDBOperations.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Users/UserList
        public async Task<IActionResult> UserList()
        {
            // Asenkron veri çekme işlemi
            var users = await _context.Users.ToListAsync();
            return View(users);
        }
    }
}
