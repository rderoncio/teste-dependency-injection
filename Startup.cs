using DependencyInjection.Endpoint;
using DependencyInjection.Middlewares;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.Configuration;

namespace DependencyInjection
{
    public class Startup
    {
        private IConfiguration configuration;
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddSingleton<IFormatadorEndereco, FormatadorEnderecoHtml>();
            //services.AddTransient<IFormatadorEndereco, FormatadorEnderecoHtml>();
            //services.AddScoped<IFormatadorEndereco, FormatadorEnderecoHtml>();

            services.AddScoped<IFormatadorEndereco>(serviceProvider => 
            {
                string tipo = configuration["servicos:IFormatadorEndereco"];
                return (IFormatadorEndereco)ActivatorUtilities.CreateInstance(serviceProvider,
                    tipo == null ? typeof(FormatadorEnderecoPlain) : Type.GetType(tipo, true)
                );
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
                    IFormatadorEndereco formatador = context.RequestServices.GetService<IFormatadorEndereco>();
                    context.Response.ContentType = "text/html; charset=utf-8;";
                    await formatador.Formatar(context, await EndpointConsultaCep.ConsultaCep("01001000"));
                    await formatador.Formatar(context, await EndpointConsultaCep.ConsultaCep("04257143"));
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
                    IFormatadorEndereco formatador = context.RequestServices.GetService<IFormatadorEndereco>();
                    context.Response.ContentType = "text/html; charset=utf-8;";
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
