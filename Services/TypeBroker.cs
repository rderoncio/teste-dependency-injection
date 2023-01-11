public static class TypeBroker
{
    private static IFormatadorEndereco instance = new FormatadorEnderecoHtml();
    public static IFormatadorEndereco FormatadorEndereco => instance;
}