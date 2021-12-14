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
using ShopMagazin.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using PagedList.Mvc;
using PagedList;

namespace ShopMagazin.Controllers
{
    public class TovariController : Controller
    {
        private readonly TovariService db;
        public TovariController(TovariService context)
        {
            db = context;
        }

        /*Создание категорий*/

        [Authorize(Roles = "Администратор")]
        [HttpGet]
        [ActionName("CreateCat")]
        public async Task<IActionResult> ConfirmCreateCat()
        {
            return PartialView();
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost]
        public async Task<IActionResult> CreateCat(Categorii p, IFormFile uploadedFile)
        {
            Debug.WriteLine(ModelState.IsValid.ToString());
            if (ModelState.IsValid)
            {
                await db.CreateCategori(p, uploadedFile.OpenReadStream(), uploadedFile.FileName);
                return RedirectToAction("IndexCategorii");
            }
            return View(p);
        }

        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> IndexCategorii(string ZhachPoiska, int page = 1)
        {
            PodCatG g = new PodCatG();
            g.PodU = await db.GetCategories();
            g.ZhachPoiska = ZhachPoiska;
            if(ZhachPoiska != null)
            {
                g.PodU = await db.GetCategsWithFilter(ZhachPoiska);
            }

            int pageSize = 5;
            var count = g.PodU.Count();
            var item = g.PodU.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            g.PageViewModel = new PageViewModel(count, page, pageSize);
            g.PodU = item;

            return View(g);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> CheckCategoris(Categorii p)
        {
            Debug.WriteLine(OldEmail.OldE + "  " + p.NameCategori);
            bool exists = db.CheckCategoris(p.NameCategori);
            if (exists && OldEmail.OldE != p.NameCategori)
                return Json(false);
            return Json(true);
        }

        public async Task<ActionResult> GetImage(string id)
        {
            var image = await db.GetImage(id);
            if (image == null)
            {
                return NotFound();
            }
            return File(image, "image/png");
        }

        [Authorize(Roles = "Администратор")]
        [HttpGet]
        [ActionName("EditCat")]
        public async Task<IActionResult> ConfirmEdit(string id)
        {
            Categorii p = await db.GetCategoriesPoID(id);
            Debug.WriteLine("Стар: " + p.NameCategori);
            OldEmail.OldE = p.NameCategori;
            if (p != null)
                return PartialView(p);
            return View();
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost]
        public async Task<IActionResult> EditCat(Categorii p, IFormFile uploadedFile)
        {
            if (uploadedFile == null)
                await db.UpdateCategori(p, null, "");
            else
                await db.UpdateCategori(p, uploadedFile.OpenReadStream(), uploadedFile.FileName);
            return RedirectToAction("IndexCategorii");
        }

        [Authorize(Roles = "Администратор")]
        [HttpGet]
        [ActionName("DeleteCat")]
        public async Task<IActionResult> ConfirmDeleteCat(string id)
        {
            Categorii p = await db.GetCategoriesPoID(id);
            return PartialView(p);
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost]
        public async Task<IActionResult> DeleteCat(string id)
        {
            await db.RemoveCategori(id);
            return RedirectToAction("IndexCategorii");
        }


        //ПОДКАТЕГОРИИ
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> IndexPodCategorii(string ZhachPoiska, string ZnachPoiskaCateg, int page = 1)
        {
            Debug.WriteLine("sfdfafafffas^: " + ZhachPoiska);
            Debug.WriteLine("1212sfdfafafffas^: " + ZnachPoiskaCateg);
            PodCatG g = new PodCatG();
            g.PodCategsList = await db.GetPodCategories();
            g.PodU = await db.GetCategories();

            int pageSize = 5;
            var count = g.PodCategsList.Count();
            var item = g.PodCategsList.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            g.PageViewModel = new PageViewModel(count, page, pageSize);
            g.PodCategsList = item;

            PodCatG t = new PodCatG();
            t.ZhachPoiska = ZhachPoiska;
            t.ZnachPoiskaCateg = ZnachPoiskaCateg;

            if (t != null)
            {
                if (t.ZnachPoiskaCateg == "Все")
                    t.ZnachPoiskaCateg = null;

                g.PodCategsList = await db.GetPodCategsWithFilter(t.ZhachPoiska, t.ZnachPoiskaCateg);

                count = g.PodCategsList.Count();
                item = g.PodCategsList.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                g.PageViewModel = new PageViewModel(count, page, pageSize);
                g.PodCategsList = item;

                if (t.ZhachPoiska != null)
                    g.ZhachPoiska = t.ZhachPoiska;
                if (t.ZnachPoiskaCateg != null)
                    g.ZnachPoiskaCateg = t.ZnachPoiskaCateg;
            }

            if (t.ZhachPoiska != null)
            {
                if (t.ZhachPoiska.Trim() != "")
                {
                    if (t.ZnachPoiskaCateg == "Все")
                    {

                        g.PodCategsList = await db.GetPodCategsWithFilter(t.ZhachPoiska, null);

                        count = g.PodCategsList.Count();
                        item = g.PodCategsList.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                        g.PageViewModel = new PageViewModel(count, page, pageSize);
                        g.PodCategsList = item;

                        return View(g);
                    }
                    else
                    {
                        return View(g);
                    }
                }
            }
            else if (t.ZnachPoiskaCateg != "Все")
            {
                g.PodCategsList = await db.GetPodCategsWithFilter(t.ZhachPoiska, t.ZnachPoiskaCateg);

                count = g.PodCategsList.Count();
                item = g.PodCategsList.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                g.PageViewModel = new PageViewModel(count, page, pageSize);
                g.PodCategsList = item;

                return View(g);
            }
            else
                return View(g);
            return View(g);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> CheckPodCategoris(PodCatG p)
        {
            //Debug.WriteLine(OldEmail.OldE + "  " + p.NameCategori);
            bool exists = db.CheckPodCategoris(p.podcategs.NamePodCategori);
            if (exists && OldEmail.OldE != p.podcategs.NamePodCategori)
                return Json(false);
            return Json(true);
        }

        [Authorize(Roles = "Администратор")]
        [HttpGet]
        [ActionName("CreatePodCat")]
        public async Task<IActionResult> ConfirmCreatePodCat()
        {
            PodCatG t = new PodCatG();
            t.PodU = await db.GetCategories();
            return PartialView(t);
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost]
        public async Task<IActionResult> CreatePodCat(PodCatG p, IFormFile uploadedFile)
        {
            if (ModelState.IsValid)
            {
                await db.CreatePodCat(p, uploadedFile.OpenReadStream(), uploadedFile.FileName);
                return RedirectToAction("IndexPodCategorii");
            }
            return View(p);
        }

        [Authorize(Roles = "Администратор")]
        [HttpGet]
        [ActionName("DeletePodCat")]
        public async Task<IActionResult> ConfirmDeletePodCat(string id)
        {
            PodCatG g = new PodCatG();
            g.podcategs = await db.GetPodCategoriesPoID(id);
            return PartialView(g);
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost]
        public async Task<IActionResult> DeletePodCat(string id)
        {
            await db.RemovePodCategori(id);
            return RedirectToAction("IndexPodCategorii");
        }

        [Authorize(Roles = "Администратор")]
        [HttpGet]
        [ActionName("EditPodCat")]
        public async Task<IActionResult> ConfirmPodEdit(string id)
        {
            PodCatG g = new PodCatG();
            g.podcategs = await db.GetPodCategoriesPoID(id);
            g.PodU = await db.GetCategories();
            OldEmail.OldE = g.podcategs.NamePodCategori;
            if (g != null)
                return PartialView(g);
            return View();
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost]
        public async Task<IActionResult> EditPodCat(PodCatG p, IFormFile uploadedFile)
        {
            if (uploadedFile == null)
                await db.UpdatePodCategori(p, null, "");
            else
                await db.UpdatePodCategori(p, uploadedFile.OpenReadStream(), uploadedFile.FileName);
            return RedirectToAction("IndexPodCategorii");
        }
    }
}
