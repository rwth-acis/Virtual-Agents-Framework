using i5.VirtualAgents.AgentTasks;
using i5.VirtualAgents.ScheduleBasedExecution;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using OpenAI;


namespace i5.VirtualAgents
{
    
    /// <summary>
    /// This script should be attached to an agent or any controller object to integrate chat GPT. If attached to an agent the chat bot can command animations.
    /// Uses Text Field, Imput Field and Ok Button for Text message interaction.
    /// </summary>
    public class OpenAIController : MonoBehaviour
    {
        public TMP_Text textField;
        public TMP_InputField inputField;
        public Button okButton;
        
        [Tooltip("How the chat agent should behave and what it has know.")]
        public string systemMessage;
        [Tooltip("How the chat bot should start the conservation.")]
        public string startString;
        [Tooltip("Chat bot is told to keep responeses short and simple for cost saving.")]
        public bool costSaving = true;

        [Tooltip("Response will always be the dummyMessage instead of actually contacting the API")]
        public bool useDummyMessage = true;
        public string dummyMessage = "The RWTH Aachen has approximately 45,000 students. doWave() \r\n\r\nSELECT ?studentCount WHERE {\r\n  wd:Q273263 wdt:P2191 ?studentCount .\r\n}";

        [Tooltip("Chat bot is told to generate a SPARQL query")]
        public bool useQuery = true;
        public ISparqlAPI sparqlAPI;

        [Tooltip("Chat bot is told to animate when appropriate")]

        public bool doAnimation = true;
        [Tooltip("The agent and its taskSystem that the animations are called on. Is automatically set to the agent on the game object if no agent is provided.")]
        public Agent agent = null;
        protected ScheduleBasedTaskSystem taskSystem;


        private OpenAIApi api;
        private readonly List<ChatMessage> messages = new();

        // Start is called before the first frame update
        void Start()
        {

            string APIKey = ReadOutAPIKey();
            api = new OpenAIApi(APIKey);
            StartConversation();
            okButton.onClick.AddListener(() => GetResponse());

            //If animations are used, check if responding components are existing
            if (doAnimation)
            {
                //If no agent is provided, try to get it from the game object
                if (agent == null)
                {
                    agent = GetComponent<Agent>();
                }
                //If no agent is provided, try to get it from the game object
                if (agent == null)
                {
                    Debug.LogWarning("OpenAIController: No agent component found on the game object. Animations will be disabled.");
                    doAnimation = false;
                }
                else
                {
                    taskSystem = (ScheduleBasedTaskSystem)agent.TaskSystem;
                }

            }
            //If queries are used, check if responding components are existing and ready to be used
            if (useQuery)
            {

                if (!TryGetComponent<ISparqlAPI>(out sparqlAPI))
                {
                    Debug.LogWarning("OpenAIController: No ISparqlAPI component found on the game object. Queries will be disabled.");
                    useQuery = false;
                }
                else
                {
                    if (!sparqlAPI.isReady())
                    {
                        Debug.LogWarning("OpenAIController: ISparqlAPI component found on the game object, but is not ready to be used. Queries will be disabled.");
                        useQuery = false;
                    }
                }
            }


        }
        /// <summary>
        /// Reads out the OpenAI API Key from the windows environment varaiable or a json file
        /// </summary>
        /// <returns>The API Key for OpenAI</returns>
        private string ReadOutAPIKey()
        {
            string apikey = "";
            // Gets API key from Windows environment varaiable if possible
            try
            {
                apikey = Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.User);
            }
            catch (Exception ex)
            {
                Debug.Log("An error occurred while trying to read OPENAI_API_KEY. This could be expected on some operating systems. Will try to read from JSON file now. " + ex.Message);
            }

            if (apikey == null || apikey == "")
            {
                try
                {
                    // Read the JSON file
                    string jsonFilePath = "Assets\\Virtual Agents Framework\\Runtime\\Scripts\\GptIntegration\\apikeys.json";
                    string jsonData = File.ReadAllText(jsonFilePath);
                    // Deserialize JSON data
                    JObject jsonObject = JObject.Parse(jsonData);
                    // Get the API key
                    apikey = (string)jsonObject["OpenAIKey"];
                }
                catch (Exception ex)
                {
                    Debug.LogError("An error occurred while trying to read json file: " + ex.Message);
                }
            }
            return apikey;

        }
        private void StartConversation()
        {
            if (costSaving)
            {
                systemMessage += " You keep your responses short and to the point.";
            }
            if (doAnimation)
            {
                systemMessage += "In reality your are an NPC charakter that needs to emote for every message. You can do two animations a headshake and a wave with your hand. Write doHeadshake() or doWave() in the message to do these animations. An example would be: \" Hello! doWave()\" or \" This is wrong. doHeadshake()\"";
            }
            if (useQuery)
            {
                systemMessage += " For each prompt answer the prompt as the guide and then add a SPARQL query that could retrieve information from Wikidata which would be beneficial to your answear. The RWTH Aachen is Q273263 in wikidata.";
            }

            Debug.Log("SystemMessage: " + systemMessage);
            var newMessage = new ChatMessage()
            {
                Role = "system",
                Content = systemMessage
            };

            messages.Add(newMessage);

            inputField.text = "";
            textField.text = startString;
            Debug.Log(startString);
        }

