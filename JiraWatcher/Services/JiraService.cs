using JiraWatcher.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using JiraWatcher.Helpers;

namespace JiraWatcher.Services
{
    public class JiraService : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _jiraBaseUrl;
        private readonly string _jiraApiUsername;
        private readonly string _jiraApiPassword;
        private string _JQL;

        public JiraService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jiraBaseUrl = Properties.Settings.Default.JiraApiURL;
            _jiraApiUsername = Properties.Settings.Default.JiraApiUsername;
            _jiraApiPassword = Properties.Settings.Default.JiraApiPassword;
            _JQL = JqlHelper.ValidateAndEncodeJql(Properties.Settings.Default.JQL);
        }

        public async Task<List<JiraItem>> GetJiraItemsAsync()
        {
            try
            {
                _httpClient.BaseAddress = new Uri(_jiraBaseUrl);
                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_jiraApiUsername}:{_jiraApiPassword}"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                HttpResponseMessage response = await _httpClient.GetAsync($"3/search?os_authType=basic&jql={_JQL}");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                return ParseIssues(responseBody);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Jira items: {ex.Message}");
                return null;
            }
        }

        private List<JiraItem> ParseIssues(string responseBody)
        {
            List<JiraItem> issues = new List<JiraItem>();

            JObject jsonResponse = JObject.Parse(responseBody);

            JArray issuesArray = (JArray)jsonResponse["issues"];

            foreach (JToken issueToken in issuesArray)
            {
                JObject fields = (JObject)issueToken["fields"];

                string key = issueToken["key"].ToString();
                string summary = fields["summary"].ToString();

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
            _httpClient?.Dispose();
        }
    }
}
