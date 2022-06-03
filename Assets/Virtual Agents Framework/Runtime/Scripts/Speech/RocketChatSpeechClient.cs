using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.Toolkit.Core.RocketChatClient;
using System.Threading.Tasks;
using TMPro;
using System;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Linq;

namespace i5.VirtualAgents.Speech
{
    
    /// <summary>
    /// Using SpeechProvider and RocketChatClient to enable the communicating between RcketChat and the Virtual Agent Framework.
    /// </summary>
    public class RocketChatSpeechClient : MonoBehaviour {
        [SerializeField] private GameObject uiPanel;
        [SerializeField] private GameObject channelGroupButton;

        //UI Panels
        private GameObject loginPanel;
        private GameObject methodPanel;
        private GameObject profilePanel;
        private GameObject postMessagePanel;
        private TMP_InputField messageField;
        private GameObject subscribePanel;
        private GameObject channelGroupPanel;
        private GameObject postAndSubscribePanel;
        private GameObject postMessageToChannelGroupPanel;

        private RocketChatService client;
        private SpeechProvider agentSpeechProvider;
        private List<ChannelGroup> channelGroups;
        private List<GameObject> channelGroupButtons;

        private string messageToWrite;
        //We create a delegate here because Unity API cannot be called outside of the main thread.
        private Action<TMP_InputField> writeMessageAction;
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
            loginPanel = uiPanel.transform.Find("Login Panel").gameObject;
            methodPanel = uiPanel.transform.Find("Method Panel").gameObject;
            profilePanel = uiPanel.transform.Find("Profile Panel").gameObject;
            channelGroupPanel = uiPanel.transform.Find("ChannelGroup Panel").gameObject;
            postMessagePanel = uiPanel.transform.Find("PostMessage Panel").gameObject;
            subscribePanel = uiPanel.transform.Find("Subscribe Panel").gameObject;
            agentSpeechProvider = GetComponent<SpeechProvider>();
            postAndSubscribePanel = uiPanel.transform.Find("Post And Subscribe Panel").gameObject;
            postMessageToChannelGroupPanel = uiPanel.transform.Find("Post Message To Channel Group Panel").gameObject;
        }

        // Update is called once per frame
        void Update() {
            //Write the message to the filed.
            if (writeMessageAction != null) {
                writeMessageAction(messageField);
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
                SpeakNoticeAsync("Login Succeeded");
                loginPanel.SetActive(false);
                methodPanel.SetActive(true);
            }
            else {
                SpeakNoticeAsync("Login Failed");
            }
        }

        public void OnPostMessageSelected() {
            messageField = postMessagePanel.transform.Find("MessageField/unity-text-input/MRKeyboardInputField_TMP(Clone)").gameObject.GetComponent<TMP_InputField>();
            methodPanel.SetActive(false);
            postMessagePanel.SetActive(true);
        }

        public void OnSubscribeMessageSelected() {
            methodPanel.SetActive(false);
            subscribePanel.SetActive(true);
        }

        public void OnShowMethodsSelected() {
            for(int i = 0; i < uiPanel.transform.childCount; i++) {
                uiPanel.transform.GetChild(i).gameObject.SetActive(false);
            }
            uiPanel.transform.Find("Method Panel").gameObject.SetActive(true);
        }

