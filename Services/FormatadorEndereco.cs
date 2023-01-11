using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class FormatadorEndereco : IFormatadorEndereco
{
    private int contadorUso = 0;

    public async Task Formatar(HttpContext context, IEndereco endereco)
    {
        contadorUso ++;

        StringBuilder html = new StringBuilder();
        html.Append($"CEP {endereco.CEP}\n");
        html.Append($"Logradouro: {endereco.Logradouro}\n");
        html.Append($"Bairro: {endereco.Bairro}\n");
        html.Append($"Cidade / UF: {endereco.Estado} / {endereco.Localidade}\n");
        html.Append($"\nServiço utilizado {contadorUso} {((contadorUso > 1) ? "vezes" : "vez")}.");

        context.Response.ContentType = "text/plain; charset=utf-8";
        await context.Response.WriteAsync(html.ToString());
    }
}