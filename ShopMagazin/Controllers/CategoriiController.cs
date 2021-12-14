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

namespace ShopMagazin.Controllers
{
    public class CategoriiController : Controller
    {
        private readonly TovariService db;
        private readonly UsersService dbs;
        public CategoriiController(TovariService context, UsersService contexts)
        {
            db = context;
            dbs = contexts;
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

        public async Task<IActionResult> Podcategorii(string id, int page = 1)
        {
            PodCatG g = new PodCatG();
            g.categMain = await db.GetCategoriesPoID(id);
            g.PodCategsList = await db.GetPodCategoriesPoMainCat(g.categMain.Id);
            g.TovariList = await db.GetTovariWithFilterAndCategori(null, g.categMain.NameCategori, null);
            g.KolTovarov = g.TovariList.Count().ToString();

            var count = g.TovariList.Count();
            int pageSize = 10;
            var item = g.TovariList.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            g.PageViewModel = new PageViewModel(count, page, pageSize);
            g.TovariList = item;

            if (HttpContext.User.Identity.Name != null)
            {
                var email = HttpContext.User.Identity.Name;
                string[] slova = email.Split(' ');
                Debug.WriteLine(slova[0]);
                User t = await dbs.GetUserPoEmail(slova[0]);
                Debug.WriteLine(t.Id);
                List<Korzina> ListKorzina = await db.GetTovariFromKorzina(t.Id.ToString());

                List<Tovar> itog = new List<Tovar>();
                foreach (var items in g.TovariList)
                {
                    foreach (var j in ListKorzina)
                    {
                        if (items.Id == j.id_tovara)
                        {
                            items.HasTovarInCart = true;
                            break;
                        }
                        else
                            items.HasTovarInCart = false;
                    }
                    itog.Add(items);
                }
                g.TovariList = itog;
            }

            return View(g);
        }


        public async Task<IActionResult> TovariPodCategorii(string id, int page = 1)
        {
            PodCatG g = new PodCatG();
            g.podcategs = await db.GetPodCategoriesPoID(id);
            g.categMain = await db.GetIDCategoriiWithName(g.podcategs.NameCategori);

            g.TovariList = await db.GetTovariWithFilterAndCategori(null, g.podcategs.NameCategori, g.podcategs.NamePodCategori);
            g.KolTovarov = g.TovariList.Count().ToString();

            var count = g.TovariList.Count();
            int pageSize = 10;
            var item = g.TovariList.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            g.PageViewModel = new PageViewModel(count, page, pageSize);
            g.TovariList = item;

            if (HttpContext.User.Identity.Name != null)
            {
                var email = HttpContext.User.Identity.Name;
                string[] slova = email.Split(' ');
                Debug.WriteLine(slova[0]);
                User t = await dbs.GetUserPoEmail(slova[0]);
                Debug.WriteLine(t.Id);
                List<Korzina> ListKorzina = await db.GetTovariFromKorzina(t.Id.ToString());

                List<Tovar> itog = new List<Tovar>();
                foreach (var items in g.TovariList)
                {
                    foreach (var j in ListKorzina)
                    {
                        if (items.Id == j.id_tovara)
                        {
                            items.HasTovarInCart = true;
                            break;
                        }
                        else
                            items.HasTovarInCart = false;
                    }
                    itog.Add(items);
                }
                g.TovariList = itog;
            }

            return View(g);
        }

        public async Task<IActionResult> Tovar(string id)
        {
            PodCatG g = new PodCatG();
            g.tovar = await db.GetTovarPoID(id);
            g.podcategs = await db.GetIDPodCategoriiWithName(g.tovar.NamePodCategori);
            g.categMain = await db.GetIDCategoriiWithName(g.tovar.NameCategori);

            if (HttpContext.User.Identity.Name != null)
            {
                var email = HttpContext.User.Identity.Name;
                string[] slova = email.Split(' ');
                Debug.WriteLine(slova[0]);
                User t = await dbs.GetUserPoEmail(slova[0]);
                Debug.WriteLine(t.Id);
                List<Korzina> ListKorzina = await db.GetTovariFromKorzina(t.Id.ToString());
                
                foreach(var item in ListKorzina)
                {
                    if(item.id_tovara == g.tovar.Id)
                    {
                        g.tovar.HasTovarInCart = true;
                        break;
                    }
                }
            }

            return View(g);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Korzina(/*string id, int page = 1*/)
        {
            TovariInCart g = new TovariInCart();

            if (HttpContext.User.Identity.Name != null)
            {
                var email = HttpContext.User.Identity.Name;
                string[] slova = email.Split(' ');
                Debug.WriteLine(slova[0]);
                User t = await dbs.GetUserPoEmail(slova[0]);
                Debug.WriteLine(t.Id);

                List<Korzina> ListKorzina = await db.GetTovariFromKorzina(t.Id.ToString());
                List<Tovar> toval = new List<Tovar>();

                foreach (var h in ListKorzina)
                {
                    toval.Add(await db.GetTovarPoID(h.id_tovara));
                }

                List<TovarForCart> itog = new List<TovarForCart>();

                foreach(var item in toval)
                {
                    TovarForCart k = new TovarForCart();
                    foreach(var items in ListKorzina)
                    {
                        if(item.Id == items.id_tovara)
                        {
                            k.Id = item.Id;
                            k.ImageId = item.ImageId;
                            k.Cena = item.Cena;
                            k.HasTovarInCart = true;
                            k.kolvo_tovara = items.kolvo_tovara;
                            k.ModelTovar = item.ModelTovar;
                            k.NameCategori = item.NameCategori;
                            k.NamePodCategori = item.NamePodCategori;
                            k.NameTovar = item.NameTovar;
                            k.StranaProizvoditel = item.StranaProizvoditel;
                            g.summazakaza = (Convert.ToDouble(g.summazakaza) + Convert.ToDouble(k.Cena) * k.kolvo_tovara).ToString();
                            itog.Add(k);
                        }
                    }
                }
                g.Korzina = itog;
            }
            return View(g);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Korzina(TovariInCart g)
        {
            Debug.WriteLine(g.zakaz.Address_Dom);
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                var email = HttpContext.User.Identity.Name;
                string[] slova = email.Split(' ');
                Debug.WriteLine(slova[0]);
                User t = await dbs.GetUserPoEmail(slova[0]);
                await db.CreateZakaz(t.Id,g);
                return RedirectToAction("Index","Home");
            }
            else
                ModelState.AddModelError("", "");

            return View("Korzina", g);//("RegistrateUser", model);
        }

        [Authorize]
        public async Task<IActionResult> MyZakazi()
        {
            var email = HttpContext.User.Identity.Name;
            string[] slova = email.Split(' ');
            Debug.WriteLine(slova[0]);
            User t = await dbs.GetUserPoEmail(slova[0]);
            ZakazUser z = new ZakazUser();
            z.zakazes = await db.GetZakaziUsera(t.Id);
            return View(z);
        }

        [Authorize]
        public async Task<IActionResult> ZakazUser(string id)
        {
            ZakazUser z = new ZakazUser();
            z.zakaz = new Zakaz();
            z.zakaz = await db.GetZakazPoID(id);
            Debug.WriteLine(id+"  :"+z.zakaz.Id + " " + z.zakaz.status);
            return View(z);
        }

        [HttpPost]
        public async Task<IActionResult> resultPoiskaAsync(string search)
        {
            PoiskTovarov g = new PoiskTovarov();
            if (search != null && search.Trim() != "")
            {
                search = search.Trim();
                g.search_string = search;
                g.TovariResult = await db.GetTovariToSearch(search);
            }
            else
            {
                g.TovariResult = null;
            }
            return View(g);
        }
    }
}
