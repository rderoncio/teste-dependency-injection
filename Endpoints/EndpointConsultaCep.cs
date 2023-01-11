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
                await TypeBroker.FormatadorEndereco.Formatar(context, jsonCepObjeto);
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
