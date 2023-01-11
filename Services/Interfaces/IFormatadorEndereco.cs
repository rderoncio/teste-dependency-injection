using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public interface IFormatadorEndereco
{
    Task Formatar (HttpContext context, IEndereco endereco);
}