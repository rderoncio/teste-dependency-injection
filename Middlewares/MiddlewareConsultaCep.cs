using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DependencyInjection.Middlewares
{
    public class MiddlewareConsultaCep
    {
        private readonly RequestDelegate _next;

        public MiddlewareConsultaCep(RequestDelegate next)
        {
            _next = next;
        }

        public MiddlewareConsultaCep()
        {
        }

        public async Task Invoke(HttpContext context, IFormatadorEndereco formatador1, IFormatadorEndereco formatador2)
        {

            if (context.Request.Path.StartsWithSegments("/middleware/classe"))
            {
                string cep = context.Request.RouteValues["cep"] as string ?? "01001000";

                JsonCepModel jsonCepObjeto1 = await ConsultaCep(cep);
                JsonCepModel jsonCepObjeto2 = await ConsultaCep("04257143");
                context.Response.ContentType = "text/html; charset=utf-8;";
                await formatador1.Formatar(context, jsonCepObjeto1);
                await formatador2.Formatar(context, jsonCepObjeto2);
            }

            if (_next != null)
            {
                await _next(context);
            }
        }

        private async Task<JsonCepModel> ConsultaCep(string cep)
        {
            cep = cep.Replace("-", "").Replace(" ", "");
            string url = $"http://viacep.com.br/ws/{cep}/json";

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Middleware Consulta CEP");
            var response = await client.GetAsync(url);

            string dadosCEP = await response.Content.ReadAsStringAsync();
            dadosCEP = dadosCEP.Replace("?(", "").Replace(")", "").Trim();

            return JsonConvert.DeserializeObject<JsonCepModel>(dadosCEP);
        }
    }
}
