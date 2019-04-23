﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [Route("api/cities")]
    public class CitiesController : Controller
    {
        [HttpGet()]
        public JsonResult GetCities()
        {
            return new JsonResult(new List<object>()
            {
                new {id = 1, Name = "New York City"},
                new {id = 2, Name = "Amsterdam"}
            });
        }
    }
}