using DependencyInjection.Endpoint;
using DependencyInjection.Middlewares;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;

namespace DependencyInjection
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IFormatadorEndereco, FormatadorEnderecoHtml>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IFormatadorEndereco formatador)
        {
            app.UseDeveloperExceptionPage();

            app.UseRouting();

            // http://localhost:port/middleware/classe
            app.UseMiddleware<MiddlewareConsultaCep>();

            // http://localhost:port/middleware/lambda
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/middleware/lambda"))
                {
                    await formatador.Formatar(context,
                     await EndpointConsultaCep.ConsultaCep("01001000"));
                }
                else
                {
                    await next();
                }
            });

            app.UseEndpoints(endpoints =>
            {
                // http://localhost:port/endpoint/classe
                endpoints.MapEndpoint<EndpointConsultaCep>("/endpoint/classe/{cep:regex(^\\d{{8}}$)?}");

                // http://localhost:port/endpoint/lambda
                endpoints.MapGet("/endpoint/lambda/{cep:regex(^\\d{{8}}$)?}", async context =>
                {
                    string cep = context.Request.RouteValues["cep"] as string ?? "01001000";
                    await formatador.Formatar(context,
                     await EndpointConsultaCep.ConsultaCep(cep));
                });
            });


            // http://localhost:port
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Middleware Terminal.");
            });
        }
    }
}
