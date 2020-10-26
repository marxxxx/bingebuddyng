using System;
using System.Threading.Tasks;
using BingeBuddyNg.Core.DrinkEvent;
using BingeBuddyNg.Core.DrinkEvent.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class DrinkEventController : Controller
    {
        private readonly IDrinkEventRepository drinkEventRepository;

        public DrinkEventController(IDrinkEventRepository drinkEventRepository)
        {
            this.drinkEventRepository = drinkEventRepository ?? throw new ArgumentNullException(nameof(drinkEventRepository));
        }

        [HttpGet("current")]
        public async Task<DrinkEventDTO> GetCurrentDrinkEvent()
        {
            var result = await this.drinkEventRepository.FindCurrentDrinkEventAsync();
            return result?.ToDto();
        }
    }
}