public interface IEndereco
{
    string CEP { get; set; }
    string Logradouro { get; set; }
    string Complemento { get; set; }
    string Bairro { get; set; }
    string Localidade { get; set; }
    string Estado { get; set; }
}