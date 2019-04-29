using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.DotNet.PlatformAbstractions;

    [Route("api/cities")]
    public class CitiesController : Controller
    {
        //[HttpGet()]
        //public JsonResult GetCities()
        //{
        //    return new JsonResult(new List<object>()
        //    {
        //        new {id = 1, Name = "New York City"},
        //        new {id = 2, Name = "Amsterdam"}
        //    });
        //    var temp = new JsonResult(CitiesDataStore.Current.Cities);
        //    temp.StatusCode = 200;
        //    return temp;
        //}

        [HttpGet()]
        public IActionResult GetCities()
        {
            return Ok(CitiesDataStore.Current.Cities);
        }

        //[HttpGet("{id}")]
        //public JsonResult GetCity(int id)
        //{
        //    return new JsonResult(CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id));
        //}

        [HttpGet("{id}")]
        public IActionResult GetCity(int id)
        {
            var cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);
            if (cityToReturn == null)
            {
                return NotFound();
            }

            return Ok(cityToReturn);
        }

    }
}
