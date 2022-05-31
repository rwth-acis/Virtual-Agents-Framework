using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;

namespace i5.VirtualAgents.Speech
{
    /// <summary>
    /// The SpeechProvider requires at least one ISpeechRecognizer and one ISpeechSynthesizer. 
    /// The ones with higher priority should be placed on top of other recognizers and synthesizers in the inspector.
    /// It manages the ISpeechRecognizer and ISpeechSynthesizer and exposes their functionalities to users.
    /// So developers only need to re-implement their own ISpeechRecognizer and ISpeechSynthesizer if needed, and don't need to care about the user-interaction aspects.
    /// There maybe also other settings (SerializeField) on the recognizer and synthesizer.
    /// </summary>
    [RequireComponent(typeof(ISpeechRecognizer))]
    [RequireComponent(typeof(ISpeechSynthesizer))]
    public class SpeechProvider : MonoBehaviour
    {
        [SerializeField] private Language language;
        [Tooltip("Set the primary audio data output form. It will be propagated to all synthesizers. But it may not have any effects due to synthesizer's implementation. " +
            "For \"AsByteStream\", the output will be converted to an audio clip and can be played by an Audio Source.")]
        [SerializeField] private AudioDataOutputForm primaryAudioOutputForm;

        private ISpeechRecognizer[] recognizers;
        private ISpeechRecognizer currentRecognizer;
        private ISpeechSynthesizer[] synthesizers;
        private ISpeechSynthesizer currentSynthesizer;

        public event ISpeechRecognizer.RecognitionResultHandler OnRecognitionResultReceived
        {
            add
            {
                foreach (var recognizer in recognizers) {
                    recognizer.OnRecognitionResultReceived += value;
                }
            }
            remove
            {
                foreach (var recognizer in recognizers) {
                    recognizer.OnRecognitionResultReceived -= value;
                }
            }
        }
        public event ISpeechSynthesizer.SynthesisResultHandler OnSynthesisResultReceived
        {
            add
            {
                foreach(var synthesizer in synthesizers) {
                    synthesizer.OnSynthesisResultReceived += value;
                }
            }
            remove
            {
                foreach(var synthesizer in synthesizers) {
                    synthesizer.OnSynthesisResultReceived -= value;
                }
            }
        }

        public Language Language
        {
            get => language;
            set
            {
                language = value;
                foreach(ISpeechRecognizer recognizer in recognizers) {
                    recognizer.Language = value;
                }
                foreach(ISpeechSynthesizer synthesizer in synthesizers) {
                    synthesizer.Language = value;
                }
            }
        }

        public AudioDataOutputForm PrimaryAudioOutputForm
        {
            get => primaryAudioOutputForm;
            set
            {
                primaryAudioOutputForm = value;
                foreach (ISpeechSynthesizer synthesizer in synthesizers) {
                    synthesizer.OutputForm = value;
                }
            }
        }

        // Start is called before the first frame update
        void Start() {
            InitializeRecognizerAndSynthesizer();
        }

        /// <summary>
        /// Start recording and recognizing the user's voice.
        /// </summary>
        public async Task<RecognitionResult> StartRecordingAsync() {
            RecognitionResult result = await currentRecognizer.StartRecordingAsync();         
            if(result.State == ResultState.Failed && !currentRecognizer.IsApplicable) {
                foreach (ISpeechRecognizer recognizer in recognizers) {
                    if(recognizer != currentRecognizer && recognizer.IsApplicable) {
                        currentRecognizer = recognizer;
                        break;
                    }
                }
            }
            return result;
        }

        public async Task StopRecordingAsync() {
            await currentRecognizer.StopRecordingAsync();
        }

        /// <summary>
        /// Synthesizing the given text and speaking the audio out.
        /// </summary>
        /// <param name="inputText"></param>
        public async Task<SynthesisResult> StartSynthesizingAndSpeakingAsync(string inputText) {
            SynthesisResult result = await currentSynthesizer.StartSynthesizingAndSpeakingAsync(inputText);
            if(result.State == ResultState.Failed && !currentSynthesizer.IsApplicable) {
                foreach (ISpeechSynthesizer synthesizer in synthesizers) {
                    if(synthesizer != currentSynthesizer && synthesizer.IsApplicable) {
                        currentSynthesizer = synthesizer;
                        break;
                    }
                }
            }
            //if the speech will not be played directly.
            if(currentSynthesizer.OutputForm == AudioDataOutputForm.AsByteStream) {
                float[] clipData = BytesToFloat(result.AudioData);
                AudioSource audioSource = gameObject.GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
                audioSource.clip = AudioClip.Create("SynthesizedSpeech", 16000 * 10, 1, 16000, false);
                audioSource.clip.SetData(clipData, 0);
                audioSource.spatialBlend = 1;
                audioSource.Play();
                
            }
            return result;
        }


        #region Private Methods

        //Initialize all recognizers and synthesizers and choose the applicables with highest priority.
        private void InitializeRecognizerAndSynthesizer() {
            recognizers = GetComponents<ISpeechRecognizer>();
            foreach (ISpeechRecognizer speechRecognizer in recognizers) {
                speechRecognizer.Language = Language;
                if (speechRecognizer.IsApplicable) {
                    currentRecognizer = speechRecognizer;
                    break;
                }
            }
            synthesizers = GetComponents<ISpeechSynthesizer>();
            foreach (ISpeechSynthesizer speechSynthesizer in synthesizers) {
                speechSynthesizer.Language = Language;
                speechSynthesizer.OutputForm = primaryAudioOutputForm;
                if (speechSynthesizer.IsApplicable) {
                    currentSynthesizer = speechSynthesizer;
                    break;
                }
            }
        }

        // convert two bytes to one float in the range -1 to 1
        private float BytesToFloat(byte firstByte, byte secondByte) {
            // convert two bytes to one short (little endian)
            short s = (short)((secondByte << 8) | firstByte);
            // convert to range from -1 to (just below) 1
            return s / 32768.0F;
        }

        private float[] BytesToFloat(byte[] byteStream) {
            float[] soundData = new float[byteStream.Length / 2];
            for (int i = 0; i < soundData.Length; i++) {
                soundData[i] = BytesToFloat(byteStream[i * 2], byteStream[i * 2 + 1]);
            }
            return soundData;
        }

        #endregion
    }
    
}
