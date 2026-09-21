using E_Commerce.Domain.Entities.IdentityModule;
using E_Commerce.Services_Abstraction;
using E_Commerce.Shared.CommonResult;
using E_Commerce.Shared.DTOs.IdentityDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthenticationService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            this._userManager = userManager;
            this._configuration = configuration;
        }
        public async Task<Result<UserDTO>> LoginAsync(LoginDTO loginDTO)
        {
            var User = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (User is null)
                return Error.InvalidCredentials("User.InvalidCredentials");
           
            bool IsPasswordValid = await _userManager.CheckPasswordAsync(User, loginDTO.Password);
            if(!IsPasswordValid)
                return Error.InvalidCredentials("User.InvalidCredentials");
            var userDto = new UserDTO(User.Email!, User.DisplayName, await CreateTokenAsync(User));
            return userDto;
        }

        public async Task<Result<UserDTO>> RegisterAsync(RegisterDTO registerDTO)
        {
            var User = new ApplicationUser()
            {
                Email = registerDTO.Email,
                DisplayName = registerDTO.DisplayName,
                PhoneNumber = registerDTO.PhoneNumber,
                UserName = registerDTO.UserName
            };
            var IdentityResult = await _userManager.CreateAsync(User, registerDTO.Password);
            if(IdentityResult.Succeeded)
            {
                var userDto = new UserDTO(User.Email!, User.DisplayName, await CreateTokenAsync(User));
                return userDto;
            }
            return IdentityResult.Errors.Select(E => Error.Validation(E.Code, E.Description)).ToList();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var user = await _userManager.FindByEmailAsync(email);
            return user is not null;
        }

        public async Task<Result<UserDTO>> GetCurrentUserAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Error.UnAuthorized("User.UnAuthorized");

            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Error.NotFound("User.NotFound", $"User with email '{email}' was not found.");

            var userDto = new UserDTO(user.Email!, user.DisplayName, await CreateTokenAsync(user));
            return userDto;
        }

        private async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            var secretKey = _configuration["JWT:Key"] 
                ?? throw new InvalidOperationException("JWT Key is not configured.");
            var issuer = _configuration["JWT:Issuer"] 
                ?? throw new InvalidOperationException("JWT Issuer is not configured.");
            var audience = _configuration["JWT:Audience"] 
                ?? throw new InvalidOperationException("JWT Audience is not configured.");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }
    }
}
