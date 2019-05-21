using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    using AutoMapper;
    using CityInfo.API.Models;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Services;

    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;
        private ICityInfoRepository _cityInfoRepository;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, ICityInfoRepository cityInfoRepository)
        {
            _logger = logger;
            _mailService = mailService;
            _cityInfoRepository = cityInfoRepository;
            // Voorkeur heeft via constructor dependency injection.
            // Direct via de container een instantie opvragen: HttpContext.RequestServices.GetService()
        }

        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {                       
            if (!_cityInfoRepository.CityExists(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound();
            }

            var pointsOfInterestForCity = _cityInfoRepository.GetPointsOfInterestForCity(cityId);
            var pointsOfInterestForCityResults =
                Mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity);

            return Ok(pointsOfInterestForCityResults);

            // Without AutoMapper
            //var pointsOfInterestForCity = _cityInfoRepository.GetPointsOfInterestForCity(cityId);
            //var pointOfInterestForCityResults = new List<PointOfInterestDto>();
            //foreach (var poi in pointsOfInterestForCity)
            //{
            //    pointOfInterestForCityResults.Add(new PointOfInterestDto()
            //    { 
            //        Id = poi.Id,
            //        Name = poi.Name,
            //        Description = poi.Description
            //    });
            //}

            //return Ok(pointOfInterestForCityResults);

            // var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            // if (city == null)
            // {
            //    return NotFound();
            // }

            // return Ok(city.PointsOfInterest);
        }

        [HttpGet("{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            try
            {
                if (!_cityInfoRepository.CityExists(cityId))
                {
                    return NotFound();
                }

                var pointOfInterest = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
                if (pointOfInterest == null)
                {
                    return NotFound();
                }

                var pointOfInterestResult = Mapper.Map<PointOfInterestDto>(pointOfInterest);
                return Ok(pointOfInterestResult);

                // Without AutoMapper
                //var pointOfInterestResult = new PointOfInterestDto()
                //{
                //    Id = pointOfInterest.Id,
                //    Name = pointOfInterest.Name,
                //    Description = pointOfInterest.Description
                //};

                // return Ok(pointOfInterestResult);

                // test exception handling.. for demo purposes.
                // throw new Exception("Exception sample");
                //
                //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

                //if (city == null)
                //{
                //    _logger.LogInformation($"City with id {cityId} was not found wehn accessing points of interest.");
                //    return NotFound();
                //}

                //var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

                //if (pointOfInterest == null)
                //{
                //    return NotFound();
                //}

                //return Ok(pointOfInterest);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}", ex);
                return StatusCode(500, "A problem happened while handling your request.");
            }
        }

        [HttpPost("{cityId}/pointsOfInterest")]
        public IActionResult CreatePointOfInterest(int cityId,
            [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            // Voor een betere validatie mechanisme zie: https://github.com/jeremySkinner/FluentValidation
            // Nu validatie met DataAnnotation en in de controller..

            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", " The provided description should be different from the name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            // demo purposes - to be improved
            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(
                                                          c => c.PointsOfInterest)
                                                      .Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            // helper methods
            return CreatedAtRoute("GetPointOfInterest", new {cityId = cityId, id = finalPointOfInterest.Id}, finalPointOfInterest);

        }

        [HttpPut("{cityId}/pointsofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id,
            [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p =>
                p.Id == id);

            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            pointOfInterestFromStore.Name = pointOfInterest.Name;
            pointOfInterestFromStore.Description = pointOfInterest.Description;

            return NoContent();
        }

        [HttpPatch("{cityId}/pointsofinterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == id);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch =
                new PointOfInterestForUpdateDto()
                {
                    Name = pointOfInterestFromStore.Name,
                    Description = pointOfInterestFromStore.Description
                };

            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            TryValidateModel(pointOfInterestToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

            return NoContent();
        }

        [HttpDelete("{cityId}/pointsofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == id);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(pointOfInterestFromStore);

            _mailService.Send("Point of interest deleted.",
                $"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted.");


            return NoContent();
        }

    }
}
