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

        public async Task Invoke(HttpContext context)
        {

            if (context.Request.Path.StartsWithSegments("/middleware/classe"))
            {
                string cep = context.Request.RouteValues["cep"] as string ?? "01001000";
                JsonCepModel jsonCepObjeto = await ConsultaCep(cep);

                await FormatadorEndereco.Singleton.Formatar(context, jsonCepObjeto);
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
