using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using MongoDB.Driver.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson.Serialization.Attributes;

namespace ShopMagazin.Models
{
    public class TovariService
    {
        IGridFSBucket gridFS;   // файловое хранилище
        IMongoCollection<Categorii> Categories;// коллекция в базе данных
        IMongoCollection<PodCategorii> PodCategories;
        IMongoCollection<Tovar> TovariList;
        IMongoCollection<Korzina> Korzina;
        IMongoCollection<Zakaz> Zakazi;
        public TovariService()
        {
            // строка подключения
            string connectionString = "mongodb://localhost:27017/ShopVasko";
            var connection = new MongoUrlBuilder(connectionString);
            // получаем клиента для взаимодействия с базой данных
            MongoClient client = new MongoClient(connectionString);
            // получаем доступ к самой базе данных
            IMongoDatabase database = client.GetDatabase(connection.DatabaseName);
            // получаем доступ к файловому хранилищу
            gridFS = new GridFSBucket(database);
            Categories = database.GetCollection<Categorii>("Categories");
            PodCategories = database.GetCollection<PodCategorii>("PodCategories");
            TovariList = database.GetCollection<Tovar>("Tovari");
            Korzina = database.GetCollection<Korzina>("Korzina");
            Zakazi = database.GetCollection<Zakaz>("Zakazi");
        }

