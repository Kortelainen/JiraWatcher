using JiraWatcher.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace JiraWatcher.Services
{
    public class JiraService : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _jiraBaseUrl;
        private readonly string _jiraApiUsername;
        private readonly string _jiraApiPassword;

        public JiraService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jiraBaseUrl = Properties.Settings.Default.JiraApiURL;
            _jiraApiUsername = Properties.Settings.Default.JiraApiUsername;
            _jiraApiPassword = Properties.Settings.Default.JiraApiPassword;
        }

        public async Task<List<JiraItem>> GetJiraItemsAsync(string jql)
        {
            _httpClient.BaseAddress = new Uri(_jiraBaseUrl);
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_jiraApiUsername}:{_jiraApiPassword}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var searchPayload = new
            {
                jql,
                fields = new[] { "summary" }
            };

            var searchContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(searchPayload), Encoding.UTF8, "application/json");
            HttpResponseMessage searchResponse = await _httpClient.PostAsync("rest/api/3/search/jql", searchContent);
            searchResponse.EnsureSuccessStatusCode();

            string searchResponseBody = await searchResponse.Content.ReadAsStringAsync();
            return ParseIssues(searchResponseBody);
        }

        private List<JiraItem> ParseIssues(string responseBody)
        {
            List<JiraItem> issues = new List<JiraItem>();

            JObject jsonResponse = JObject.Parse(responseBody);
            JArray? issuesArray = jsonResponse["issues"] as JArray;

            if (issuesArray == null)
            {
                return issues;
            }

            foreach (JToken issueToken in issuesArray)
            {
                JObject? fields = issueToken["fields"] as JObject;
                string? key = issueToken["key"]?.ToString();
                string? summary = fields?["summary"]?.ToString();

                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(summary))
                {
                    continue;
                }

                JiraItem issue = new JiraItem
                {
                    Key = key,
                    Summary = summary,
                    Link = Properties.Settings.Default.JiraURL + "/browse/" + key
                };

                issues.Add(issue);
            }

            return issues;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
