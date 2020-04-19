using BingeBuddyNg.Services.DrinkEvent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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
        public async Task<DrinkEvent> GetCurrentDrinkEvent()
        {
            var result = await this.drinkEventRepository.FindCurrentDrinkEventAsync();

            return result;
        }
    }
}
