using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEngine;

namespace i5.VirtualAgents.LLM
{
    public class WikiDataAPI : MonoBehaviour, IQueryAPI
    {
        /// <summary>
        /// The query type name that will be provided to the LLM to build the a query with the correct syntax.
        /// </summary>
        [Tooltip("Query type name that will be provided to the LLM to build the a query with the correct syntax.")]
        public string QueryTypeName { get; set; }
        /// <summary>
        /// Service name that will be provided to the LLM so that it could use datapoints known by the LLM.
        /// </summary>
        [Tooltip("Service name that will be provided to the LLM so that it could use datapoints known by the LLM.")]
        public string ServiceName { get; set; }
        /// <summary>
        /// This example data can provided further examples for the LLM of how a query should look like, what the specifed service name offers or include additional ids of datapoints.
        /// </summary>
        [Tooltip("This example data can provided further examples for the LLM of how a query should look like, what the specifed service name offers or include additional ids of datapoints.")]
        public string ExampleData { get; set; }

        /// <summary>
        /// Please enter your email address here. This is required by the WikiData API to idetify spam, see Wikidata Query Service User Manuel SPARQL Endpoint.
        /// </summary>
        [SerializeField]
        [Tooltip("Please enter your email address here. This is required by the WikiData API to idetify spam, see Wikidata Query Service User Manuel SPARQL Endpoint.")]
        private string email = "";
        //https://www.mediawiki.org/wiki/Wikidata_Query_Service/User_Manual#SPARQL_endpoint

        public WikiDataAPI()
        {
            QueryTypeName = "SPARQL";
            ServiceName = "WikiData";
            ExampleData = "The RWTH Aachen is Q273263 in WikiData.";
        }
        public async Task<string> SendQueryToAPI(string sparqlQuery)
        {
            if (email == "")
            {
                Debug.LogError("Please enter your email address in the wikiDataAPI script, that should be attached to the same GameObject as the OpenAIController.");
                return "";
            }
            string apiUrl = $"https://query.wikidata.org/bigdata/namespace/wdq/sparql?query={Uri.EscapeDataString(sparqlQuery)}";
            Debug.Log(apiUrl);
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Virtual_Agents_Framework", "1.3"));
                client.DefaultRequestHeaders.Add("From", email);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/csv"));

                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        Debug.Log($"Error Status Code: {response.StatusCode}\n Reason: {response.ReasonPhrase}\n Content: {errorContent}");
                        return "";
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log($"An error occurred: {ex.Message}");
                    return "";
                }
            }
        }
        public bool IsReady()
        {
            if (email == "")
            {
                Debug.LogWarning("Please enter your email address in the wikiDataAPI script");
                return false;
            }
            return true;
        }
    }
}
