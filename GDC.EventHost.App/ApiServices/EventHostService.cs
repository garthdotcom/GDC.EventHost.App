using GDC.EventHost.App.ApiServices;
using GDC.EventHost.Shared.Series;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace GDC.EventHost.App.ApiServices
{
    public class EventHostService : IEventHostService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _config;

        public bool Error { get; private set; }
        public string? Messages { get; private set; }

        public EventHostService(HttpClient client, IConfiguration config)
        {
            _client = client;
            _config = config;

            var apiUri = config["ApiUri"] ?? throw new ArgumentNullException(nameof(config));
            var apiVersion = config["ApiVersionUriPrefix"];

            var baseAddress = $"{apiUri}/api/{apiVersion}";

            //_client.BaseAddress = new Uri(baseAddress);


        }

        public async Task<IEnumerable<T>> GetMany<T>(string uri)
        {
            var request = await _client
                .GetFromJsonAsync<IEnumerable<T>>($"{_config["ApiUri"]}/api/{_config["ApiVersionUriPrefix"]}{uri}");

            return request;

            //var request = new HttpRequestMessage(HttpMethod.Get,
            //    string.Concat(_config["ApiVersionUriPrefix"], uri));

            //var response = await _client.SendAsync(request);

            //if (request.IsSuccessStatusCode)
            //{
            //    using var responseStream = await response.Content.ReadAsStreamAsync();

            //    // need options here or the deserialization will return empty values 
            //    // https://www.pmichaels.net/2020/02/22/my-object-wont-deserialise-using-system-text-json/

            //    var options = new JsonSerializerOptions()
            //    {
            //        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            //    };

            //    var listOfObjects = await JsonSerializer
            //        .DeserializeAsync<IEnumerable<T>>(responseStream, options);

            //    Error = false;
            //    return listOfObjects ?? [];
            //}
            //else
            //{
            //    Error = true;
            //    return [];
            //}
        }

        public async Task<T?> GetOne<T>(string uri)
        {
            var request = await _client
                .GetFromJsonAsync<T>($"{_config["ApiUri"]}/api/{_config["ApiVersionUriPrefix"]}{uri}");

            return request;

            //var request = new HttpRequestMessage(HttpMethod.Get,
            //    string.Concat(_config["ApiVersionUriPrefix"], uri));

            //var response = await _client.SendAsync(request);

            //if (response.IsSuccessStatusCode)
            //{
            //    using var responseStream = await response.Content.ReadAsStreamAsync();

            //    var options = new JsonSerializerOptions()
            //    {
            //        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //        PropertyNameCaseInsensitive = true
            //    };

            //    var singleObject = await JsonSerializer.DeserializeAsync<T>(responseStream, options);

            //    Error = false;
            //    return singleObject;
            //}
            //else
            //{
            //    Error = true;
            //    return Activator.CreateInstance<T>();
            //}
        }

        // on creation the new object will be returned
        // on failure an empty object is returned; error flag set and errors property full
        public async Task<T> PostOne<T>(string uri, string stringData)
        {
            var request = new HttpRequestMessage(HttpMethod.Post,
                $"{_config["ApiUri"]}/api/{_config["ApiVersionUriPrefix"]}{uri}")
            {
                Content = new StringContent(stringData, Encoding.UTF8, "application/json")
            };

            var response = await _client.SendAsync(request);

            using var responseStream = await response.Content.ReadAsStreamAsync();

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            if (response.IsSuccessStatusCode)
            {
                responseStream.Position = 0;  // workaround
                var theNewObject = await JsonSerializer.DeserializeAsync<T>(responseStream, options);
                Error = false;
                return theNewObject;
            }
            else
            {
                // error occurred on the API, pull the errors out of the response
                // todo: get the error format and deserialize into that type
                var jsonString = await response.Content.ReadAsStringAsync();
                Messages = GetJsonErrorMessages(jsonString);
                Error = true;
                return Activator.CreateInstance<T>();
            }
        }


        // on update, nothing is returned; error flag is false and no errors
        // on failure, nothing is returned; error flag is true and errors exist
        public async Task PutOne(string uri, string stringData)
        {
            Error = false;

            var request = new HttpRequestMessage(HttpMethod.Put,
                $"{_config["ApiUri"]}/api/{_config["ApiVersionUriPrefix"]}{uri}")
            {
                Content = new StringContent(stringData, Encoding.UTF8, "application/json")
            };

            var response = await _client.SendAsync(request);

            using var responseStream = await response.Content.ReadAsStreamAsync();

            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                // error occurred on the API, pull the errors out of the response
                // todo: get the error format and deserialize into that type
                var jsonString = await response.Content.ReadAsStringAsync();
                Messages = GetJsonErrorMessages(jsonString);
                Error = true;
            }
        }


        // on update, nothing is returned; error flag is false and no errors
        // on failure, nothing is returned; error flag is true and errors exist
        public async Task PatchOne(string uri, string stringData)
        {
            Error = false;

            var request = new HttpRequestMessage(HttpMethod.Patch,
                $"{_config["ApiUri"]}/api/{_config["ApiVersionUriPrefix"]}{uri}")
            {
                Content = new StringContent(stringData, Encoding.UTF8, "application/json")
            };

            var response = await _client.SendAsync(request);

            using var responseStream = await response.Content.ReadAsStreamAsync();

            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                // error occurred on the API, pull the errors out of the response
                // todo: get the error format and deserialize into that type
                var jsonString = await response.Content.ReadAsStringAsync();
                Messages = GetJsonErrorMessages(jsonString);
                Error = true;
            }
        }

        public async Task DeleteOne(string uri)
        {
            Error = false;

            var request = new HttpRequestMessage(HttpMethod.Delete,
                $"{_config["ApiUri"]}/api/{_config["ApiVersionUriPrefix"]}{uri}");

            var response = await _client.SendAsync(request);

            using var responseStream = await response.Content.ReadAsStreamAsync();

            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                // error occurred on the API, pull the errors out of the response
                // todo: get the error format and deserialize into that type
                var jsonString = await response.Content.ReadAsStringAsync();
                Messages = GetJsonErrorMessages(jsonString);
                Error = true;
            }
        }

        /// <summary>
        /// Uses Json.Net to pull error message values from a json string
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns>String of error messages</returns>
        private string GetJsonErrorMessages(string jsonString)
        {
            try
            {
                var messageBuilder = new StringBuilder();

                JsonNode? json = JsonObject.Parse(jsonString);

                //var json = JsonObject.Parse(jsonString);

                var title = json["title"];

                if (title != null)
                {
                    messageBuilder.Append(json["title"].ToString());
                }

                var errors = json["errors"];

                if (errors != null)
                {
                    var someErrors = errors.AsArray()
                        .Deserialize<List<Error>>();  //.Children();

                    foreach (var error in someErrors)
                    {
                        foreach (var value in error.Values)
                        {
                            messageBuilder.Append(value);
                        }
                    }
                }

                if (messageBuilder.Length == 0)
                {
                    // errors were not in the format, so just return the raw string
                    return jsonString;
                }

                return messageBuilder.ToString().Replace(".", ". "); ;
            }
            catch
            {
                // Parse will throw an exception for invalid json
                //return string.Empty;
                return jsonString;
            }
        }

    }

    public class Error
    {
        public List<string> Values { get; set; }
    }
}
