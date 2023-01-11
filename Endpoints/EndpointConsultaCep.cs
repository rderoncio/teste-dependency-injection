using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;

namespace DependencyInjection.Endpoint
{
    public static class EndpointConsultaCep
    {
        public static async Task Endpoint(HttpContext context)
        {
            string cep = context.Request.RouteValues["cep"] as string ?? "01001000";

            JsonCepModel jsonCepObjeto = await ConsultaCep(cep);

            if (!jsonCepObjeto.Erro)
            {
                StringBuilder html = new StringBuilder();
                html.Append($"<h3>Dados para o CEP {jsonCepObjeto.CEP} </h3>");
                html.Append($"<p>Logradouro: {jsonCepObjeto.Logradouro}</p>");
                html.Append($"<p>Bairro: {jsonCepObjeto.Bairro}</p>");
                html.Append($"<p>Cidade/UF: {jsonCepObjeto.Localidade}/{jsonCepObjeto.Estado}</p>");

                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync(html.ToString());
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
            }
        }

        public static async Task<JsonCepModel> ConsultaCep(string cep)
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
