using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using UserManegmentWithIdentity.Models;
using UserManegmentWithIdentity.Services;
using UserManegmentWithIdentity.ViewModels;

namespace UserManegmentWithIdentity.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task <IActionResult> Index()
        {
          
            var users = await _userManager.Users.Select(user => new UserVM
            {
                Id = user.Id,
                UserName = user.UserName,   
                FirstName = user.FirstName,
                LastName=user.LastName,
                Email=user.Email,
                Roles=_userManager.GetRolesAsync(user).Result
            }).ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Add()
        {
           

            var roles = await _roleManager.Roles.Select(r=> new RoleVM { RoleId=r.Id,RoleName=r.Name}).ToListAsync();
            var viewModel = new AddUserVM
            {
               
                Roles = roles
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddUserVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (!model.Roles.Any(r=>r.IsSelected))
            {
                ModelState.AddModelError("Roles", "You must Select At Least One Role");
                return View(model);
            }
            if (await _userManager.FindByEmailAsync(model.Email)!=null)
            {
                ModelState.AddModelError("Email", "Email is Already Exist");
                return View(model);
            }
            if (await _userManager.FindByNameAsync(model.UserName) != null)
            {
                ModelState.AddModelError("UserName", "UserName is Already Exist");
                return View(model);
            }
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email=model.Email,  
                FirstName=model.FirstName,
                LastName=model.LastName
                
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {


                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Roles", error.Description);
                }
                return View(model);
            }
            await _userManager.AddToRolesAsync(user, model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName));
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ManageRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return NotFound();
            }

            var roles = await _roleManager.Roles.ToArrayAsync();
            var viewModel = new UserRolesVM
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = roles.Select(role => new RoleVM
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    IsSelected = _userManager.IsInRoleAsync(user, role.Name).Result
                }).ToList() 
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(UserRolesVM model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in model.Roles)
            {
               if (userRoles.Any(r=>r == role.RoleName) && !role.IsSelected)
                {
                    await _userManager.RemoveFromRoleAsync(user , role.RoleName);
                }
                if (!userRoles.Any(r => r == role.RoleName) && role.IsSelected)
                {
                    await _userManager.AddToRoleAsync(user,role.RoleName);
                }

            }
               return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _roleManager.Roles.ToArrayAsync();
            var viewModel = new ProfileFormVM
            {
               Id = user.Id,
               UserName = user.UserName,
               FirstName = user.FirstName,
               LastName = user.LastName,
               Email= user.Email,
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileFormVM model)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }
            var userWithSameEmail = await _userManager.FindByEmailAsync(model.Email);
            if (userWithSameEmail != null &&userWithSameEmail.Id != model.Id  )
            {
                ModelState.AddModelError("Email", "This Email is already asigned to another user");
                return View(model); 
            }
            var userWithSameUserName = await _userManager.FindByNameAsync(model.UserName);
            if (userWithSameUserName != null && userWithSameUserName.Id != model.Id)
            {
                ModelState.AddModelError("UserName", "This User Name is already assigned to another user");
                return View(model);
            }

                 user.Id = model.Id;
                 user.UserName =model.UserName;
                 user.FirstName = model.FirstName;
                 user.LastName = model.LastName;
                 user.Email    =model.Email;  

            await _userManager.UpdateAsync(user);
         return RedirectToAction(nameof(Index));
        }

    }
}
