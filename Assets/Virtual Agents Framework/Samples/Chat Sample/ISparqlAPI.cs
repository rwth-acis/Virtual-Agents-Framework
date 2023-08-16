using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public interface ISparqlAPI
    {
        Task<string> SparqlQueryToAPI(string sparqlQuery);
    }
}
