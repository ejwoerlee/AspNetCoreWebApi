using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Models;
using CityInfo.API.Services;

namespace CityInfo.API.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.DotNet.PlatformAbstractions;

    [Route("api/cities")]
    public class CitiesController : Controller
    {
        private ICityInfoRepository _cityInfoRepository;

        public CitiesController(ICityInfoRepository cityInfoRepository )
        {
            _cityInfoRepository = cityInfoRepository;
        }
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
            // return Ok(CitiesDataStore.Current.Cities);

            IEnumerable<Entities.City> cityEntities = _cityInfoRepository.GetCities();
            IEnumerable<CityWithoutPointsOfInterestDto> results = Mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities);

            // Without AutoMapper..
            // var results = new List<CityWithoutPointsOfInterestDto>();
            //foreach (var cityEntity in cityEntities)
            //{
            //    results.Add(new CityWithoutPointsOfInterestDto()
            //    {
            //        Id = cityEntity.Id,
            //        Description = cityEntity.Description,
            //        Name = cityEntity.Name
            //    });
            //}

            return Ok(results);

        }

        //[HttpGet("{id}")]
        //public JsonResult GetCity(int id)
        //{
        //    return new JsonResult(CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id));
        //}

        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePointsOfInterest = false)
        {
            Entities.City city = _cityInfoRepository.GetCity(id, includePointsOfInterest);

            if (city == null)
            {
                return NotFound();
            }

            if (includePointsOfInterest)
            {
                var cityResult = Mapper.Map<CityDto>(city);
                return Ok(cityResult);
                // Zonder Automapper..
                //var cityResult = new CityDto()
                //{
                //    Id = city.Id,
                //    Name = city.Name,
                //    Description = city.Description
                //};

                //foreach (var poi in city.PointsOfInterest)
                //{
                //    cityResult.PointsOfInterest.Add(
                //        new PointOfInterestDto()
                //        {
                //            Id = poi.Id,
                //            Name = poi.Name,
                //            Description = poi.Description
                //        });
                //}
                // return Ok(cityResult);
            }

            //Zonder AutoMapper
            //var cityWithoutPointsOfInterestResult =
            //    new CityWithoutPointsOfInterestDto()
            //    {
            //        Id = city.Id,
            //        Description = city.Description,
            //        Name = city.Name
            //    };
            var cityWithoutPointsOfInterestResult = Mapper.Map<CityWithoutPointsOfInterestDto>(city);

            return Ok(cityWithoutPointsOfInterestResult);

            //var cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);
            //if (cityToReturn == null)
            //{
            //    return NotFound();
            //}

            //return Ok(cityToReturn);
        }

    }
}
