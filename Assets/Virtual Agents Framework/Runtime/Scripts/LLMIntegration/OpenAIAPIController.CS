

using i5.VirtualAgents.AgentTasks;
using i5.VirtualAgents.ScheduleBasedExecution;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Threading;




#if USE_OPENAI_API
using OpenAI;
#endif

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

        [Space(10)]

        [Tooltip("How the chat agent should behave and what it has know.")]
        public string systemMessage = "You are an honorable, friendly guide of the RWTH Aachen University.You will teach students about the processes of the RWTH if they ask.";
        [Tooltip("How the chat bot should start the conservation.")]
        public string startString = "Hello! I am here to help you with any questions about the RWTH Aachen.";
        [Tooltip("Chat bot is told to keep responeses short and simple for cost saving.")]
        public bool costSaving = true;

        [Tooltip("Chat bot is told to generate a query that retrives relevant infromation.")]
        public bool useQuery = true;
        public IQueryAPI queryAPI;

        [Tooltip("Chat bot is told to animate when appropriate")]
        public bool doAnimation = true;
        [Tooltip("The agent and its taskSystem that the animations are called on. Is automatically set to the agent on the game object if no agent is provided.")]
        public Agent agent = null;
        protected ScheduleBasedTaskSystem taskSystem;
        [Space(10)]
        [Tooltip("Response will always be the dummyMessage instead of actually contacting the OpenAPI")]
        public bool useDummyMessage = true;
        public string dummyMessage = "The RWTH Aachen has approximately 45,000 students. doWave() \r\n\r\n SPARQL-Query:\r\n SELECT ?studentCount WHERE {\r\n  wd:Q273263 wdt:P2196 ?studentCount .\r\n}";
        [Tooltip("Model of the ChatGPT AI that is uses, see OpenAI documentation for model names")]
        public string model = "gpt-3.5-turbo-1106";
        [Tooltip("If true the response will be streamed, one word after another.")]
        public bool streamResponse = false;
        private CancellationTokenSource token = new CancellationTokenSource();
#if !USE_OPENAI_API
        private void Start()
        {
            Debug.LogError("OpenAIController: OpenAI API is not installed. Please press Virtual Agents Framework > Open AI > Add unofficial OpenAI helper package.");
        }
#endif

