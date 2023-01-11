using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder
{
    public static class ExtensionMethod
    {
        public static void MapEndpoint<T>(this IEndpointRouteBuilder app, string path, string method = "Endpoint")
        {
            // Metadados para o parâmetro vindo em 'method'
            MethodInfo methodInfo = typeof(T).GetMethod(method);

            // 'method' Encontrado e é um Task
            if (methodInfo == null || methodInfo.ReturnType != typeof(Task))
                throw new System.Exception("Método não compatível!");

            // ActivatorUtilities.CreateInstance: Cria instância para o 'T'
            // app.ServiceProvider: Resolve parâmetros via Injeção de Dependências
            T instanceEndpoint = ActivatorUtilities.CreateInstance<T>(app.ServiceProvider);

            // Mapeia uma nova rota para 'path' apontando para o 'T'
            app.MapGet(path, (RequestDelegate)methodInfo.CreateDelegate(typeof(RequestDelegate), instanceEndpoint));
        }
    }
}