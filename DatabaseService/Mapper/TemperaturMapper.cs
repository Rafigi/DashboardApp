using System.Collections.Generic;
using System.Linq;
using DatabaseService.Dtos;
using DatabaseService.Models;

namespace DatabaseService.Mapper
{
    public static class TemperaturMapper
    {
        public static List<TemperaturDto> MapToDto(this List<Temperatur> temperaturs) =>
            temperaturs.Select(temperatur => MapToDto(temperatur)).ToList();

        public static TemperaturDto MapToDto(this Temperatur temperatur)
        {
            return new TemperaturDto()
            {
                Dato = temperatur.Dato,
                Grader = temperatur.Grader,
                Tidspunkt = temperatur.Tidspunkt
            };
        }
    }
}
