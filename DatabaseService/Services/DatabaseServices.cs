using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseService.Dtos;
using DatabaseService.Mapper;
using DatabaseService.Repository;

namespace DatabaseService.Services
{
    public interface IDatabaseServices
    {
        Task<List<TemperaturDto>> GetTemperatursAsync(int take);
    }

    public class DatabaseServices : IDatabaseServices
    {
        private readonly ITemperaturRepository _temperaturRepository;

        public DatabaseServices(ITemperaturRepository temperaturRepository)
        {
            _temperaturRepository = temperaturRepository;
        }
        public async Task<List<TemperaturDto>> GetTemperatursAsync(int take)
        {
           var temperaturAsync = await _temperaturRepository.GetTemperaturAsync(take);
           return temperaturAsync.MapToDto();
        }
    }
}
