using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.Toolkit.Core.RocketChatClient;
using System.Threading.Tasks;
using TMPro;
using System;

namespace i5.VirtualAgents.Speech
{
    
    /// <summary>
    /// Using SpeechProvider and RocketChatClient to enable the communicating between RcketChat and the Virtual Agent Framework.
    /// </summary>
    public class RocketChatSpeechClient : MonoBehaviour {
        [SerializeField] private GameObject UIPanel;

        //UI Panels
        private GameObject loginPanel;
        private GameObject methodPanel;
        private GameObject profilePanel;
        private GameObject postMessagePanel;
        private TMP_InputField messageField;
        private GameObject subscribePanel;

        private RocketChatService client;
        private SpeechProvider agentSpeechProvider;

        private string messageToWrite;
        //We create a delegate here because Unity API cannot be called outside of the main thread.
        private Action writeMessageAction;
        public string HostAddress
        {
            get => loginPanel.transform.Find("HostAddress/unity-text-input/MRKeyboardInputField_TMP(Clone)").gameObject.GetComponent<TMP_InputField>().text;
            set => loginPanel.transform.Find("HostAddress/unity-text-input/MRKeyboardInputField_TMP(Clone)").gameObject.GetComponent<TMP_InputField>().text = value;
        }
        public string Username
        {
            get => loginPanel.transform.Find("Username/unity-text-input/MRKeyboardInputField_TMP(Clone)").gameObject.GetComponent<TMP_InputField>().text;
            set => loginPanel.transform.Find("Username/unity-text-input/MRKeyboardInputField_TMP(Clone)").gameObject.GetComponent<TMP_InputField>().text = value;
        }
        public string Password
        {
            get => loginPanel.transform.Find("Password/unity-text-input/MRKeyboardInputField_TMP(Clone)").gameObject.GetComponent<TMP_InputField>().text;
            set => loginPanel.transform.Find("Password/unity-text-input/MRKeyboardInputField_TMP(Clone)").gameObject.GetComponent<TMP_InputField>().text = value;
        }

