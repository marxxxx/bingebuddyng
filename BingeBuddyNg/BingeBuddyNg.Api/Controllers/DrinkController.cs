using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Drink;
using BingeBuddyNg.Core.Drink.DTO;
using BingeBuddyNg.Core.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class DrinkController : Controller
    {
        private readonly IDrinkRepository drinkRepository;
        private readonly IIdentityService identityService;

        public DrinkController(IDrinkRepository drinkRepository, IIdentityService identityService)
        {
            this.drinkRepository = drinkRepository ?? throw new ArgumentNullException(nameof(drinkRepository));
            this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        [HttpGet]
        public async Task<IEnumerable<DrinkDTO>> Get()
        {
            var userId = this.identityService.GetCurrentUserId();
            var result = await this.drinkRepository.GetDrinksAsync(userId);
            return result;
        }

        [HttpGet("{drinkId}")]
        public async Task<DrinkDTO> GetById(string drinkId)
        {
            var userId = this.identityService.GetCurrentUserId();
            return await this.drinkRepository.GetDrinkAsync(userId, drinkId);
        }

        [HttpPost]
        public async Task SaveDrink([FromBody] IEnumerable<DrinkDTO> drinks)
        {
            var userId = this.identityService.GetCurrentUserId();
            await this.drinkRepository.SaveDrinksAsync(userId, drinks);
        }

        [HttpDelete("{drinkId}")]
        public async Task Delete(string drinkId)
        {
            var userId = this.identityService.GetCurrentUserId();
            await this.drinkRepository.DeleteDrinkAsync(userId, drinkId);
        }
    }
}