using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;
using Trydis.WebApi.Versioning.Formatting;

namespace Trydis.WebApi.Versioning
{
    /// <summary>
    /// Registers the versioned media types.
    /// </summary>
    public static class VersionedMediaTypeConfig
    {
        public static void RegisterTypedFormatters(HttpConfiguration config, JsonMediaTypeFormatter defaultFormatter = null, params Assembly[] assembliesToScan)
        {
            var jsonMediaTypeFormatter = defaultFormatter ?? config.Formatters.JsonFormatter;
            if (jsonMediaTypeFormatter == null)
                throw new ArgumentNullException(nameof(defaultFormatter), $"{nameof(defaultFormatter)} should be set when Formatters.JsonFormatter is null");

            if (assembliesToScan == null || assembliesToScan.Length == null)
            {
                assembliesToScan = new[] {Assembly.GetCallingAssembly()};
            }

            foreach (var type in assembliesToScan.SelectMany(a=> a.GetTypes()))
            {
                var customAttributes = type.GetCustomAttributes(true);
                if (customAttributes.Length == 0) continue;
                var matchingCustomAttributes = customAttributes.OfType<VersionedMediaType>();

                foreach (var matchingCustomAttribute in matchingCustomAttributes)
                {
                    var formatterExists = config.Formatters
                        .OfType<TypedJsonMediaTypeFormatter>()
                        .Any(typeFormatter => typeFormatter.CanReadType(type) && typeFormatter.CanWriteType(type));

                    if (!formatterExists)
                    {                        
                        config.Formatters.Add(new TypedJsonMediaTypeFormatter(type, new MediaTypeHeaderValue(matchingCustomAttribute.MediaType), jsonMediaTypeFormatter));
                    }
                }
            }
        }
    }
}