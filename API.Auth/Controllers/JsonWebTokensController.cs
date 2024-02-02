﻿using Crosscutting.Configuration.AuthPolicyConfiguration;
using Crosscutting.Configuration.JwtConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Auth.Controllers
{
    [Route("API/BC/Auth")]
    public class JsonWebTokensController : Controller
    {
        private readonly IConfiguration _configuration;
        public JsonWebTokensController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public class JwtModel
        {
            public string RefreshToken { get; set; } = null!;
            public string? Hwid { get; set; }
        }

        [HttpPost("JwtRefreshAndGenerate")]
        [AllowAnonymous]
        public async Task<IActionResult> Generate([FromBody] JwtModel body)
        {
            try
            {
                var claims = PolicyClaimAuth.ClaimsConfiguration(_configuration, hwid:body.Hwid);

                var jwt = await JwtApiResponse.JwtRefreshAndGenerate(claims, _configuration, body.RefreshToken, null!);

                return await Task.FromResult(Ok(jwt));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Unsuccessful Auth JWT Generation: " + ex.Message);
            }
        }
    }
}