        private async void GetResponse()
        {
            //Check if there was an input
            if (inputField.text.Length < 1)
            {
                return;
            }

            // Disable the OK button
            okButton.enabled = false;

            // Fill the user message from the input field
            ChatMessage userMessage = new()
            {
                Role = "user",
                Content = inputField.text
            };
            if (userMessage.Content.Length > 100)
            {
                // Limit messages to 100 characters
                userMessage.Content = userMessage.Content[..100];
            }
            Debug.Log(string.Format("{0}: {1}", userMessage.Role, userMessage.Content));

            // Add the message to the list
            messages.Add(userMessage);

            // Update the text field with the user message
            textField.text = string.Format("You: {0}", userMessage.Content);

            // Clear the input field
            inputField.text = "";


            ChatMessage responseMessage = new();

            if (useDummyMessage == false)
            {
                // Send the entire chat to OpenAI to get the next message
                var chatResult = await api.CreateChatCompletion(new CreateChatCompletionRequest()
                {
                    Model = "gpt-3.5-turbo-1106",
                    Temperature = 0.5f,
                    MaxTokens = 100,
                    Messages = messages
                });
                // Get the response message

                responseMessage.Role = chatResult.Choices[0].Message.Role;
                responseMessage.Content = chatResult.Choices[0].Message.Content;
                Debug.Log("Tokens used" + chatResult.Usage.CompletionTokens);
            }
            else
            {
                //Insert dummy message
                responseMessage.Role = "assistant";
                responseMessage.Content = dummyMessage;
            }

            Debug.Log(string.Format("{0}: {1}", responseMessage.Role, responseMessage.Content));

            //Split message into acutal message and included query
            string[] splittedMessage = ExtractQueryFromMessage(responseMessage.Content);
            string query = splittedMessage[0];
            responseMessage.Content = splittedMessage[1];

            // Add the response to the list of messages
            messages.Add(responseMessage);

            //If there is a quere, process that query and ask the chat bot to incorporate the new information.
            if (query != null & query != "")
            {
                string queryResult = await sparqlAPI.SparqlQueryToAPI(query);
                //If request failed skip sending another message and just present normal answer
                if (queryResult != "")
                {
                    ChatMessage queryResultMessage = new()
                    {
                        Role = "system",
                        Content = "The quere results is:\n" + queryResult + "Incorporate this new information in your answear. And don't include the querie again."
                    };
                    if (queryResultMessage.Content.Length > 200)
                    {
                        // Limit messages to 100 characters
                        queryResultMessage.Content = queryResultMessage.Content[..100];
                    }
                    Debug.Log(string.Format("{0}: {1}", queryResultMessage.Role, queryResultMessage.Content));

                    // Add the message to the list
                    messages.Add(queryResultMessage);

                    if (useDummyMessage == false)
                    {
                        // Send the entire chat to OpenAI to get the next message
                        var chatResult = await api.CreateChatCompletion(new CreateChatCompletionRequest()
                        {
                            Model = "gpt-3.5-turbo-1106",
                            Temperature = 0.9f,
                            MaxTokens = 100,
                            Messages = messages
                        });
                        // Get the response message

                        responseMessage.Role = chatResult.Choices[0].Message.Role;
                        responseMessage.Content = chatResult.Choices[0].Message.Content;
                        Debug.Log("Tokens used" + chatResult.Usage.CompletionTokens);
                    }
                    else
                    {
                        //Insert dummy message
                        responseMessage.Role = "assistant";
                        responseMessage.Content = dummyMessage + queryResult;
                    }
                    Debug.Log(string.Format("{0}: {1}", responseMessage.Role, responseMessage.Content));

                    responseMessage.Content = ExtractQueryFromMessage(responseMessage.Content)[1];
                    messages.Add(responseMessage);
                } 
            }
            //Check for animation calls, play them and remove them from the message
            responseMessage.Content = CheckMessageForAnimationPromptsAndCallThem(responseMessage.Content);
            // Update the text field with the response
            textField.text = responseMessage.Content;

            // Re-enable the OK button
            okButton.enabled = true;
        }
        private string[] ExtractQueryFromMessage(string message)
        {
            Debug.Log("Extracting from: " + message);
            // Get the SPARKL query from the message.
            int splitIndex = message.IndexOf("SELECT");
            if (splitIndex < 0)
            {
                return new string[] { "", message };
            }
            string text = message.Substring(0, splitIndex);
            string sparqlQuery = message.Substring(splitIndex);

            // Remove any whitespace from the SPARKL query.
            sparqlQuery = sparqlQuery.Trim().Replace("\n", " ").Replace("\r", " ").Replace("`", "");
            Debug.Log("Extracted sparqlQuery: " + sparqlQuery);


            // Return the SPARKL query.
            return new string[] { sparqlQuery, text };
        }

        private string CheckMessageForAnimationPromptsAndCallThem(string message)
        {
            string[] animations = { "doHeadshake()", "doWave()" };
            foreach (string animation in animations)
            {
                if (message.Contains(animation))
                {
                    // Remove the method from the string
                    int index = message.IndexOf(animation);
                    message = message.Remove(index, animation.Length);

                    // Call the method
                    if (animation == "doHeadshake()")
                    {
                        AgentBaseTask headShake = taskSystem.Tasks.PlayAnimation("ShakeHead", 5, "", 0, "Head");
                    }
                    else if (animation == "doWave()")
                    {
                        AgentBaseTask wave1 = taskSystem.Tasks.PlayAnimation("Wave", 5, "", 0, "Left Arm");
                    }
                }
            }
            return message;
        }
    }

}