using System.Linq;
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

            // CASO DE USO PARA .ADDSINGLETON<>()
            // Mapeia uma nova rota para 'path' apontando para o 'T'
            // app.MapGet(path, (RequestDelegate)methodInfo.CreateDelegate(typeof(RequestDelegate), instanceEndpoint));
        
            // CASO DE USO PARA .ADDSCOPED<>()
            // Retorna os parametros do método
            ParameterInfo[] parameterInfo = methodInfo.GetParameters();

            app.MapGet(path, context =>
                (Task)methodInfo.Invoke(instanceEndpoint, parameterInfo.Select(
                    p => p.ParameterType == typeof(HttpContext) ? context : context.RequestServices.GetService(p.ParameterType)).ToArray()
                ));
        
        }
    }
}