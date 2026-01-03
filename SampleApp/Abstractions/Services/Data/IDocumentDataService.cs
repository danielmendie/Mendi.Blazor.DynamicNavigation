namespace CountryApp.Abstractions.Services.Data
{
    public interface IDocumentDataService
    {
        Task<string> GetDocumentType(string documentType);
        Task<T?> GetConfiguration<T>(string settingType);
    }
}