        public async void PostMessageAsync() {

            string target = postMessagePanel.transform.Find("RoomField/unity-text-input/MRKeyboardInputField_TMP(Clone)").gameObject.GetComponent<TMP_InputField>().text;
            string message = postMessagePanel.transform.Find("MessageField/unity-text-input/MRKeyboardInputField_TMP(Clone)").gameObject.GetComponent<TMP_InputField>().text;
            var response = await client.PostMessageAsync(target, message);
            if (response.Successful) {
                SpeakNoticeAsync("Post Message Succeeded.");
                postMessagePanel.transform.Find("MessageField/unity-text-input/MRKeyboardInputField_TMP(Clone)").gameObject.GetComponent<TMP_InputField>().text = "";
            }
            else {
                SpeakNoticeAsync($"Post Message Failed. {response.ErrorMessage}");
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
                    SpeakNoticeAsync($"Cannot recognize. {result.Message}");
                    break;
                case ResultState.Failed:
                    SpeakNoticeAsync($"Recognition failed. {result.Message}");
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
            SpeakNoticeAsync("Room Subscribed");
            client.OnMessageReceived += SpeakMessageAsync;
        }

        public async void UnsubscribeRoomAsync() {
            await client.UnsubscribeRoomMessageAsync("1");
            SpeakNoticeAsync("Room Unsubscribed");
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

        public async void GetJoinedChannelAndGroupAsync() {
            //First time call this method
            if (channelGroups == null) {
                var channelResponse = await client.GetChannelListJoinedAsync();
                var groupResponse = await client.GetGroupListAsync();
                if (channelResponse.Successful && groupResponse.Successful) {
                    channelGroups = new List<ChannelGroup>(Enumerable.Concat(channelResponse.Content, groupResponse.Content));                  
                    channelGroupButtons = new List<GameObject>();
                    foreach(ChannelGroup channelGroup in channelGroups) {
                        GameObject button = Instantiate(channelGroupButton);
                        button.transform.SetParent(channelGroupPanel.transform.Find("Container/GridObjectCollection"), false);
                        button.name = channelGroup.name;
                        button.GetComponent<ButtonConfigHelper>().MainLabelText = channelGroup.name;
                        button.GetComponent<ButtonConfigHelper>().OnClick.AddListener(() => OpenPostAndSubscribePanel(channelGroup));
                    }
                    channelGroupPanel.transform.Find("Container/GridObjectCollection").GetComponent<GridObjectCollection>().UpdateCollection();
                    methodPanel.SetActive(false);
                    channelGroupPanel.SetActive(true);
                    SpeakNoticeAsync("Get joined channel and group succeeded");
                }
                else {
                    SpeakNoticeAsync($"Get joined channel and group failed, {channelResponse.ErrorMessage}");
                }
            }
            else {
                methodPanel.SetActive(false);
                channelGroupPanel.SetActive(true);
            }

        }
        #region Private Methods

        //Display a message string on the UI.
        private async void SpeakNoticeAsync(string message) {
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
        private void WriteStringToMessageField(TMP_InputField messageField) {
            messageField.text = messageToWrite;
        }

        private async void SpeakMessageAsync(MessageFieldsArguments args) {
            await agentSpeechProvider.StartSynthesizingAndSpeakingAsync($"{args.Sender.name} said: {args.MessageContent}");
        }
        #endregion

        private void OpenPostAndSubscribePanel(ChannelGroup channelGroup) {
            postAndSubscribePanel.SetActive(true);
            channelGroupPanel.SetActive(false);
            postAndSubscribePanel.transform.Find("ChannelGroupLabel/Text").GetComponent<TextMeshPro>().text = channelGroup.name;
            postAndSubscribePanel.transform.Find("PostMessageButton").GetComponent<ButtonConfigHelper>().OnClick.AddListener(() => OpenPostMessageToChannelGroupPanel(channelGroup));
            postAndSubscribePanel.transform.Find("SubscribeButton").GetComponent<ButtonConfigHelper>().OnClick.AddListener(() => SubscribeChannelGroupAysnc(channelGroup));
        }

        private void OpenPostMessageToChannelGroupPanel(ChannelGroup channelGroup) {
            messageField = postMessageToChannelGroupPanel.transform.Find("MessageField/unity-text-input/MRKeyboardInputField_TMP(Clone)").gameObject.GetComponent<TMP_InputField>();
            postMessageToChannelGroupPanel.SetActive(true);
            postAndSubscribePanel.SetActive(false);
            postMessageToChannelGroupPanel.transform.Find("ChannelGroupLabel/Text").GetComponent<TextMeshPro>().text = channelGroup.name;
            postMessageToChannelGroupPanel.transform.Find("PostMessageButton").GetComponent<ButtonConfigHelper>().OnClick.AddListener(() => PostMessageToChannelGroupAsync(channelGroup));
        }

        private async void SubscribeChannelGroupAysnc(ChannelGroup channelGroup) {
            string roomID = channelGroup._id;
            await client.SubscribeRoomMessageAsync(roomID, "1");
            SpeakNoticeAsync($"Room {channelGroup.name} Subscribed");
            client.OnMessageReceived += SpeakMessageAsync;
        }

        private async void PostMessageToChannelGroupAsync(ChannelGroup channelGroup) {
            string target = channelGroup._id;
            string message = postMessageToChannelGroupPanel.transform.Find("MessageField/unity-text-input/MRKeyboardInputField_TMP(Clone)").gameObject.GetComponent<TMP_InputField>().text;
            var response = await client.PostMessageAsync(target, message);
            if (response.Successful) {
                SpeakNoticeAsync("Post Message Succeeded.");
                postMessageToChannelGroupPanel.transform.Find("MessageField/unity-text-input/MRKeyboardInputField_TMP(Clone)").gameObject.GetComponent<TMP_InputField>().text = "";
            }
            else {
                SpeakNoticeAsync($"Post Message Failed. {response.ErrorMessage}");
            }
            StopRecordingAsync();
        }
    }
}
