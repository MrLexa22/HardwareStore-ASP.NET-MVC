using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using PagedList;
using ShopMagazin.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using PagedList.Mvc;
using PagedList;
using System.Threading;

namespace ShopMagazin.Controllers
{
    public static class OldEmail
    {
        public static string OldE { get; set; }
    }

    public static class AuthenticatedUser
    {
        public static string Ima { get; set; }
        public static string Familia { get; set; }
    }



    public class HomeController : Controller
    {
        private readonly UsersService db;
        private readonly TovariService dbs;
        public HomeController(UsersService context, TovariService cot)
        {
            db = context;
            dbs = cot;
        }

        public async Task<IActionResult> Index(IndexPageModel p)
        {
            p.Categs = await dbs.GetCategories();
            p.podCategoriis = await dbs.GetPodCategories();
            p.Tovari = await dbs.GetSpecialTovari();
            return View(p);
        }

        public IActionResult CheckAuthentication()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<ActionResult> GetImage(string id)
        {
            var image = await dbs.GetImage(id);
            if (image == null)
            {
                return NotFound();
            }
            return File(image, "image/png");
        }

        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> UsersPage(string ZhachPoiska, string ZnachPoiskaRole, int page = 1)
        {
            Debug.WriteLine("sfdfafafffas^: " + ZhachPoiska);
            Debug.WriteLine("1212sfdfafafffas^: " + ZnachPoiskaRole);
            UsersPages h = new UsersPages();
            h.Users = await db.GetUsers();

            int pageSize = 5;
            var count = h.Users.Count();
            var item = h.Users.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            h.PageViewModel = new PageViewModel(count, page, pageSize);
            h.Users = item;

            if (ZnachPoiskaRole == "Все")
                    ZnachPoiskaRole = null;
                h.Users = await db.GetUsersWithFilter(ZhachPoiska, ZnachPoiskaRole);
                if (ZhachPoiska != null)
                    h.ZhachPoiska = ZhachPoiska;
                if (ZnachPoiskaRole != null)
                    h.ZnachPoiskaRole = ZnachPoiskaRole;

            if (ZhachPoiska != null)
            {
                if (ZhachPoiska.Trim() != "")
                {
                    if (ZnachPoiskaRole == "Все")
                    {
                        h.Users = await db.GetUsersWithFilter(ZhachPoiska, null);

                        count = h.Users.Count();
                        item = h.Users.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                        h.PageViewModel = new PageViewModel(count, page, pageSize);
                        h.Users = item;

                        return View(h);
                    }
                    else
                    {
                        count = h.Users.Count();
                        item = h.Users.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                        h.PageViewModel = new PageViewModel(count, page, pageSize);
                        h.Users = item;

                        return View(h);
                    }
                }
            }
            else if (ZnachPoiskaRole != "Все")
            {
                h.Users = await db.GetUsersWithFilter(ZhachPoiska, ZnachPoiskaRole);

                count = h.Users.Count();
                item = h.Users.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                h.PageViewModel = new PageViewModel(count, page, pageSize);
                h.Users = item;

                return View(h);
            }
            else
                return View(h);
            return View(h);
        }

        [Authorize(Roles = "Администратор")]
        [HttpGet]
        [ActionName("DeleteUsersModal")]
        public async Task<IActionResult> ConfirmDelete(string id)
        {
            User p = await db.GetUsersID(id);
            if (p != null)
                return PartialView(p);
            return View();
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost]
        public async Task<IActionResult> DeleteUsersModal(string id)
        {
            await db.Remove(id);
            return RedirectToAction("UsersPage");
        }

        [Authorize(Roles = "Администратор")]
        [HttpGet]
        [ActionName("EditUsersModal")]
        public async Task<IActionResult> ConfirmEdit(string id)
        {
            User p = await db.GetUsersID(id);
            if (p != null)
                return PartialView(p);
            return View();
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost]
        public async Task<IActionResult> EditUsersModal(User p)
        {
                await db.Update(p);
                return RedirectToAction("UsersPage");
        }

