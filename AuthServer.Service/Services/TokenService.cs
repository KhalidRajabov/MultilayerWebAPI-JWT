using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SharedLibrary.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<UserApp>? _userManager;
        private readonly CustomTokenOptions? _customTokenOptions;

        public TokenService(UserManager<UserApp>? userManager, IOptions<CustomTokenOptions> options)
        {
            _userManager = userManager;
            _customTokenOptions = options.Value;
        }

        string CreateRefreshToken()
        {
            var numberByte=new byte[32];

            //returns a cryptographic number generator
            using var rnd = RandomNumberGenerator.Create();

            //sends its value to numberByte
            rnd.GetBytes(numberByte);

            //creates a guid like string combination, better than guid
            return Convert.ToBase64String(numberByte);
        }
        IEnumerable<Claim> GetClaims(UserApp userApp, List<string> audiences) 
        {
            var userList = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userApp.Id),
                //code below can be written as ~new Claim("email", userApp.Email)~
                //because first parameter is always a string.
                //so ~JwtRegisteredClaimNames.Email~ is equal to "email" string
                new Claim(JwtRegisteredClaimNames.Email, userApp.Email),

                //but claimtypes are important for something like "Roles", because ClaimTypes.Role lets
                //system understand that you are checking the user's role, not email or other things
                new Claim(ClaimTypes.Name, userApp.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //creates a claim for each of list<string>audiences before sending to api
            //api checks whether or not the request has right to request
            userList.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            return userList;
        }

        public TokenDTO CreateToken(UserApp userApp)
        {
            return null;
        }

        public ClientTokenDTO CreateTokenByClient(Client client)
        {
            return null;
        }
    }
}
