using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AzureApimAdminSso.Helpers
{
    public class ApimHelper
    {

        const string API_VERSION = "2014-02-14-preview";
        private string tenantName;
        private string sharedAccessSignature;
        private string baseUrl;



        public ApimHelper()
        {
            // Retrieve the base information from web.config if local / env variables if on App Service
            tenantName = ConfigurationManager.AppSettings["ApimTenant"];
            sharedAccessSignature = ConfigurationManager.AppSettings["ApimSas"];
            baseUrl = string.Format("https://{0}.management.azure-api.net", tenantName);


        }


        public async Task<string> RetrieveSsoUrl()
        {

            string requestUrl = string.Format("{0}/users/{1}/generateSsoUrl?api-version={2}", baseUrl, "1", API_VERSION);


            using (HttpClient httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("SharedAccessSignature", sharedAccessSignature);


                HttpResponseMessage response = await httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                return responseBody;

            }

        }
    }
}