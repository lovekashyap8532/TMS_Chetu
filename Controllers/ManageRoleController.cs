using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    //[Authorize]
    public class ManageRoleController : Controller
    {
        private UserManager<IdentityUserNew> userManager { get; set; }
        private RoleManager<IdentityRole> roleManager { get; set; }

        public ManageRoleController(
            RoleManager<IdentityRole> _roleManager,
            UserManager<IdentityUserNew> _userManager
            )
        {
            userManager = _userManager;
            roleManager = _roleManager;
        }

        public async Task<IActionResult> GetAllRoles()
        {
            var rs = await roleManager.Roles.ToListAsync();
            return View(rs);
        }
        //[Authorize(Roles = "Admin")]
        public IActionResult AddRole()
        {
            return View();
        }
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddRole(string role)
        {
            if (role == null)
            {
                ModelState.AddModelError("", "Please enter a Role");
                return View();
            }
            else
            {
                var r = await roleManager.FindByNameAsync(role);
                if (r == null)
                {
                    var rol = new IdentityRole() { Name = role };
                    var res = await roleManager.CreateAsync(rol);
                    if (res.Succeeded)
                    {
                        return RedirectToAction("GetAllRoles", "ManageRole");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Role did not created");
                        return View();
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Role already Exists");
                    return View();
                }
            }
            //return View();
        }
        public async Task<IActionResult> EditRole(string Id)
        {
            var r = await roleManager.FindByIdAsync(Id);
            return View(r);
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(string role,string Id)
        {
            if (role != null && Id != null)
            {
                var editrole = await roleManager.FindByIdAsync(Id);
                if (editrole != null)
                {
                    editrole.Name = role;
                    var res = await roleManager.UpdateAsync(editrole);
                    if (res.Succeeded)
                        return RedirectToAction("GetAllRoles", "ManageRole");
                    else
                        return View();

                }
                else
                {
                    ModelState.AddModelError("", "Role not found !!");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("", "Details not found !!");
                return View();
            }
        }


        public async Task<IActionResult> DisplayAllUsers()
        {
            var res = await userManager.Users.ToListAsync();
            return View(res);
        }
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignedRole(string id)
        {
            ViewBag.msg = TempData["msg"];
            var user = await userManager.FindByIdAsync(id);
            ViewBag.UserName = user.FirstName + " " + user.LastName;
            var allroles = await roleManager.Roles.ToListAsync();
            List<UserRole> users = new List<UserRole>();
            foreach (var role in allroles)
            {
                UserRole vm = new UserRole();
                vm.Id = role.Id;
                vm.Name = role.Name;
                vm.UserId = id;
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    vm.IsAssigned = true;
                }
                else
                {
                    vm.IsAssigned = false;
                }
                users.Add(vm);


            }
            return View(users);
        }
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AssignedRole(List<UserRole> userRoles)
        {
            string msg = string.Empty;
            var user = await userManager.FindByIdAsync(userRoles[0].UserId);
            foreach (var role in userRoles)
            {
                if (role.IsAssigned && await userManager.IsInRoleAsync(user, role.Name) == false)
                {
                    if (role.IsAssigned)
                    {
                        var res = await userManager.AddToRoleAsync(user, role.Name);
                        if (res.Succeeded)
                        {
                            msg = "Data Saved Successfully !!";
                        }
                    }
                }
                else if (role.IsAssigned == false && await userManager.IsInRoleAsync(user, role.Name) == true)
                {
                    var res = await userManager.RemoveFromRoleAsync(user, role.Name);
                    if (res.Succeeded)
                    {
                        msg = "Data Saved Successfully !!";
                    }

                }
                TempData["msg"] = msg;
            }
            return RedirectToAction("AssignedRole");
        }
    }
}
