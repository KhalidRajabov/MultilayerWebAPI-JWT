using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using SharedLibrary.DTOs;
using SharedLibrary.Exceptions;
using System.Text.Json;

namespace SharedLibrary.Extensions
{
    //class to return customized exception. Method needs to be called from program class.
    //Methid must be called before other middlewares so that if there is an exception, do not 
    //load server, just stop the job from first middleware
    public static class CustomExceptionHandler
    {
        public static void UseCustomException(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(config =>
            {
                config.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (errorFeature != null)
                    {
                        var ex = errorFeature.Error;

                        ErrorDTO errorDto = null;

                        if (ex is CustomException)
                        {
                            errorDto = new ErrorDTO(ex.Message, true);
                        }
                        else
                        {
                            errorDto = new ErrorDTO(ex.Message, false);
                        }

                        var response = Response<NoDataDTO>.Fail(errorDto, 500);

                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    }
                });
            });
        }
    }
}
