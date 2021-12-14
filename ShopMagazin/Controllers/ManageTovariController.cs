using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopMagazin.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ShopMagazin.Controllers
{
    public static class TecSsilka
    {
        public static string Ssilka { get; set; }
    }

    public class ManageTovariController : Controller
    {
        private readonly TovariService db;
        public ManageTovariController(TovariService context)
        {
            db = context;
        }

        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> SelectCatPodCat()
        {
            PodCatG g = new PodCatG();
            g.PodCategsList = await db.GetPodCategories();
            g.PodU = await db.GetCategories();
            return View(g);
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
        public async Task<IActionResult> PageManageTovari(string id, string ZhachPoiska, int page = 1)
        {
            TovariManage j = new TovariManage();
            j.PodCatG = new PodCatG();

            j.PodCatG.podcategs = await db.GetPodCategoriesPoID(id);
            j.PodCatG.ZhachPoiska = ZhachPoiska;

            j.TovariList = await db.GetTovari();

            if (ZhachPoiska != null)
            {
                j.TovariList = await db.GetTovariWithFilterAndCategori(ZhachPoiska, j.PodCatG.podcategs.NameCategori, j.PodCatG.podcategs.NamePodCategori);
            }
            else
            {
                j.TovariList = await db.GetTovariWithFilterAndCategori(null, j.PodCatG.podcategs.NameCategori, j.PodCatG.podcategs.NamePodCategori);
            }

            int pageSize = 10;
            var count = j.TovariList.Count();
            var item = j.TovariList.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            j.PodCatG.PageViewModel = new PageViewModel(count, page, pageSize);
            j.TovariList = item;
            TecSsilka.Ssilka = j.PodCatG.podcategs.Id;

            return View(j);
        }

        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> PageTovariAll(string ZhachPoiska, int page = 1)
        {
            TovariManage j = new TovariManage();
            j.PodCatG = new PodCatG();

            //j.PodCatG.podcategs = await db.GetPodCategoriesPoID(id);
            j.PodCatG.ZhachPoiska = ZhachPoiska;

            j.TovariList = await db.GetTovari();

            if (ZhachPoiska != null)
            {
                j.TovariList = await db.GetTovariWithFilterAndCategori(ZhachPoiska, null, null);
            }

            int pageSize = 15;
            var count = j.TovariList.Count();
            var item = j.TovariList.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            j.PodCatG.PageViewModel = new PageViewModel(count, page, pageSize);
            j.TovariList = item;
            TecSsilka.Ssilka = "All";

            return View(j);
        }

        [Authorize(Roles = "Администратор")]
        [HttpGet]
        [ActionName("CreateTovar")]
        public async Task<IActionResult> ConfirmCreateTovar(string id)
        {
            TovariManage g = new TovariManage();
            g.tovar = new Tovar();
            PodCategorii h = new PodCategorii();
            h = await db.GetPodCategoriesPoID(id);
            g.tovar.NameCategori = h.NameCategori;
            g.tovar.NamePodCategori = h.NamePodCategori;
            return PartialView(g);
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost]
        public async Task<IActionResult> CreateTovar(TovariManage p, IFormFile uploadedFile)
        {
            Debug.WriteLine(ModelState.IsValid.ToString());
            if (ModelState.IsValid)
            {
                p.tovar.articul = p.tovar.articul.Trim();
                p.tovar.Cena = p.tovar.Cena.Trim();
                p.tovar.ModelTovar = p.tovar.ModelTovar.Trim();
                p.tovar.NameTovar = p.tovar.NameTovar.Trim();
                p.tovar.StranaProizvoditel = p.tovar.StranaProizvoditel.Trim();
                await db.CreateTovar(p, uploadedFile.OpenReadStream(), uploadedFile.FileName);
                return RedirectToAction("PageManageTovari", "ManageTovari", new { id = TecSsilka.Ssilka });
            }
            return View(p);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> CheckArticul(TovariManage p)
        {
            bool exists = db.CheckArticul(p.tovar.articul);
            if (exists && OldEmail.OldE != p.tovar.articul)
                return Json(false);
            return Json(true);
        }

        [Authorize(Roles = "Администратор")]
        [HttpGet]
        [ActionName("DeleteTovar")]
        public async Task<IActionResult> ConfirmDeleteTovar(string id)
        {
            Tovar p = await db.GetTovarPoID(id);
            TovariManage g = new TovariManage();
            g.tovar = p;
            return PartialView(g);
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost]
        public async Task<IActionResult> DeleteTovar(string id)
        {
            await db.RemoveTovar(id);
            if (TecSsilka.Ssilka == "All")
                return RedirectToAction("PageTovariAll", "ManageTovari");
            else
                return RedirectToAction("PageManageTovari", "ManageTovari", new { id = TecSsilka.Ssilka });
        }

        [Authorize(Roles = "Администратор")]
        [HttpGet]
        [ActionName("EditTovar")]
        public async Task<IActionResult> ConfirmEdit(string id)
        {
            Tovar p = await db.GetTovarPoID(id);
            TovariManage g = new TovariManage();
            g.tovar = p;
            Debug.WriteLine(g.tovar.Id);
            OldEmail.OldE = p.articul;
            if (p != null)
                return PartialView(g);
            return View();
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost]
        public async Task<IActionResult> EditTovar(TovariManage p, IFormFile uploadedFile)
        {
            Debug.WriteLine(p.tovar.Id + "  " + p.tovar.NameTovar);
            if (uploadedFile == null)
                await db.UpdateRovar(p, null, "");
            else
                await db.UpdateRovar(p, uploadedFile.OpenReadStream(), uploadedFile.FileName);
            if(TecSsilka.Ssilka == "All")
                return RedirectToAction("PageTovariAll", "ManageTovari");
            else
                return RedirectToAction("PageManageTovari", "ManageTovari", new { id = TecSsilka.Ssilka });
        }
    }
}
