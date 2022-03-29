using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseService.Context;
using DatabaseService.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseService.Repository
{
    public interface ITemperaturRepository
    {
        Task<List<Temperatur>> GetTemperaturAsync(int take);
    }
    public class TemperaturRepository : ITemperaturRepository
    {
        private readonly IndeklimaContext _indeklimaContext;

        public TemperaturRepository(IndeklimaContext indeklimaContext)
        {
            _indeklimaContext = indeklimaContext;
        }

        public async Task<List<Temperatur>> GetTemperaturAsync(int take)
        {
            return await _indeklimaContext
                .Temperatur.OrderByDescending(p => p.Dato).Take(take)
                .ToListAsync();
        }
    }
}
