using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configurations;
using SharedLibrary.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

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


        //creates token that require registration
        IEnumerable<Claim> GetClaims(UserApp userApp, List<string> audiences) 
        {
            var userList = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userApp.Id),
                //code below can be written as ~new Claim("email", userApp.Email)~
                //because first parameter is always a string.
                //so ~JwtRegisteredClaimNames.Email~ is equal to "email" string
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Email, userApp.Email),

                //but claimtypes are important for something like "Roles", because ClaimTypes.Role lets
                //system understand that you are checking the user's role, not email or other things
                new Claim(ClaimTypes.Name, userApp.UserName),
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //creates a claim for each of list<string>audiences before sending to api
            //api checks whether or not the request has right to request
            userList.AddRange(audiences.Select(x => new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Aud, x)));
            return userList;
        }


        //creates token that does not require registry
        IEnumerable<Claim> GetClaimsByClient(Client? client)
        {
            var claims = new List<Claim>();
            claims.AddRange(client.Audience.Select(x => new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Aud, x)));
            new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
            new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub, client.Id.ToString());
            return claims;
        }

        public TokenDTO CreateToken(UserApp userApp)
        {
            var accessTokenExpression = DateTime.Now.AddMinutes(_customTokenOptions.AccessTokenExpiration);
            var refreshTokenExpression = DateTime.Now.AddMinutes(_customTokenOptions.RefreshTokenExpiration);
            var securityKey = SignService.GetSymmetricSecurityKey(_customTokenOptions.SecurityKey);
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _customTokenOptions.Issuer,
                expires: accessTokenExpression,
                notBefore: DateTime.Now,
                claims: GetClaims(userApp, _customTokenOptions.Audience),
                signingCredentials: signingCredentials
                );

            var handler = new JwtSecurityTokenHandler();
            var accessToken = handler.WriteToken(jwtSecurityToken);
            var tokenDto = new TokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = CreateRefreshToken(),
                AccessTokenExpiration = accessTokenExpression,
                RefreshTokenExpiration = refreshTokenExpression,
            };
            return tokenDto;
        }

        public ClientTokenDTO CreateTokenByClient(Client client)
        {
            var accessTokenExpression = DateTime.Now.AddMinutes(_customTokenOptions.AccessTokenExpiration);
            var securityKey = SignService.GetSymmetricSecurityKey(_customTokenOptions.SecurityKey);
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _customTokenOptions.Issuer,
                expires: accessTokenExpression,
                notBefore: DateTime.Now,
                claims: GetClaimsByClient(client),
                signingCredentials: signingCredentials
                );

            var handler = new JwtSecurityTokenHandler();
            var accessToken = handler.WriteToken(jwtSecurityToken);
            var tokenDto = new ClientTokenDTO
            {
                AccessToken = accessToken,
                AccessTokenExpiration = accessTokenExpression,
            };
            return tokenDto;
        }
    }
}