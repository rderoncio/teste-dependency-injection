using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class FormatadorEnderecoHtml : IFormatadorEndereco
{
    private int contadorUso = 0;
    private Guid guid = Guid.NewGuid();

    public async Task Formatar(HttpContext context, IEndereco endereco)
    {
        StringBuilder html = new StringBuilder();
        html.Append($"<h3>CEP {endereco.CEP}</h3>");
        html.Append($"<p>Logradouro: {endereco.Logradouro}</p>");
        html.Append($"<p>Bairro: {endereco.Bairro}</p>");
        html.Append($"<p>Cidade/UF: {endereco.Localidade}/</p>");
        html.Append($"<p>Serviço utilizado {++contadorUso} {((contadorUso > 1) ? "vezes" : "vez")}.</p>");
        html.Append($"<p><small>GUID => {guid}</small></p>");

        //context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.WriteAsync(html.ToString());
    }
}