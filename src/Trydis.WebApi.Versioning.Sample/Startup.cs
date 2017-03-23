using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;

namespace Trydis.WebApi.Versioning.Sample
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();


            var formatter = config.Formatters.JsonFormatter;
            formatter.SerializerSettings = new JsonSerializerSettings
            {
                //Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.None,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            VersionedMediaTypeConfig.RegisterTypedFormatters(config);
            VersionConstraint.MediaTypePattern = "^application/vnd.versioningsample.([a-zA-Z]+)-v([0-9]+)\\+json$";

            appBuilder.UseWebApi(config);
        }
    } 
}