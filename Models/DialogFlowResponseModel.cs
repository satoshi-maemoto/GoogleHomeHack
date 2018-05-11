using Newtonsoft.Json;

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
}
