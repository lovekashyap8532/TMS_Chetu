using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManagementSystem.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskManagementSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDBContext dbContext;
        private UserManager<IdentityUserNew> userManager { get; set; }
        private RoleManager<IdentityRole> roleManager { get; set; }


        public HomeController(
            RoleManager<IdentityRole> _roleManager,
            UserManager<IdentityUserNew> _userManager,
            ApplicationDBContext _dbContext
            )
        {
            dbContext = _dbContext;
            userManager = _userManager;
            roleManager = _roleManager;
        }
        public IActionResult HomeIndex()
        {
            return View();
        }
        public async Task<IActionResult> ShowAllUsers()
        {
            var user = await dbContext.IdentityUserNews.ToListAsync();
            return View(user);
        }
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await dbContext.IdentityUserNews.SingleOrDefaultAsync(e=>e.Id == id);
            if(user!=null)
            {
                return View(user);
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(IdentityUserNew user)
        {
            var edituser = await userManager.FindByIdAsync(user.Id);
            if(edituser!=null)
            {
                edituser.FirstName = user.FirstName;
                edituser.LastName = user.LastName;
                edituser.Email = user.Email;
                var res = await userManager.UpdateAsync(edituser);
                if (res.Succeeded)
                    return RedirectToAction("ShowAllUsers", "Home");
                else
                    return View();

            }
            else
            {
                ModelState.AddModelError("", "User not found !!");
                return View();
            }
        }
        public async Task<IActionResult> ReadAllTasks(string state)
        {
            //ViewBag.update = TempData["msg"];
            if (state == "E")
            {
                var task = await dbContext
                    .Tasks
                    .Include(e => e.AssignedTo)
                    .Where(e => e.DueDate <= DateTime.Now /*&& e.AssignedTo.Email == User.Identity.Name*/ && e.Status != "Completed")
                    .OrderBy(d=>d.DueDate)
                    .ToListAsync();
                return View(task);
            }
            else if (state == "C")
            {
                var task = await dbContext
                    .Tasks
                    .Include(e => e.AssignedTo)
                    .Where(e => e.Status == "Completed"/* && e.AssignedTo.Email == User.Identity.Name*/)
                    .OrderBy(d=>d.DueDate)
                    .ToListAsync();
                return View(task);
            }
            else
            {
                var x = await dbContext.Tasks
                .Include(e => e.AssignedTo)
                .Where(e => e.Status != "Completed" && e.DueDate>=DateTime.Now)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
            return View(x);
            }
        }
        public async Task<IActionResult> ReadAllUserTasks()
        {
            var userTasks = await dbContext.Tasks
                                           .Include(e => e.AssignedTo)
                                           .Where(t => t.AssignedTo.Email == User.Identity.Name && t.Status != "Completed" && t.DueDate>=DateTime.Now)
                                           .OrderBy(d=>d.DueDate)
                                           .ToListAsync();

            return View(userTasks);
        }
        public async Task<IActionResult> GetEmployee()
        {
            var emp = await dbContext.IdentityUserNews.ToListAsync();
            return Json(emp);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAssignedUser(int taskId, string? user_id)
        {
            var use = dbContext.IdentityUserNews.SingleOrDefault(e => e.Id == user_id);
            if (taskId == 0)
            {
                return Json(new { success = false, message = "Invalid task ID" });
            }

            var task = await dbContext.Tasks.FindAsync(taskId);
            if (task == null)
            {
                return Json(new { success = false, message = "Task not found" });
            }

            // Update AssignedTo field
            task.AssignedTo = use;
            await dbContext.SaveChangesAsync();

            return Json(new { success = true });
        }


        [Authorize(Roles = "Admin")]
        public IActionResult CreateTask()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskVM tasks)
        {
            if (tasks == null)
            {
                ModelState.AddModelError("", "Please Complete the details");
                return View();
            }
            else if (tasks != null)
            {
                var cdt = tasks.CreatedDate = DateTime.UtcNow;
                //var t = dbContext.Tasks.SingleOrDefault(e => e.Id == tasks.Id);
                if (tasks.DueDate <= cdt)
                {
                    ModelState.AddModelError("DueDate", "Due Date must be greater than Today's Date.");
                }
                else /*if (t == null)*/
                {
                    //var user = dbContext.IdentityUserNews.SingleOrDefault(e => e.Id == userId);
                    TaskModel taskk = new TaskModel
                    {
                        Title = tasks.Title,
                        Description = tasks.Description,
                        //StartDate = tasks.StartDate,
                        DueDate = tasks.DueDate,
                        Status = tasks.Status,
                        Priority = tasks.Priority,
                        CreatedDate = cdt
                        //AssignedTo = user,
                        //AssignedDate = DateTime.UtcNow
                    };
                    await dbContext.Tasks.AddAsync(taskk);
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction("ReadAllTasks", "Home");
                }
                //else
                //{
                //    ModelState.AddModelError("", "Task Already Exist");
                //    return View();
                //}
            }
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult EditTask(int id)
        {
            /*var user = dbContext.IdentityUserNews.SingleOrDefaultAsync(e => e.Email == currentUser);*/
            //if (User.IsInRole("Admin"))
            //{   
            var t = dbContext.Tasks.Include(e => e.AssignedTo).ToList().SingleOrDefault(e => e.Id == id);
            if (t.Status == "Pending")
            {
                return View(t);
            }
            else
            {
                TempData["msg"] = "You Can't update the Task because it is not now in Pending";
                return RedirectToAction("ReadAllTasks", "Home");
            }

            //}
            //else
            //{
            //return RedirectToAction("EditUserTask", "Home");
            //}

            /*var task = dbContext.Tasks.SingleOrDefault(e => e.Id == id);*/
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditTask(TaskModel task/*, string user_id*/)
        {
            if (task == null /*&& user_id == null*/)
            {
                ModelState.AddModelError("", "Please Complete the details");
                return View();
            }
            else if (task != null /*&& user_id != null*/)
            {
                var cdt = task.CreatedDate = DateTime.UtcNow;
                //var t = dbContext.Tasks.SingleOrDefault(e => e.Id == tasks.Id);
                if (task.DueDate <= cdt)
                {
                    ModelState.AddModelError("DueDate", "Due Date must be greater than Today's Date.");
                    return View();
                }
                var t = await dbContext.Tasks.SingleOrDefaultAsync(e => e.Id == task.Id);
                if (t != null)
                {
                    //var user = await dbContext.IdentityUserNews.SingleOrDefaultAsync(e => e.Id == user_id);

                    t.Title = task.Title;
                    t.Description = task.Description;
                    //t.StartDate = task.StartDate;
                    t.DueDate = task.DueDate;
                    //t.Status = task.Status;
                    t.Priority = task.Priority;
                    //t.AssignedTo = user;
                    //t.AssignedDate = task.AssignedDate;

                    dbContext.Entry(t).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    return RedirectToAction("ReadAllTasks", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Task did not Exist");
                    return View();
                }
            }
            return View();
        }
        [Authorize(Roles = "Users")]
        public async Task<IActionResult> EditUserTask(int id)
        {
            
            return View();
        }
        
        [Authorize(Roles = "Users")]
        [HttpPost]
        public async Task<IActionResult> EditUserTask(TaskModel usertask)
        {
             
            var taskuid = await dbContext.Tasks.SingleOrDefaultAsync(e => e.Id == usertask.Id);
            if(usertask.Status == null && usertask.Id != null)
            {
                var st = taskuid.Status = "InProgress";
                var sd = taskuid.StartDate = DateTime.UtcNow;
                dbContext.Entry(taskuid).State = EntityState.Modified;
                TaskProgress task = new TaskProgress()
                {
                    Title = taskuid.Title,
                    Taskss = taskuid,
                    TaskStatus = st,
                    CreatedDate = taskuid.CreatedDate,
                    StartDate = sd,
                    UpdatedDate = DateTime.UtcNow,
                    DueDate = taskuid.DueDate,
                    Description = "You have Started the Task Successfully !!"

                };
                await dbContext.AddAsync(task);
                await dbContext.SaveChangesAsync();
                return RedirectToAction("ReadAllUserTasks", "Home");
            }
            else if (taskuid!=null)
            {
                TaskProgress task = new TaskProgress()
                {
                    Title=taskuid.Title,
                    Taskss = taskuid,
                    TaskStatus=usertask.Status,
                    CreatedDate=taskuid.CreatedDate,
                    StartDate=taskuid.StartDate,
                    UpdatedDate = DateTime.UtcNow,
                    DueDate=taskuid.DueDate,
                    Description = usertask.Description
                    
                };
                taskuid.Status = usertask.Status;
                await dbContext.AddAsync(task);
                dbContext.Entry(taskuid).State = EntityState.Modified;
                dbContext.SaveChanges();
                //return RedirectToAction("ReadAllTasks", "Home");
                return Json(new { success = true, message = "Operation successful" });

            }
                return View();
        }
        public async Task<IActionResult> DeleteTask(int id)
        {
            var del = await dbContext.Tasks.SingleOrDefaultAsync(e => e.Id == id);
            if (del != null)
            {
                dbContext.Remove(del);
                //dbContext.SaveChanges();
                return RedirectToAction("ReadAllTasks", "Home");
            }
            return View();

            //var task = await dbContext.Tasks.FindAsync(id);
            //if (task == null)
            //{
            //    return NotFound();
            //}

            //dbContext.Tasks.Remove(task);
            ////await dbContext.SaveChangesAsync();

            //TempData["msg"] = "Task deleted successfully!";
            //return RedirectToAction("ReadAllTasks");
        }

        //public async Task<IActionResult> ShowTaskProgress(int id)
        //{
        //    var tskid = await dbContext.Tasks.SingleOrDefaultAsync(e => e.Id==id);

        //    var task = await dbContext.TasksProgress
        //        .Include(e => e.Taskss)
        //        .Where(e=>e.Taskss==tskid)
        //        .ToListAsync();
        //    if(task==null)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        return Json(task);
        //    }
        //}
        public async Task<IActionResult> ShowTaskProgress(int id)
        {
               var tskid = await dbContext.Tasks.SingleOrDefaultAsync(e => e.Id==id);

            var task = await dbContext.TasksProgress
                .Include(e=>e.Taskss)
                .Where(tp => tp.Taskss == tskid) // Assuming TaskId is the foreign key in TaskProgress
                .ToListAsync();

            if (!task.Any())
            {
                return Json(new { message = "No records found" }); // Return message instead of NotFound()
            }
             
            return Json(task);
        }
        public async Task<IActionResult> ShowCompleteTask(string state)
        {
            if(state=="E")
            {
                var task = await dbContext
                    .Tasks
                    .Include(e => e.AssignedTo)
                    .Where(e => e.DueDate<= DateTime.Now && e.AssignedTo.Email == User.Identity.Name && e.Status!="Completed")
                    .ToListAsync();
                return View(task);
            }
            else if(state=="C")
            {
                var task = await dbContext
                    .Tasks
                    .Include(e=>e.AssignedTo)
                    .Where(e => e.Status == "Completed" && e.AssignedTo.Email == User.Identity.Name)
                    .ToListAsync();
                return View(task);
            }
            else
            {
                return View(new { message = "No records found" });
            }
        }
    }
}
