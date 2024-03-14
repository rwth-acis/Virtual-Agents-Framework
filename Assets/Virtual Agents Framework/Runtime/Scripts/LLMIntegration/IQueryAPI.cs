using System.Threading.Tasks;

namespace i5.VirtualAgents.LLM
{
    /// <summary>
    /// Interface that should be used to implement more query APIs, these could be offline or online
    /// </summary>
    public interface IQueryAPI
    {
        /// <summary>
        /// Query type name that will be provided to the LLM to build the a query with the correct syntax.
        /// </summary>
        string QueryTypeName { get; set; }
        /// <summary>
        /// Service name that will be provided to the LLM so that it could use datapoints known by the LLM.
        /// </summary>
        string ServiceName { get; set; }
        /// <summary>
        /// This example data can provided further examples for the LLM of how a query should look like or include additional ids of datapoints.
        /// </summary>
        string ExampleData { get; set; }

        /// <summary>
        /// Method that should be used to send a query to the API
        /// </summary>
        /// <param name="sparqlQuery">query to send</param>
        /// <returns>String containing the result of the query in a csv like format</returns>
        Task<string> SendQueryToAPI(string query);

        /// <summary>
        /// Method that should be used to check if all necessary data (API key etc) is entered and the API is ready to be used 
        /// </summary>
        /// <returns>True if the API is ready, false otherwise</returns>
        public bool IsReady();
    }
}
