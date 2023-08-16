using Codice.ThemeImages;
using i5.VirtualAgents.AgentTasks;
using i5.VirtualAgents.ScheduleBasedExecution;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using TMPro;
using UnityEditor.Graphs;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using static Codice.Client.Common.WebApi.WebApiEndpoints;
using static System.Net.Mime.MediaTypeNames;

namespace i5.VirtualAgents.Examples
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
        //How the chat agent should behave and what it has know.
        public string systemMessage;
        //How the chat bot should behave.
        public string startString;
        //Chat bot is told to keep responeses short and simple for cost saving.
        public bool costSaving = true;

        //Response will always be the dummyMessage instead of actually contacting the API
        public bool useDummyMessage = true;
        public string dummyMessage = "The RWTH Aachen has approximately 45,000 students. doWave() \r\n\r\nSELECT ?studentCount WHERE {\r\n  wd:Q273263 wdt:P2191 ?studentCount .\r\n}";

        //Chat bot is told to generate a SPARQL query
        public bool useQuery = true;
        public ISparqlAPI sparqlAPI;

        //Chat bot is told to animate when appropriate
        public bool doAnimation = true;
        //The agent and its taskSystem that the animations are called on. Is automatically set to the agent on the game object if no agent is provided.
        public Agent agent = null;
        protected ScheduleBasedTaskSystem taskSystem;


        private OpenAIAPI api;
        private List<ChatMessage> messages;

        // Start is called before the first frame update
        void Start()
        {
            // This line gets your API key (and could be slightly different on Mac/Linux)
            api = new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.User));
            StartConversation();
            okButton.onClick.AddListener(() => GetResponse());

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

            if (useQuery)
            {
                sparqlAPI = GetComponent<ISparqlAPI>();
                if (sparqlAPI == null)
                {
                    Debug.LogWarning("OpenAIController: No ISparqlAPI component found on the game object. Queries will be disabled.");
                    useQuery = false;
                }
            }


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
            messages = new List<ChatMessage> {
                new ChatMessage(ChatMessageRole.System, systemMessage)
            };

            inputField.text = "";
            textField.text = startString;
            Debug.Log(startString);
        }

        private async void GetResponse()
        {
            if (inputField.text.Length < 1)
            {
                return;
            }

            // Disable the OK button
            okButton.enabled = false;

            // Fill the user message from the input field
            ChatMessage userMessage = new ChatMessage();
            userMessage.Role = ChatMessageRole.User;
            userMessage.Content = inputField.text;
            if (userMessage.Content.Length > 100)
            {
                // Limit messages to 100 characters
                userMessage.Content = userMessage.Content.Substring(0, 100);
            }
            Debug.Log(string.Format("{0}: {1}", userMessage.Role, userMessage.Content));

            // Add the message to the list
            messages.Add(userMessage);

            // Update the text field with the user message
            textField.text = string.Format("You: {0}", userMessage.Content);

            // Clear the input field
            inputField.text = "";


            ChatMessage responseMessage = new ChatMessage();

            if (useDummyMessage == false)
            {
                // Send the entire chat to OpenAI to get the next message
                var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
                {
                    Model = Model.ChatGPTTurbo,
                    Temperature = 0.5,
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
                responseMessage.Role = ChatMessageRole.Assistant;
                responseMessage.Content = dummyMessage;
            }

            Debug.Log(string.Format("{0}: {1}", responseMessage.Role, responseMessage.Content));

            // Add the response to the list of messages
            string[] splittedMessage = extractQueryFromMessage(responseMessage.Content);
            string query = splittedMessage[0];
            responseMessage.Content = splittedMessage[1];

            messages.Add(responseMessage);

            string queryResult = "";
            if (query != null & query != "")
            {
                queryResult = await sparqlAPI.SparqlQueryToAPI(query);

                // Fill the user message from the input field
                ChatMessage queryResultMessage = new ChatMessage();
                userMessage.Role = ChatMessageRole.User;
                userMessage.Content = "The quere results is:\n" + queryResult + "Incorporate this new information in your answear. And don't include the querie again.";
                if (userMessage.Content.Length > 200)
                {
                    // Limit messages to 100 characters
                    userMessage.Content = userMessage.Content.Substring(0, 100);
                }
                Debug.Log(string.Format("{0}: {1}", userMessage.Role, userMessage.Content));

                // Add the message to the list
                messages.Add(userMessage);

                if (useDummyMessage == false)
                {
                    // Send the entire chat to OpenAI to get the next message
                    var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
                    {
                        Model = Model.ChatGPTTurbo,
                        Temperature = 0.9,
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
                    responseMessage.Role = ChatMessageRole.Assistant;
                    responseMessage.Content = dummyMessage + queryResult;
                }
                Debug.Log(string.Format("{0}: {1}", responseMessage.Role, responseMessage.Content));

                responseMessage.Content = extractQueryFromMessage(responseMessage.Content)[1];
                messages.Add(responseMessage);
            }
            //Check for animation calls, play them and remove them
            responseMessage.Content = checkMessageForAnimationPromptsAndCallThem(responseMessage.Content);
            // Update the text field with the response
            textField.text = responseMessage.Content;

            // Re-enable the OK button
            okButton.enabled = true;
        }
        private string[] extractQueryFromMessage(string message)
        {
            Debug.Log(message);
            // Get the SPARKL query from the message.
            int splitIndex = message.IndexOf("SELECT");
            if (splitIndex < 0)
            {
                return new string[] { "", message };
            }
            string text = message.Substring(0, splitIndex);
            string sparqlQuery = message.Substring(splitIndex);

            // Remove any whitespace from the SPARKL query.
            sparqlQuery = sparqlQuery.Trim().Replace("\n", "").Replace("\r", "");
            Debug.Log(sparqlQuery);


            // Return the SPARKL query.
            return new string[] { sparqlQuery, text };
        }

        private string checkMessageForAnimationPromptsAndCallThem(string message)
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
                        AgentBaseTask wave1 = taskSystem.Tasks.PlayAnimation("Wave", 5, "", 0, "Left Arm");
                    }
                    else if (animation == "doWave()")
                    {
                        AgentBaseTask headShake = taskSystem.Tasks.PlayAnimation("ShakeHead", 5, "", 0, "Head");
                    }
                }
            }
            return message;
        }
    }
}
