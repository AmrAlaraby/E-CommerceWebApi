using E_Commerce.Shared.CommonResult;
using E_Commerce.Shared.DTOs.IdentityDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services_Abstraction
{
    public interface IAuthenticationService
    {
        // login
        // email , Password => Token , DisplayName , Email
        Task<Result<UserDTO>> LoginAsync(LoginDTO loginDTO);
        // register
        // Email , Password , UserName , DisplayName , PhoneNumber => Token , DisplayName , Email
        Task<Result<UserDTO>> RegisterAsync(RegisterDTO registerDTO);

        // check email exists
        Task<bool> EmailExistsAsync(string email);

        // get current user
        Task<Result<UserDTO>> GetCurrentUserAsync(string email);
    }
}
