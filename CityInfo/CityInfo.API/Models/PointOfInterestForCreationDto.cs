using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Models
{
    // zie de inhoud (preview) van System.ComponentModel.DataAnnotations voor
    // beschikbare standaard constraints. Je kunt ook je eigen DataAnnotation maken
    // door af te leiden van een class (zie artsportaal)
    using System.ComponentModel.DataAnnotations;

    public class PointOfInterestForCreationDto
    {
        [Required(ErrorMessage = "You should provide a name value.")]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }
    }
}
