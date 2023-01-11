using Newtonsoft.Json;

public sealed class JsonCepModel : IEndereco
{
    [JsonProperty("cep")]
    public string CEP { get; set; }

    [JsonProperty("logradouro")]
    public string Logradouro { get; set; }

    [JsonProperty("complemento")]
    public string Complemento { get; set; }

    [JsonProperty("bairro")]
    public string Bairro { get; set; }

    [JsonProperty("localidade")]
    public string Localidade { get; set; }

    [JsonProperty("uf")]
    public string Estado { get; set; }

    [JsonProperty("ddd")]
    public string Ddd { get; set; }

    [JsonProperty("ibge")]
    public string Ibge { get; set; }

    [JsonProperty("erro")]
    public bool Erro { get; set; }
}