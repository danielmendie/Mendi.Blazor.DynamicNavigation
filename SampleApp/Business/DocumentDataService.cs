using CountryApp.Abstractions.Models;
using CountryApp.Abstractions.Services.Data;
using System.Reflection;

namespace CountryApp.Business
{
    public class DocumentDataService : IDocumentDataService
    {
        public async Task<T?> GetConfiguration<T>(string settingType)
        {
            await Task.Delay(1);
            var resourceName = $"CountryApp.Abstractions.Data.{settingType}.latest.json";
            var data = GetMetadataJson(resourceName);
            var retVal = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(data);

            if (retVal == null)
            {
                throw new FileNotFoundException($"Unable to deserialize location data");
            }

            return retVal;
        }

        public async Task<string> GetDocumentType(string documentType)
        {
            await Task.Delay(1);
            var resourceName = $"CountryApp.Abstractions.Data.{documentType}";
            var retVal = GetMetadataJson(resourceName);

            return retVal;
        }

        private string GetMetadataJson(string resourceName)
        {
            string? data = GetMetadataFile(resourceName);
            if (data == null)
            {
                throw new FileNotFoundException($"Unable to find '{resourceName}' json file");
            }

            return data;
        }

        string? GetMetadataFile(string resourceName)
        {
            string? data = null;

            var assembly = Assembly.GetAssembly(typeof(AppSettings));
            if (assembly != null)
            {
                using Stream? stream = assembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    using StreamReader reader = new StreamReader(stream);
                    data = reader.ReadToEnd();
                }
            }
            return data;
        }
    }
}
