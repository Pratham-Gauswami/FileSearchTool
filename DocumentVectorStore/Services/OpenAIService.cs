using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DocumentVectorStore.Services
{
    public class OpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string BaseUrl = "https://api.openai.com/v1";
        private readonly JsonSerializerOptions _jsonOptions;

        public OpenAIService(IConfiguration configuration)
        {
            _apiKey = configuration["OpenAI:ApiKey"];
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        public async Task<(string fileId, long fileSize)> UploadFileAsync(Stream fileStream, string fileName)
        {
            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            
            content.Add(fileContent, "file", fileName);
            content.Add(new StringContent("assistants"), "purpose");

            var response = await _httpClient.PostAsync($"{BaseUrl}/files", content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var fileData = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
            
            return (fileData.GetProperty("id").GetString(), 
                    fileData.GetProperty("bytes").GetInt64());
        }

        public async Task<string> CreateVectorStoreAsync(string storeName)
        {
            var requestBody = new
            {
                name = storeName
            };

            var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{BaseUrl}/vector_stores", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"OpenAI API Error: {response.StatusCode} - {responseContent}");
            }

            var storeData = JsonSerializer.Deserialize<JsonElement>(responseContent);
            return storeData.GetProperty("id").GetString();
        }

        public async Task AttachFileToVectorStoreAsync(string vectorStoreId, string fileId)
        {
            var requestBody = new
            {
                file_id = fileId
            };

            var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                $"{BaseUrl}/vector_stores/{vectorStoreId}/files", 
                content
            );
            
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"OpenAI API Error: {response.StatusCode} - {responseContent}");
            }
        }

        public async Task<string> CreateAssistantAsync(string vectorStoreId)
        {
            var requestBody = new
            {
                model = "gpt-4-turbo-preview",
                name = "Document Assistant",
                instructions = "You are a helpful assistant that answers questions based on the uploaded documents. Use the file search tool to find relevant information.",
                tools = new[] { new { type = "file_search" } },
                tool_resources = new
                {
                    file_search = new
                    {
                        vector_store_ids = new[] { vectorStoreId }
                    }
                }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync($"{BaseUrl}/assistants", content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var assistantData = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
            
            return assistantData.GetProperty("id").GetString();
        }

        public async Task<string> CreateThreadAsync()
        {
            var response = await _httpClient.PostAsync($"{BaseUrl}/threads", null);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var threadData = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
            
            return threadData.GetProperty("id").GetString();
        }

        public async Task AddMessageToThreadAsync(string threadId, string message)
        {
            var requestBody = new
            {
                role = "user",
                content = message
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                $"{BaseUrl}/threads/{threadId}/messages", 
                content
            );
            response.EnsureSuccessStatusCode();
        }

        public async Task<string> RunAssistantAsync(string threadId, string assistantId)
        {
            var requestBody = new
            {
                assistant_id = assistantId
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                $"{BaseUrl}/threads/{threadId}/runs", 
                content
            );
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var runData = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
            
            return runData.GetProperty("id").GetString();
        }

        public async Task<string> GetRunStatusAsync(string threadId, string runId)
        {
            var response = await _httpClient.GetAsync(
                $"{BaseUrl}/threads/{threadId}/runs/{runId}"
            );
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var runData = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
            
            return runData.GetProperty("status").GetString();
        }

        public async Task<string> GetLatestMessageAsync(string threadId)
        {
            var response = await _httpClient.GetAsync(
                $"{BaseUrl}/threads/{threadId}/messages?limit=1&order=desc"
            );
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var messages = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
            var data = messages.GetProperty("data");
            
            if (data.GetArrayLength() > 0)
            {
                var message = data[0];
                var content = message.GetProperty("content")[0];
                return content.GetProperty("text").GetProperty("value").GetString();
            }

            return string.Empty;
        }
    }
}