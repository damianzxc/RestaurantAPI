﻿using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.DTOs;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [Route("api/account")]
    [ApiController] // Auto Validation
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _service;

        public AccountController(IAccountService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public ActionResult RegisterUser([FromBody] RegisterUserDto dto)
        {

            _service.RegisterUser(dto);
            return Ok();
        }
    }
}
