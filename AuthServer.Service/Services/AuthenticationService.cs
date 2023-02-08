using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Repository;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<Client>? _clients;
        private readonly ITokenService? _tokenService;
        private readonly UserManager<UserApp>? _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken>? _userRefreshTokenService;

        public AuthenticationService(UserManager<UserApp>? userManager, ITokenService? tokenService, IOptions<List<Client>>? clients, IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken>? userRefreshTokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _clients = clients.Value;
            _unitOfWork = unitOfWork;
            _userRefreshTokenService = userRefreshTokenService;
        }

        public async Task<Response<TokenDTO>> CreateTokenAsync(LoginDTO loginDTO)
        {
            //when user logs in, we check if user exists
            if (loginDTO == null) throw new ArgumentNullException(nameof(loginDTO));
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null) return Response<TokenDTO>.Fail("Email not found", 400, true);
            if (!await _userManager.CheckPasswordAsync(user, loginDTO.Password)) return Response<TokenDTO>.Fail("Email or password is wrong", 400, true);
            //if user exists, we create new refresh token to update their token
            var token = _tokenService.CreateToken(user);
            var userRefreshToken = await _userRefreshTokenService.Where(x => x.UserId == user.Id).SingleOrDefaultAsync();
            //if user does not have refresh token, we give him a new refresh token
            if (userRefreshToken == null) 
            {
                await _userRefreshTokenService.AddAsync(new UserRefreshToken { UserId = user.Id, Code = token.RefreshToken, Expiration = token.RefreshTokenExpiration });
            }
            //if user alread has a token, we update his refresh token
            else
            {
                userRefreshToken.Code=token.RefreshToken;
                userRefreshToken.Expiration=token.RefreshTokenExpiration;
            }

            await _unitOfWork.CommitAsync();
            return Response<TokenDTO>.Success(token, 200);

        }

        public Response<ClientTokenDTO> CreateTokenByClient(ClientLoginDTO loginDTO)
        {
            var client=_clients.SingleOrDefault(x=>x.Id==loginDTO.ClientId&&x.Secret==loginDTO.ClientSecret);
            if (client == null) return Response<ClientTokenDTO>.Fail("Client id or secret not found", 404, true);
            var token = _tokenService.CreateTokenByClient(client);
            return Response<ClientTokenDTO>.Success(token, 200);
        }

        public async Task<Response<TokenDTO>> CreateTokenByRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            if (existRefreshToken == null) return Response<TokenDTO>.Fail("Refresh token not found", 404, true);
            var user = await _userManager.FindByIdAsync(existRefreshToken.UserId);
            if (user==null) return Response<TokenDTO>.Fail("User id not found", 404, true);
            var token =_tokenService.CreateToken(user);
            existRefreshToken.Code = token.RefreshToken;
            existRefreshToken.Expiration = token.RefreshTokenExpiration;
            await _unitOfWork.CommitAsync();
            return Response<TokenDTO>.Success(200);
        }

        public async Task<Response<NoDataDTO>> RevokeRefreshToken(string refreshToken)
        {
            var existRefreshToken=await _userRefreshTokenService.Where(x=>x.Code==refreshToken).SingleOrDefaultAsync();
            if (existRefreshToken==null) return Response<NoDataDTO>.Fail("Refresh token not found", 404, true);
            _userRefreshTokenService.Remove(existRefreshToken);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDTO>.Success(200);
        }
    }
}
