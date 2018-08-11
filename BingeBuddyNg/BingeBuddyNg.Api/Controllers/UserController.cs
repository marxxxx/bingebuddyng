using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BingeBuddyNg.Api.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        public IUserRepository UserRepository { get; }

        public UserController(IUserRepository userRepository)
        {
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }
                

        // POST api/<controller>
        [HttpPost]
        public async Task Post([FromBody]User user)
        {
            await this.UserRepository.SaveUserAsync(user);
        }

    }
}
