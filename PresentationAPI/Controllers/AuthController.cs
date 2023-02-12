using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PresentationAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //created "CustomBaseController" that inherites ControllerBase, 
    //then authcontroller inherites CBC controller so that auth becomes a controller
    //then adding it a method to get status code without doing if else statements in this controller
    public class AuthController : CustomBaseController
    {
        private readonly IAuthenticationService? _authenticationService;

        public AuthController(IAuthenticationService? authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken(LoginDTO loginDTO)
        {
            var result = await _authenticationService.CreateTokenAsync(loginDTO);

            //this method below gives current status code for result
            return ActionResultInstance(result);
        }

        [HttpPost]
        public IActionResult CreateTokenByClient(ClientLoginDTO clientLoginDTO)
        {
            var result=_authenticationService.CreateTokenByClient(clientLoginDTO);
            return ActionResultInstance(result);
        }

        [HttpPost]
        public async Task<IActionResult> RevokeRefreshToken(string refreshToken)
        {
            var result = await _authenticationService.RevokeRefreshToken(refreshToken);
            return ActionResultInstance(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTokenByRefreshToken(RefreshTokenDto refreshToken)
        {
            var result = await _authenticationService.CreateTokenByRefreshToken(refreshToken.Token);
            return ActionResultInstance(result);
        }
    }
}