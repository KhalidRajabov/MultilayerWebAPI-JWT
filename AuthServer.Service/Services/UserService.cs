using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using AuthServer.Data.Configurations;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserApp>? _userManager;

        public UserService(UserManager<UserApp>? userManager)
        {
            _userManager = userManager;
        }

        public async Task<Response<UserAppDTO>> CreateUserAsync(CreateUserDTO userDTO)
        {
            var user = new UserApp { Email = userDTO.Email, UserName = userDTO.UserName };
            var result = await _userManager.CreateAsync(user, userDTO.Password);
            if(!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();
                return Response<UserAppDTO>.Fail(new ErrorDTO(errors, true),400);
            }
            return Response<UserAppDTO>.Success(ObjectMapper.Mapper.Map<UserAppDTO>(user),200);
        }

        public async Task<Response<UserAppDTO>> GetUserByNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return Response<UserAppDTO>.Fail("User not found", 400, true);
            return Response<UserAppDTO>.Success(ObjectMapper.Mapper.Map<UserAppDTO>(user), 200);
        }
    }
}