        [Authorize(Roles = "Администратор")]
        [HttpGet]
        [ActionName("AddUsersModal")]
        public async Task<IActionResult> ConfirmCreate()
        {
            return PartialView();
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost]
        public async Task<IActionResult> AddUsersModal(User p)
        {
            if (ModelState.IsValid)
            {
                await db.Create(p);
                return RedirectToAction("UsersPage");
            }
            return View(p);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> CheckEmail(User p)
        {
            bool exists = db.CheckExistEmail(p.Email);
            if (exists && OldEmail.OldE != p.Email)
                return Json(false);
            return Json(true);
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult CheckRole(User p)
        {
            if (p.Role != "Администратор" && p.Role != "Покупатель" && p.Role != "Оператор")
                return Json(false);
            return Json(true);
        }

        [Authorize(Roles = "Администратор")]
        [HttpGet]
        [ActionName("ChangePasswordModal")]
        public async Task<IActionResult> ConfirmChangePassword(string id)
        {
            User p = await db.GetUsersID(id);
            if (p != null)
                return PartialView(p);
            return View();
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost]
        public async Task<IActionResult> ChangePasswordModal(User p)
        {
            await db.UpdatePassword(p);
            return RedirectToAction("UsersPage");
        }

        [HttpGet]
        [ActionName("AuthenticateUser")]
        public async Task<IActionResult> Authen()
        {
            ModelState.Clear();
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AuthenticateUser(AuthenticationUser model)
        {
            //if (ModelState.IsValid)
            //{
                bool t = db.CheckUserAuth(model.Email, model.Password);
                if (t != false)
                {

                await Authenticate(model.Email); // аутентификация
                    Debug.Write("Authenticate TRUE");
                    ModelState.Clear();
                    //return Content($"HiWorld You are authenticated in System");
                    if (User.IsInRole("Администратор"))
                        return RedirectToAction("UsersPage", "Home");
                    else
                        return RedirectToAction("Index", "Home");
            }
                else
                    ModelState.AddModelError("", "");
            //}
            return PartialView("AuthenticateUser", model);
        }

        private async Task Authenticate(string userName)
        {
            User p = await db.GetUserPoEmail(userName);
            AuthenticatedUser.Familia = p.Familia;
            AuthenticatedUser.Ima = p.Ima;

            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultRoleClaimType, p.Role),
                new Claim(ClaimsIdentity.DefaultNameClaimType, p.Email+" "+p.Familia+" "+p.Ima)
            };
            //Debug.WriteLine(p.Id);
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //return RedirectToAction("CheckAuthentication", "Home");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [ActionName("RegistrateUser")]
        public async Task<IActionResult> Reg()
        {
            ModelState.Clear();
            return PartialView();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrateUser(RegistrateUsers model)
        {
            Debug.WriteLine("TTT: ");
            bool exists = db.CheckExistEmail(model.Email);
            if (exists)
            {
                Debug.WriteLine("TTT: " + exists.ToString());
                ModelState.AddModelError("", "");
                //return PartialView("RegistrateUser", model);
            }
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                model.Role = "Покупатель";
                User p = new User();
                p.Email = model.Email;
                p.Ima = model.Ima;
                p.Familia = model.Familia;
                p.Password = model.Password;
                p.Role = "Покупатель";
                p.ConfirmedEmail = true;
                string t = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                t = t.Replace("/", "");
                t = t.Replace(@"\", "");
                t = t.Replace(@"=", "");
                p.Token = t;
                Debug.WriteLine("TT: " + p.Token);
                await db.Create(p);
                await Authenticate(model.Email);
            }
            else
                ModelState.AddModelError("", "");

            return View();//("RegistrateUser", model);
        }

        [HttpGet]
        //[ActionName("ObratniZvonok")]
        public async Task<IActionResult> ObratniZvonok()
        {
            ModelState.Clear();
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ObratniZvonok(ObratZvonok model)
        {
            bool exists = db.CheckExistZaavkaObrZvonok(model.Telefon);
            //bool exists = false;
            if (exists)
            {
                Debug.WriteLine("UserIsExists");
                ModelState.AddModelError("", "");
                return PartialView("ObratniZvonok", model);
            }
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                Debug.WriteLine("UserMustBeCreated");
                await db.CreateZaavka(model);
                return View();
            }
            else
                ModelState.AddModelError("", "");

            return PartialView("ObratniZvonok", model);//("RegistrateUser", model);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> CheckTel(ObratZvonok p)
        {
            bool exists = db.checkExistTel(p.Telefon);
            if(exists)
                return Json(false);
            return Json(true);
        }

        [Authorize(Roles = "Администратор,Оператор")]
        public async Task<IActionResult> ObratZvOperator()
        {
            return View(await db.GetZaavki());
        }

        [Authorize(Roles = "Администратор,Оператор")]
        [HttpGet]
        [ActionName("ObrabotkaModal")]
        public async Task<IActionResult> ConfirmObr(string id)
        {
            ObratZvonok p = await db.GetZvPoID(id);
            if (p != null)
                return PartialView(p);
            return View();
        }

        [Authorize(Roles = "Администратор,Оператор")]
        [HttpPost]
        public async Task<IActionResult> ObrabotkaModal(string id)
        {
            await db.RemoveZv(id);
            return RedirectToAction("ObratZvOperator");
        }

        [Authorize]
        [ActionName("AddToCart")]
        public async Task<IActionResult> AddToCart(string id_tovara)
        {
            var email = HttpContext.User.Identity.Name;
            string[] slova = email.Split(' ');
            Debug.WriteLine(slova[0]);
            User t = await db.GetUserPoEmail(slova[0]);
            

            await dbs.AddToCart(id_tovara, t.Id);
            return RedirectToAction("Korzina","Categorii");
        }


        [Authorize]
        [ActionName("AddItemInExistCart")]
        public async Task<IActionResult> AddItemInExistCart(string id_tovara, int new_kolvo)
        {
            var email = HttpContext.User.Identity.Name;
            string[] slova = email.Split(' ');
            Debug.WriteLine(slova[0]);
            User t = await db.GetUserPoEmail(slova[0]);


            await dbs.AddToCartNewKolvo(id_tovara, new_kolvo, t.Id);
            return RedirectToAction("Korzina", "Categorii");
        }

        [Authorize]
        public IActionResult PerexodInCart()
        {
            return Redirect("~/Categorii/Korzina");
        }

        [Authorize]
        public async Task<IActionResult> DeleteFromCart(string id_tovara)
        {
            var email = HttpContext.User.Identity.Name;
            string[] slova = email.Split(' ');
            Debug.WriteLine(slova[0]);
            User t = await db.GetUserPoEmail(slova[0]);

            await dbs.DeleteFromCart(id_tovara, t.Id);
            return RedirectToAction("Korzina", "Categorii");
        }
    }
}
