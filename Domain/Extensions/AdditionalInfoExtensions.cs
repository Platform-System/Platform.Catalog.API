using Platform.BuildingBlocks.Extensions;
using System.Text.Json;

namespace Platform.Catalog.API.Domain.Extensions
{
    public static class AdditionalInfoExtensions
    {
        public const string BlobKey = "blob";

        public static JsonDocument Set<T>(
            this JsonDocument? doc,
            string key,
            T value)
        {
            var dict = doc.ToDictionary();
            dict[key] = value;
            return dict.ToJsonDocument();
        }

        public static T? Get<T>(
            this JsonDocument? doc,
            string key)
        {
            if (doc == null) return default;
            return doc.GetProperty<T>(key);
        }
    }
}
