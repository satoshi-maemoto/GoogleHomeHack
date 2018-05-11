using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VoiceTextWebAPI.Client;

namespace ChomadoVoice
{
    public static class GoogleHome
    {
        [FunctionName("GoogleHome")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req
            , /* Azure Blob Storage(ファイル置き場) への出力 */ [Blob("mp3/voice.mp3", FileAccess.ReadWrite)] CloudBlockBlob mp3Out
            , TraceWriter log
        )
        {
            log.Info("C# HTTP trigger function processed a request.");
            try
            {
                var data = await req.Content.ReadAsAsync<Models.DialogFlowResponseModel>();
                var say = data.QueryRequest.QueryText;
                log.Info("SAY: " + say);

                // VoiceText Web API に投げる処理
                var key = ConfigurationManager.AppSettings.Get("VoiceTextAPIKey");
                log.Info("key: " + key);
                var voiceTextClient = new VoiceTextClient
                {
                    APIKey = key,
                    Speaker = Speaker.Bear,
                    Emotion = Emotion.Anger,
                    EmotionLevel = EmotionLevel.High,
                    Format = Format.MP3
                };
                var bytes = await voiceTextClient.GetVoiceAsync(text: say);

                // Azure Blob Storage への書き込み（保存）
                await mp3Out.UploadFromByteArrayAsync(buffer: bytes, index: 0, count: bytes.Length);

                // Azure Blob Storage に書き込まれた mp3 にアクセスするための URL
                var mp3Url = mp3Out.Uri;
                log.Info("MP3: " + mp3Url);

                var response =
                    "{" +
                    "  \"fulfillmentText\": " + $"\"<speak><audio src='{mp3Url}' /></speak>\"," +
                    "\"payload\": {" +
                    "  \"google\": {" +
                    "  \"expectUserResponse\": true," +
                    "  \"isSsml\": true," +
                    "  \"speech\": " + $"\"<speak><audio src='{mp3Url}' /></speak>\"" +
                    "  }" +
                    "}" +
                    "}";
                log.Info("Res: " + response);
                var result = req.CreateResponse(HttpStatusCode.OK, response);
                result.Headers.Add("ContentType", "application/json");
                return result;
            }
            catch (Exception e)
            {
                log.Info(e.GetType().Name + "\n" + e.StackTrace);
                throw e;
            }
        }
        
    }
}
