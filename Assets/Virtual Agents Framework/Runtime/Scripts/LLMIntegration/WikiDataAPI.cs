using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace i5.VirtualAgents
{
    
    public class WikiDataAPI :  MonoBehaviour, ISparqlAPI
    {
        private readonly string email = "";
        //https://www.mediawiki.org/wiki/Wikidata_Query_Service/User_Manual#SPARQL_endpoint
        public async Task<string> SparqlQueryToAPI(string sparqlQuery)
        {
            if(email == ""){
                Debug.LogError("Please enter your email address in the wikiDataAPI script");
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
        public bool isReady()
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