        private void Awake() {
            loginPanel = UIPanel.transform.Find("Login Panel").gameObject;
            methodPanel = UIPanel.transform.Find("Method Panel").gameObject;
            profilePanel = UIPanel.transform.Find("Profile Panel").gameObject;
            postMessagePanel = UIPanel.transform.Find("PostMessage Panel").gameObject;
            messageField = postMessagePanel.transform.Find("MessageField/unity-text-input/MRKeyboardInputField_TMP(Clone)").gameObject.GetComponent<TMP_InputField>();
            subscribePanel = UIPanel.transform.Find("Subscribe Panel").gameObject;
            agentSpeechProvider = GetComponent<SpeechProvider>();
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update() {
            //Write the message to the filed.
            if (writeMessageAction != null) {
                writeMessageAction();
                writeMessageAction = null;
            }
        }
        
        /// <summary>
        /// Login to the host server.
        /// </summary>
        public async void LoginAsync() {
            client = new RocketChatService(HostAddress);
            bool loginSucceeded = await client.LoginAsync(Username, Password);
            if (loginSucceeded) {
                SpeakMessageAsync("Login Succeeded");
                loginPanel.SetActive(false);
                methodPanel.SetActive(true);
            }
            else {
                SpeakMessageAsync("Login Failed");
            }
        }

        public void OnPostMessageSelected() {
            methodPanel.SetActive(false);
            postMessagePanel.SetActive(true);
        }

        public void OnSubscribeMessageSelected() {
            methodPanel.SetActive(false);
            subscribePanel.SetActive(true);
        }

        public void OnShowMethodsSelected() {
            for(int i = 0; i < UIPanel.transform.childCount; i++) {
                UIPanel.transform.GetChild(i).gameObject.SetActive(false);
            }
            UIPanel.transform.Find("Method Panel").gameObject.SetActive(true);
        }

        public async void PostMessageAsync() {
            string target = postMessagePanel.transform.Find("RoomField/unity-text-input/MRKeyboardInputField_TMP(Clone)").gameObject.GetComponent<TMP_InputField>().text;
            string message = postMessagePanel.transform.Find("MessageField/unity-text-input/MRKeyboardInputField_TMP(Clone)").gameObject.GetComponent<TMP_InputField>().text;
            var response = await client.PostMessageAsync(target, message);
            if (response.Successful) {
                SpeakMessageAsync("Post Message Succeeded.");
                postMessagePanel.transform.Find("MessageField/unity-text-input/MRKeyboardInputField_TMP(Clone)").gameObject.GetComponent<TMP_InputField>().text = "";
            }
            else {
                SpeakMessageAsync($"Post Message Failed. {response.ErrorMessage}");
            }
            StopRecordingAsync();
        }

        public async void StartRecordingMessageAsync() {
            agentSpeechProvider.OnRecognitionResultReceived += WriteRecognitionResultToMessageField;
            postMessagePanel.transform.Find("StartRecordingButton").gameObject.SetActive(false);
            postMessagePanel.transform.Find("StopRecordingButton").gameObject.SetActive(true);
            var result = await agentSpeechProvider.StartRecordingAsync();
            switch (result.State) {
                case ResultState.Succeeded:
                    break;
                case ResultState.NoMatch:
                    SpeakMessageAsync($"Cannot recognize. {result.Message}");
                    break;
                case ResultState.Failed:
                    SpeakMessageAsync($"Recognition failed. {result.Message}");
                    break;
            }
        }

        public async void StopRecordingAsync() {
            agentSpeechProvider.OnRecognitionResultReceived -= WriteRecognitionResultToMessageField;
            messageToWrite = "";
            await agentSpeechProvider.StopRecordingAsync();
            postMessagePanel.transform.Find("StartRecordingButton").gameObject.SetActive(true);
            postMessagePanel.transform.Find("StopRecordingButton").gameObject.SetActive(false);
        }

        public async void SubscribeRoomAsync() {
            string roomID = subscribePanel.transform.Find("RoomField/unity-text-input/MRKeyboardInputField_TMP(Clone)").gameObject.GetComponent<TMP_InputField>().text;
            await client.SubscribeRoomMessageAsync(roomID, "1");
            SpeakMessageAsync("Room Subscribed");
            client.OnMessageReceived += SpeakMessageAsync;
        }

        public async void UnsubscribeRoomAsync() {
            await client.UnsubscribeRoomMessageAsync("1");
            SpeakMessageAsync("Room Unsubscribed");
            client.OnMessageReceived -= SpeakMessageAsync;
        }

        public async void GetProfileAsync() {
            profilePanel.SetActive(true);
            methodPanel.SetActive(false);
            var result = await client.GetMeAsync();
            if (result.Successful) {
                UserInfo info = result.Content;
                profilePanel.transform.Find("NameLabel/Text").gameObject.GetComponent<TextMeshPro>().text = $" Name: {info.name}";
                profilePanel.transform.Find("EmailLabel/Text").gameObject.GetComponent<TextMeshPro>().text = $" Email: {info.email}";
                profilePanel.transform.Find("IdLabel/Text").gameObject.GetComponent<TextMeshPro>().text = $" ID: {info._id}";
                string roles = "";
                for (int i = 0; i < info.roles.Length; i++) {
                    roles += info.roles[i];
                    if (i == info.roles.Length - 1) {
                        roles += ".";
                    }
                    else {
                        roles += ", ";
                    }
                }
                profilePanel.transform.Find("RoleLabel/Text").gameObject.GetComponent<TextMeshPro>().text = " Roles: " + roles;
            }
            else {
                profilePanel.transform.Find("NameLabel/Text").gameObject.GetComponent<TextMeshPro>().text = $"Get Profile Failed, {result.ErrorMessage}";
            }
        }


        #region Private Methods

        //Display a message string on the UI.
        private async void SpeakMessageAsync(string message) {
            await agentSpeechProvider.StartSynthesizingAndSpeakingAsync(message);
        }

        private void WriteRecognitionResultToMessageField(RecognitionResult result) {
            if (result.Text != null) {
                messageToWrite += result.Text;
                //We create a delegate here because Unity API cannot be called outside of the main thread.
                writeMessageAction = WriteStringToMessageField;
            }
        }

        //A function for the event.
        private void WriteStringToMessageField() {
            messageField.text = messageToWrite;
        }

        private async void SpeakMessageAsync(MessageFieldsArguments args) {
            await agentSpeechProvider.StartSynthesizingAndSpeakingAsync($"{args.Sender.name} said: {args.MessageContent}");
        }
        #endregion
    }
}
