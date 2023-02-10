using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManegmentWithIdentity.Services;
using UserManegmentWithIdentity.ViewModels;

namespace UserManegmentWithIdentity.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(RoleFormVM model)
        {
            if (ModelState.IsValid)
            {
                if (await _roleManager.RoleExistsAsync(model.Name))
                {
                    ModelState.AddModelError("Name" ,"Role Already Exist");
                    return View("index", await _roleManager.Roles.ToListAsync());
                }
                await _roleManager.CreateAsync( new IdentityRole(model.Name.Trim()) );
                return RedirectToAction(nameof(Index));   
            }
            else
            {
                return View("index", await _roleManager.Roles.ToListAsync());
            }
            
        }
    }     
}
