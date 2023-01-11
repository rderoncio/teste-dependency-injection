using DependencyInjection.Endpoint;
using DependencyInjection.Middlewares;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

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
            services.AddSingleton(typeof(ICollection<>), typeof(List<>));

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

            app.UseEndpoints(endpoint =>
            {
                endpoint.MapGet("/string", async context =>
                {
                    ICollection<string> lista = context.RequestServices.GetService<ICollection<string>>();
                    lista.Add($"Request: {DateTime.Now.ToLongTimeString()}");
                    context.Response.ContentType = "text/plain; charset=utf-8;";
                    foreach (var str in lista)
                    {
                        await context.Response.WriteAsync($"String: {str}\n");
                    }
                });
            });

            app.UseEndpoints(endpoint =>
            {
                endpoint.MapGet("/int", async context =>
                {
                    ICollection<int> lista = context.RequestServices.GetService<ICollection<int>>();
                    lista.Add(lista.Count + 1);
                    context.Response.ContentType = "text/plain; charset=utf-8;";
                    foreach (var val in lista)
                    {
                        await context.Response.WriteAsync($"Int: {val}\n");
                    }
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
