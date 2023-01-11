using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class FormatadorEnderecoPlain : IFormatadorEndereco
{
    private int contadorUso = 0;

    private static FormatadorEnderecoPlain _instance;

    public async Task Formatar(HttpContext context, IEndereco endereco)
    {
        StringBuilder html = new StringBuilder();
        html.Append($"CEP {endereco.CEP}\n");
        html.Append($"Logradouro: {endereco.Logradouro}\n");
        html.Append($"Bairro: {endereco.Bairro}\n");
        html.Append($"Cidade/UF: {endereco.Localidade}/{endereco.Estado}\n");
        html.Append($"\nServiço utilizado {++contadorUso} {((contadorUso > 1) ? "vezes" : "vez")}.\n");

        context.Response.ContentType = "text/plain; charset=utf-8";
        await context.Response.WriteAsync(html.ToString());
    }

    public static FormatadorEnderecoPlain Singleton
    {
        get
        {
            if (_instance == null)
            {
                _instance = new FormatadorEnderecoPlain();
            }
            return _instance;
        }
    }
}