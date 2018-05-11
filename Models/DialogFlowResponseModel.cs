using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace ChomadoVoice.Models
{
    /// <summary>
    /// DialogFlow から渡ってくる JSON をパースする用
    /// </summary>
    public class DialogFlowResponseModel
    {
        [JsonProperty("queryResult")]
        public QueryRequest QueryRequest { get; set; }
    }

    public partial class QueryRequest
    {
        [JsonProperty("queryText")]
        public string QueryText { get; set; }
    }

    public partial class Welcome
    {
        public static Welcome FromJson(string json) => JsonConvert.DeserializeObject<Welcome>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Welcome self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
