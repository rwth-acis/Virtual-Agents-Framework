using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    /// <summary>
    /// Interface that should be used to implement more SPARQL APIs, these could be offline or online
    /// </summary>
    public interface ISparqlAPI
    {
        /// <summary>
        /// Method that should be used to send a SPARQL query to the API
        /// </summary>
        /// <param name="sparqlQuery">SPARQL query to send</param>
        /// <returns>String containing the result of the SPARQL query in a csv like format</returns>
        Task<string> SparqlQueryToAPI(string sparqlQuery);
        
        /// <summary>
        /// Method that should be used to check if all necessary data (API key etc) is entered and the API is ready to be used 
        /// </summary>
        /// <returns>True if the API is ready, false otherwise</returns>
        public bool isReady();
    }
}
