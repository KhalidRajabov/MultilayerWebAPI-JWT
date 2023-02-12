using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DTOs;

namespace PresentationAPI.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        public IActionResult ActionResultInstance<T>(Response<T> response)where T : class
        {
            return new ObjectResult(response)
            {
                StatusCode= response.StatusCode,
            };
        }
    }
}
