using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using PagedList;

namespace ShopMagazin.Models
{
    public class PoiskUsers
    {
        public string ZhachPoiska { get; set; }       
        public string ZnachPoiskaRole { get; set; }        
    }

    public class UsersPages
    {
        public IEnumerable<User> Users { get; set; }
        //public IPagedList<User> Users { get; set; }
        public string ZhachPoiska { get; set; }
        public string ZnachPoiskaRole { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
