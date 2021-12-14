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

namespace ShopMagazin.Models
{
    public class IndexPageModel
    {
        public IEnumerable<Categorii> Categs { get; set; }
        public IEnumerable<PodCategorii> podCategoriis { get; set; }
        public IEnumerable<Tovar> Tovari { get; set; }
    }
}
