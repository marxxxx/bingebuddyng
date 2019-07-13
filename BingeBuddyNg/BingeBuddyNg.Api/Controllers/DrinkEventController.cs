using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Services.DrinkEvent;
using BingeBuddyNg.Services.Infrastructure;

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class DrinkEventController : Controller
    {
        public IDrinkEventRepository DrinkEventRepository { get; }
        public IIdentityService IdentityService { get; set; }

        public DrinkEventController(IIdentityService identityService, IDrinkEventRepository drinkEventRepository)
        {
            this.IdentityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.DrinkEventRepository = drinkEventRepository ?? throw new ArgumentNullException(nameof(drinkEventRepository));
        }

        [HttpGet("current")]
        public async Task<DrinkEvent> GetCurrentDrinkEvent()
        {
            var result = await this.DrinkEventRepository.FindCurrentDrinkEventAsync();

            return result;
        }
    }
}
