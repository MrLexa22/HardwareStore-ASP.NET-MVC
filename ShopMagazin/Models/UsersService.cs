using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShopMagazin.Models
{
    public class UsersService
    {
        IGridFSBucket gridFS;   // файловое хранилище
        IMongoCollection<User> Users;
        IMongoCollection<ObratZvonok> ObratsZvonki;
        //IMongoCollection<Categorii> Categories;// коллекция в базе данных
        public UsersService()
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
            // обращаемся к коллекции Products
            Users = database.GetCollection<User>("Users");
            ObratsZvonki = database.GetCollection<ObratZvonok>("ObratniZconoks");
            //Categories = database.GetCollection<Categorii>("Categories");
        }

        // получаем все документы, используя критерии фальтрации
        public async Task<IEnumerable<User>> GetUsers()
        {
            // строитель фильтров
            var builder = new FilterDefinitionBuilder<User>();
            var filter = builder.Empty; // фильтр для выборки всех документов

            return await Users.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersWithFilter(string fil, string role)
        {
            Debug.WriteLine("БлаБлаБла: "+fil+"  "+role);
            // строитель фильтров
            var builder = new FilterDefinitionBuilder<User>();
            var filter = builder.Empty;
            if (fil != null)
            {
                if (fil.Trim() != null)
                {
                    Debug.WriteLine(fil);
                    if (role == null)
                        filter = Builders<User>.Filter.Where(g => g.Ima.ToLower().StartsWith(fil) || g.Familia.ToLower().StartsWith(fil) || g.Email.ToLower().StartsWith(fil));
                    else
                        filter = Builders<User>.Filter.Where(g => (g.Ima.ToLower().StartsWith(fil) || g.Familia.ToLower().StartsWith(fil) || g.Email.ToLower().StartsWith(fil)) && g.Role == role);
                }
            }
            else if (role != null)
            {
                Debug.WriteLine("Только поиск по роли");
                filter = Builders<User>.Filter.Where(g => g.Role == role);
            }

            return await Users.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<ObratZvonok>> GetZaavki()
        {
            // строитель фильтров
            var builder = new FilterDefinitionBuilder<ObratZvonok>();
            var filter = builder.Empty; // фильтр для выборки всех документов

            return await ObratsZvonki.Find(filter).ToListAsync();
        }

        // получаем один документ по id
        public async Task<User> GetUsersID(string id)
        {
            return await Users.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
        }

        public async Task<ObratZvonok> GetZvPoID(string id)
        {
            return await ObratsZvonki.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
        }

        public bool CheckExistEmail(string emails)
        {
            bool exists = Users.Find(_ => _.Email == emails).Any();
            return exists;
        }

        public bool checkExistTel(string tel)
        {
            bool exists = ObratsZvonki.Find(_ => _.Telefon == tel).Any();
            return exists;
        }

        public bool CheckUserAuth(string emails, string password)
        {
            bool exists = Users.Find(_ => _.Email == emails && _.Password == password && _.ConfirmedEmail == true).Any();
            //Debug.WriteLine("Говнище: "+emails + "  " + password + "  "+exists.ToString());
            return exists;
        }

        // добавление документа
        public async Task Create(User p)
        {
            await Users.InsertOneAsync(p);
        }

        public async Task<User> GetUserPoEmail(string email)
        {
            var builder = Builders<User>.Filter;
            var idFilter = builder.Eq(u => u.Email, email);
            var cursor = Users.Find(idFilter);
            User users = cursor.FirstOrDefault();
            Debug.WriteLine("Email: " + users.Email);
            return users;
            //return await Users.Find(new BsonDocument("Email", new ObjectId(email))).FirstOrDefaultAsync();
        }

        // обновление документа
        public async Task Update(User p)
        {
            User t1 = await Users.Find(new BsonDocument("_id", new ObjectId(p.Id))).FirstOrDefaultAsync();
            p.Id = t1.Id;
            p.Password = t1.Password;
            await Users.FindOneAndReplaceAsync(new BsonDocument("_id", new ObjectId(p.Id)), p);
        }

        public async Task UpdatePassword(User p)
        {
            User t1 = await Users.Find(new BsonDocument("_id", new ObjectId(p.Id))).FirstOrDefaultAsync();
            p.Id = t1.Id;
            p.Email = t1.Email;
            p.Familia = t1.Familia;
            p.Ima = t1.Ima;
            p.Role = t1.Role;
            await Users.FindOneAndReplaceAsync(new BsonDocument("_id", new ObjectId(p.Id)), p);
        }

        // удаление документа
        public async Task Remove(string id)
        {
            await Users.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }

        public async Task RemoveZv(string id)
        {
            await ObratsZvonki.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }

        public bool CheckExistZaavkaObrZvonok(string phone)
        {
            bool exists = ObratsZvonki.Find(_ => _.Telefon == phone).Any();
            return exists;
        }

        public async Task CreateZaavka(ObratZvonok p)
        {
            await ObratsZvonki.InsertOneAsync(p);
        }
    }
}
