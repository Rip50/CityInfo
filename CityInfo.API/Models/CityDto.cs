﻿namespace CityInfo.API.Models
{
    public class CityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<PointOfInterestDto> PointsOfInterests { get; set; } 
            = new List<PointOfInterestDto>();

        public int NumberOfPointsOfInterest => PointsOfInterests.Count;
    }
}