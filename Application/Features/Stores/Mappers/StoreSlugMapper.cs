using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Platform.Catalog.API.Application.Features.Stores.Mappers;

public static class StoreSlugMapper
{
    private static readonly Regex MultiDashRegex = new("-{2,}", RegexOptions.Compiled);
    private static readonly Regex NonAlphaNumericRegex = new("[^a-z0-9]+", RegexOptions.Compiled);

    public static string ToStoreSlug(this string storeName)
    {
        if (string.IsNullOrWhiteSpace(storeName))
            return "store";

        var normalized = storeName.Trim().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                builder.Append(character);
        }

        var asciiValue = builder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        var slug = NonAlphaNumericRegex.Replace(asciiValue, "-");
        slug = MultiDashRegex.Replace(slug, "-").Trim('-');

        return string.IsNullOrWhiteSpace(slug) ? "store" : slug;
    }
}