        public async Task AddToCart(string id_tovara, string user_id)
        {
            
            Korzina p = new Korzina();
            p.id_tovara = id_tovara;
            p.id_user = user_id;
            if (checkexisttovarincart(user_id, id_tovara))
            {
                Korzina z = await GetOneItemFromKorzina(user_id, id_tovara);
                z.kolvo_tovara += 1;
                await Korzina.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(z.Id)), z);
            }
            else
            {
                p.kolvo_tovara = 1;
                await Korzina.InsertOneAsync(p);
            }
        }

        public async Task AddToCartNewKolvo(string id_tovara, int new_kolvo, string user_id)
        {

            Korzina p = new Korzina();
            p.id_tovara = id_tovara;
            p.id_user = user_id;
            if (checkexisttovarincart(user_id, id_tovara))
            {
                Korzina z = await GetOneItemFromKorzina(user_id, id_tovara);
                z.kolvo_tovara = new_kolvo;
                await Korzina.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(z.Id)), z);
            }
            else
            {
                p.kolvo_tovara = 1;
                await Korzina.InsertOneAsync(p);
            }
        }

        public async Task<Korzina> GetOneItemFromKorzina(string user_id, string id_tovara)
        {
            return await Korzina.Find(_ => _.id_tovara == id_tovara && _.id_user == user_id).SingleAsync();
        }


        public bool checkexisttovarincart(string id_user, string id_tovar)
        {
            bool res = false;
            res = Korzina.Find(_ => _.id_tovara == id_tovar && _.id_user == id_user).Any();
            return res;
        }

        // получаем все документы, используя критерии фальтрации
        public async Task<IEnumerable<Categorii>> GetCategories()
        {
            // строитель фильтров
            var builder = new FilterDefinitionBuilder<Categorii>();
            var filter = builder.Empty; // фильтр для выборки всех документов

            return await Categories.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Categorii>> GetCategsWithFilter(string fil)
        {
            // строитель фильтров
            var builder = new FilterDefinitionBuilder<Categorii>();
            var filter = builder.Empty;
            if (fil != null && fil.Trim() != "")
            {
                filter = Builders<Categorii>.Filter.Where(g => g.NameCategori.ToLower().Contains(fil));
            }

            return await Categories.Find(filter).ToListAsync();
        }

        public async Task<Categorii> GetIDCategoriiWithName(string fil)
        {
            return await Categories.Find(_ => _.NameCategori == fil).SingleAsync();
        }

        // получаем один документ по id
        public async Task<Categorii> GetCategoriesPoID(string id)
        {
            return await Categories.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
        }
        // добавление документа
        public async Task CreateCategori(Categorii p, Stream imageStream, string imageName)
        {
            ObjectId imageId = await gridFS.UploadFromStreamAsync(imageName, imageStream);
            // обновляем данные по документу
            p.ImageId = imageId.ToString();
            await Categories.InsertOneAsync(p);
        }
        // обновление документа
        public async Task UpdateCategori(Categorii p, Stream imageStream, string imageName)
        {
            Categorii k = await GetCategoriesPoID(p.Id);
            if (imageName != "")
            {
                ObjectId imageId = await gridFS.UploadFromStreamAsync(imageName, imageStream);
                // обновляем данные по документу
                p.ImageId = imageId.ToString();
            }
            else
                p.ImageId = k.ImageId;

            UpdateDefinition<PodCategorii> updateDefinition = Builders<PodCategorii>.Update.Set(x => x.NameCategori, p.NameCategori);
            PodCategories.UpdateMany(x => x.NameCategori == k.NameCategori, updateDefinition);

            UpdateDefinition<Tovar> updateDefinition1 = Builders<Tovar>.Update.Set(x => x.NameCategori, p.NameCategori);
            TovariList.UpdateMany(x => x.NameCategori == k.NameCategori, updateDefinition1);

            await Categories.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(p.Id)), p);
        }
        // удаление документа
        public async Task RemoveCategori(string id)
        {
            Categorii p = await GetCategoriesPoID(id);
            await gridFS.DeleteAsync(new ObjectId(p.ImageId));

            foreach(var item in await GetPodCategories())
            {
                if(item.NameCategori  == p.NameCategori)
                    await gridFS.DeleteAsync(new ObjectId(item.ImageId));
            }

            IEnumerable<Tovar> h = await GetTovariWithFilterAndCategori(null, p.NameCategori, null);
            IEnumerable<Korzina> j = await AllGetTovariFromKorzina();
            foreach (var item in h)
            {
                await Korzina.DeleteManyAsync(t => t.id_tovara == item.Id);
            }

            await PodCategories.DeleteManyAsync(t => t.NameCategori == p.NameCategori);
            await TovariList.DeleteManyAsync(t => t.NameCategori == p.NameCategori);

            await Categories.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }
        // получение изображения
        public async Task<byte[]> GetImage(string id)
        {
            return await gridFS.DownloadAsBytesAsync(new ObjectId(id));
        }


        // сохранение изображения
        public async Task StoreImage(string id, Stream imageStream, string imageName)
        {
            Categorii p = await GetCategoriesPoID(id);
            if (p.HasImage())
            {
                // если ранее уже была прикреплена картинка, удаляем ее
                await gridFS.DeleteAsync(new ObjectId(p.ImageId));
            }
            // сохраняем изображение
            ObjectId imageId = await gridFS.UploadFromStreamAsync(imageName, imageStream);
            // обновляем данные по документу
            p.ImageId = imageId.ToString();
            var filter = Builders<Categorii>.Filter.Eq("_id", new ObjectId(p.Id));
            var update = Builders<Categorii>.Update.Set("ImageId", p.ImageId);
            await Categories.UpdateOneAsync(filter, update);
        }
        public bool CheckCategoris(string emails)
        {
            bool exists = Categories.Find(_ => _.NameCategori == emails).Any();
            return exists;
        }



        //ПОДКАТЕГОРИИ
        public bool CheckPodCategoris(string emails)
        {
            bool exists = PodCategories.Find(_ => _.NamePodCategori == emails).Any();
            Debug.WriteLine("Рузльтат нахождения: " + exists.ToString());
            return exists;
        }

        public async Task<PodCategorii> GetIDPodCategoriiWithName(string fil)
        {
            return await PodCategories.Find(_ => _.NamePodCategori == fil).SingleAsync();
        }

        public async Task<IEnumerable<PodCategorii>> GetPodCategories()
        {
            // строитель фильтров
            var builder = new FilterDefinitionBuilder<PodCategorii>();
            var filter = builder.Empty; // фильтр для выборки всех документов

            return await PodCategories.Find(filter).ToListAsync();
        }

        public async Task CreatePodCat(PodCatG p, Stream imageStream, string imageName)
        {
            Debug.WriteLine(imageName);
            ObjectId imageId = await gridFS.UploadFromStreamAsync(imageName, imageStream);
            // обновляем данные по документу
            p.podcategs.ImageId = imageId.ToString();
            await PodCategories.InsertOneAsync(p.podcategs);
        }

        public async Task<PodCategorii> GetPodCategoriesPoID(string id)
        {
            return await PodCategories.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
        }

        public async Task<List<PodCategorii>> GetPodCategoriesPoMainCat(string id)
        {
            Categorii p = await GetCategoriesPoID(id);
            var filter = new BsonDocument("NameCategori", p.NameCategori);
            return await PodCategories.Find(filter).ToListAsync();
        }

        public async Task RemovePodCategori(string id)
        {
            PodCategorii p = await GetPodCategoriesPoID(id);
            await gridFS.DeleteAsync(new ObjectId(p.ImageId));

            IEnumerable<Tovar> h = await GetTovariWithFilterAndCategori(null, p.NameCategori, p.NamePodCategori);
            IEnumerable<Korzina> j = await AllGetTovariFromKorzina();
            foreach(var item in h)
            {
                await Korzina.DeleteManyAsync(t => t.id_tovara == item.Id);
            }

            await TovariList.DeleteManyAsync(t => t.NamePodCategori == p.NamePodCategori);
            await PodCategories.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }

        public async Task UpdatePodCategori(PodCatG p, Stream imageStream, string imageName)
        {
            PodCategorii k = await GetPodCategoriesPoID(p.podcategs.Id);
            if (imageName != "")
            {
                ObjectId imageId = await gridFS.UploadFromStreamAsync(imageName, imageStream);
                // обновляем данные по документу
                p.podcategs.ImageId = imageId.ToString();
            }
            else
                p.podcategs.ImageId = k.ImageId;

            UpdateDefinition<Tovar> updateDefinition1 = Builders<Tovar>.Update.Set(x => x.NamePodCategori, p.podcategs.NamePodCategori);
            TovariList.UpdateMany(x => x.NamePodCategori == k.NamePodCategori, updateDefinition1);

            await PodCategories.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(p.podcategs.Id)), p.podcategs);
        }

        public async Task<IEnumerable<PodCategorii>> GetPodCategsWithFilter(string fil, string role)
        {
            // строитель фильтров
            var builder = new FilterDefinitionBuilder<PodCategorii>();
            var filter = builder.Empty;
            if (fil != null)
            {
                if (fil.Trim() != null)
                {
                    Debug.WriteLine(fil);
                    if (role == null)
                        filter = Builders<PodCategorii>.Filter.Where(g => g.NamePodCategori.ToLower().Contains(fil));
                    else
                        filter = Builders<PodCategorii>.Filter.Where(g => g.NamePodCategori.ToLower().Contains(fil) && g.NameCategori == role);
                }
            }
            else if (role != null)
            {
                Debug.WriteLine("Только поиск по роли");
                filter = Builders<PodCategorii>.Filter.Where(g => g.NameCategori == role);
            }

            return await PodCategories.Find(filter).ToListAsync();
        }


        //ТОВАРЫ:
        // получаем все документы, используя критерии фальтрации
        public async Task<IEnumerable<Tovar>> GetTovari()
        {
            // строитель фильтров
            var builder = new FilterDefinitionBuilder<Tovar>();
            var filter = builder.Empty; // фильтр для выборки всех документов

            return await TovariList.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Tovar>> GetTovariWithFilterAndCategori(string fil, string categ, string podcateg)
        {
            // строитель фильтров
            Debug.WriteLine(categ + " " + podcateg);
            var builder = new FilterDefinitionBuilder<Tovar>();
            var filter = builder.Empty;
            if (categ == null && podcateg == null)
            {
                filter = Builders<Tovar>.Filter.Where(g => g.NameTovar.ToLower().Contains(fil) || g.StranaProizvoditel.ToLower().Contains(fil) || g.ModelTovar.ToLower().Contains(fil) || g.Cena.ToLower().Contains(fil) || g.articul.ToLower().Contains(fil));
            }
            else if (fil != null && fil.Trim() != "")
            {
                filter = Builders<Tovar>.Filter.Where(g => (g.NameTovar.ToLower().Contains(fil) || g.StranaProizvoditel.ToLower().Contains(fil) || g.ModelTovar.ToLower().Contains(fil) || g.Cena.ToLower().Contains(fil) || g.articul.ToLower().Contains(fil)) && (g.NameCategori == categ && g.NamePodCategori == podcateg));
            }
            else if(fil == null && podcateg != null)
                filter = Builders<Tovar>.Filter.Where(g => g.NameCategori == categ && g.NamePodCategori == podcateg);
            else if(fil == null && categ != null && podcateg == null)
                filter = Builders<Tovar>.Filter.Where(g => g.NameCategori == categ);
            else if(fil == null && categ != null && podcateg != null)
                filter = Builders<Tovar>.Filter.Where(g => g.NameCategori == categ && g.NamePodCategori == podcateg);

            return await TovariList.Find(filter).ToListAsync();
        }

        // получаем один документ по id
        public async Task<Tovar> GetTovarPoID(string id)
        {
            return await TovariList.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
        }

        public bool CheckArticul(string emails)
        {
            bool exists = TovariList.Find(_ => _.articul == emails).Any();
            return exists;
        }

        public async Task CreateTovar(TovariManage p, Stream imageStream, string imageName)
        {
            ObjectId imageId = await gridFS.UploadFromStreamAsync(imageName, imageStream);
            // обновляем данные по документу
            p.tovar.ImageId = imageId.ToString();
            await TovariList.InsertOneAsync(p.tovar);
        }

        public async Task RemoveTovar(string id)
        {
            Tovar p = await GetTovarPoID(id);
            await gridFS.DeleteAsync(new ObjectId(p.ImageId));
            await Korzina.DeleteManyAsync(t => t.id_tovara == p.Id);
            await TovariList.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }

        public async Task UpdateRovar(TovariManage p, Stream imageStream, string imageName)
        {
            Tovar k = await GetTovarPoID(p.tovar.Id);
            if (imageName != "")
            {
                ObjectId imageId = await gridFS.UploadFromStreamAsync(imageName, imageStream);
                // обновляем данные по документу
                p.tovar.ImageId = imageId.ToString();
            }
            else
                p.tovar.ImageId = k.ImageId;

            await TovariList.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(p.tovar.Id)), p.tovar);
        }

        public async Task<List<Korzina>> GetTovariFromKorzina(string id_user)
        {
            var builder = new FilterDefinitionBuilder<Korzina>();
            var filter = builder.Empty; // фильтр для выборки всех документов
            filter = Builders<Korzina>.Filter.Where(g => g.id_user == id_user);
            return await Korzina.Find(filter).ToListAsync();
        }

        public async Task<List<Tovar>> GetSpecialTovari()
        {
            return await TovariList.AsQueryable().Sample(10).ToListAsync();
        }

        public async Task<List<Zakaz>> GetZakaziUsera(string id_user)
        {
            var builder = new FilterDefinitionBuilder<Zakaz>();
            var filter = builder.Empty; // фильтр для выборки всех документов
            filter = Builders<Zakaz>.Filter.Where(g => g.Id_User == id_user);
            return await Zakazi.Find(filter).ToListAsync();
        }


        public async Task<Zakaz> GetZakazPoID(string id_user)
        {
            var builder = new FilterDefinitionBuilder<Zakaz>();
            var filter = builder.Empty; // фильтр для выборки всех документов
            filter = Builders<Zakaz>.Filter.Where(g => g.Id == id_user);
            return await Zakazi.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<Korzina>> AllGetTovariFromKorzina()
        {
            var builder = new FilterDefinitionBuilder<Korzina>();
            var filter = builder.Empty; // фильтр для выборки всех документов
            return await Korzina.Find(filter).ToListAsync();
        }

        public async Task<List<Tovar>> GetTovariToSearch(string ser)
        {
            var builder = new FilterDefinitionBuilder<Tovar>();
            var filter = builder.Empty; // фильтр для выборки всех документов
            filter = Builders<Tovar>.Filter.Where(g => g.NameTovar.ToLower().Contains(ser) || g.articul.ToLower().Contains(ser) || g.ModelTovar.ToLower().Contains(ser));
            return await TovariList.Find(filter).ToListAsync();
        }

        public async Task DeleteFromCart(string id_tovara, string id_user)
        {
            await Korzina.DeleteOneAsync(t=>t.id_tovara==id_tovara && t.id_user == id_user);
        }

        public async Task CreateZakaz(string ID_User, TovariInCart g)
        {
            Debug.WriteLine(g.zakaz.Address_Ulica);
            var kor = await GetTovariFromKorzina(ID_User);
            g.zakaz.Id_User = ID_User;
            g.zakaz.status = "Принят. Обработка";
            g.zakaz.StoimostZakakaAll = g.zakaz.StoimostZakakaTovari;
            g.zakaz.CenaDostavki = "0";
            g.zakaz.tovars = new List<TovarsInZakaz>();
            List<Korzina> ListKorzinas = await GetTovariFromKorzina(ID_User);
            foreach (var item in kor)
            {
                Tovar j = new Tovar();
                j = await GetTovarPoID(item.id_tovara);
                TovarsInZakaz n = new TovarsInZakaz();
                n.Cena = j.Cena;
                n.Id_tovara = j.Id;
                foreach (var item2 in ListKorzinas)
                {
                    if (item2.id_tovara == j.Id)
                    {
                        n.kolvo_tovara = item2.kolvo_tovara;
                        break;
                    }
                }
                n.ModelTovar = j.ModelTovar;
                n.NameTovar = j.NameTovar;           
                g.zakaz.tovars.Add(n);
            }
            
            await Zakazi.InsertOneAsync(g.zakaz);
        }
    }
}