#if USE_OPENAI_API
        private OpenAIApi api;
        private readonly List<ChatMessage> messages = new();

        // Start is called before the first frame update
        void Start()
        {

            string APIKey = ReadOutAPIKey();

            if (APIKey == "" || APIKey == null)
            {
                Debug.Log("No API Key found in environment variable. Will try to read from JSON in user directory, see documentation.");
                api = new OpenAIApi();
            }
            else
            {
                Debug.Log("OpenAI API Key found in environment variable.");
                api = new OpenAIApi(APIKey);
            }

            if (!streamResponse)
            {
                okButton.onClick.AddListener(() => GetResponse());
            }
            else
            {
                okButton.onClick.AddListener(() => GetResponseStream());
            }


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

                if (!TryGetComponent<IQueryAPI>(out queryAPI))
                {
                    Debug.LogWarning("OpenAIController: No IQueryAPI component found on the game object. Queries will be disabled.");
                    useQuery = false;
                }
                else
                {
                    if (!queryAPI.IsReady())
                    {
                        Debug.LogWarning("OpenAIController: IQueryAPI component found on the game object, but is not ready to be used. Queries will be disabled.");
                        useQuery = false;
                    }
                }
            }

            if (streamResponse)
            {
                if (useQuery)
                {
                    Debug.LogWarning("OpenAIController: Stream response is not supported with queries. Will use normal response.");
                    streamResponse = false;
                }
                if (doAnimation)
                {
                    Debug.LogWarning("OpenAIController: Stream response is not supported with animations. Will use normal response.");
                    streamResponse = false;
                }
            }

            StartConversation();


        }
        /// <summary>
        /// Reads out the OpenAI API Key from the windows environment varaiable
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
                Debug.Log("An error occurred while trying to read OPENAI_API_KEY. This could be expected on some operating systems. Will try to read from JSON file now, see documentation." + ex.Message);
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
                systemMessage += " For each prompt answer the prompt as the guide and then add a " + queryAPI.QueryTypeName + " query that could retrieve information from " + queryAPI.ServiceName + " which would be beneficial to your answear. " + queryAPI.ExampleData + " Its very important that you write \"" + queryAPI.QueryTypeName + "-Query: \" in the row before each querie.";
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
                    Model = model,
                    Temperature = 0.5f,
                    MaxTokens = 100,
                    Messages = messages
                });
                // Get the response message

                responseMessage.Role = chatResult.Choices[0].Message.Role;
                responseMessage.Content = chatResult.Choices[0].Message.Content;
                Debug.Log("Tokens used " + chatResult.Usage.CompletionTokens);
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
                string queryResult = await queryAPI.SendQueryToAPI(query);
                //If request failed skip sending another message and just present normal answer
                if (queryResult != "")
                {
                    ChatMessage queryResultMessage = new()
                    {
                        Role = "system",
                        Content = "The quere results is:\n" + queryResult + " Incorporate this new information in your answear. The last message has not been sent by the user so don't refere to it again."
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
                            Model = model,
                            Temperature = 0.9f,
                            MaxTokens = 100,
                            Messages = messages
                        });
                        // Get the response message

                        responseMessage.Role = chatResult.Choices[0].Message.Role;
                        responseMessage.Content = chatResult.Choices[0].Message.Content;
                        Debug.Log("Tokens used " + chatResult.Usage.CompletionTokens);
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
            string startingString = queryAPI.QueryTypeName + "-Query:";
            // Get the query from the message.
            int splitIndex = message.IndexOf(startingString);
            if (splitIndex < 0)
            {
                Debug.Log("No query found.");
                return new string[] { "", message };
            }
            string text = message.Substring(0, splitIndex).Replace(queryAPI.QueryTypeName + "-Query:", "").Replace("SPARQL:", "").Replace("`", "").Replace("Query", "");
            string query = message.Substring(splitIndex + startingString.Length);

            // Remove any whitespace from the query.
            query = query.Trim().Replace("\n", null).Replace("\r", null).Replace("```", "").Replace("´´´", ""); ;
            Debug.Log("Extracted Query: " + query);


            // Return the SPARKL query.
            return new string[] { query, text };
        }

        private void GetResponseStream()
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
                api.CreateChatCompletionAsync(new CreateChatCompletionRequest()
                {
                    Model = model,
                    Temperature = 0.5f,
                    MaxTokens = 100,
                    Messages = messages,
                    Stream = true
                }, HandleResponse, CompletionDone, token);

            }
            else
            {
                //Insert dummy message
                responseMessage.Role = "assistant";
                responseMessage.Content = dummyMessage;

                //Split message into acutal message and included query
                string[] splittedMessage = ExtractQueryFromMessage(responseMessage.Content);
                responseMessage.Content = splittedMessage[1];
                // Update the text field with the response
                textField.text = responseMessage.Content;

                Debug.Log(string.Format("{0}: {1}", responseMessage.Role, responseMessage.Content));

                // Add the response to the list of messages
                messages.Add(responseMessage);

                // Re-enable the OK button
                okButton.enabled = true;
            }


        }
        private void HandleResponse(List<CreateChatCompletionResponse> responses)
        {
            Debug.Log(string.Join("", responses.Select(r => r.Choices[0].Delta.Content)));
            textField.text = string.Join("", responses.Select(r => r.Choices[0].Delta.Content));

        }
        private void CompletionDone()
        {
            ChatMessage responseMessage = new();
            responseMessage.Role = "assistant";
            responseMessage.Content = textField.text;
            Debug.Log(string.Format("{0}: {1}", responseMessage.Role, responseMessage.Content));

            // Add the response to the list of messages
            messages.Add(responseMessage);

            // Re-enable the OK button
            okButton.enabled = true;
        }
        private void OnDestroy()
        {
            token.Cancel();
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
#endif
    }
}