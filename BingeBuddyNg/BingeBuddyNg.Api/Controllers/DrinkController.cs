using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Drink;
using BingeBuddyNg.Services.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class DrinkController : Controller
    {
        public DrinkController(IDrinkRepository drinkRepository, IIdentityService identityService)
        {
            DrinkRepository = drinkRepository ?? throw new ArgumentNullException(nameof(drinkRepository));
            IdentityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        public IDrinkRepository DrinkRepository { get; }
        public IIdentityService IdentityService { get; }

        // GET: api/<controller>
        [HttpGet]
        public async Task<IEnumerable<Drink>> Get()
        {
            var userId = this.IdentityService.GetCurrentUserId();
            return await this.DrinkRepository.GetDrinksAsync(userId);
        }

      

        // POST api/<controller>
        [HttpPost]
        public async Task Post([FromBody]IEnumerable<Drink> drinks)
        {
            var userId = this.IdentityService.GetCurrentUserId();
            await this.DrinkRepository.SaveDrinksAsync(userId, drinks);
        }

     

        // DELETE api/<controller>/5
        [HttpDelete("{drinkId}")]
        public async Task Delete(string drinkId)
        {
            var userId = this.IdentityService.GetCurrentUserId();
            await this.DrinkRepository.DeleteDrinkAsync(userId, drinkId);
        }
    }
}
