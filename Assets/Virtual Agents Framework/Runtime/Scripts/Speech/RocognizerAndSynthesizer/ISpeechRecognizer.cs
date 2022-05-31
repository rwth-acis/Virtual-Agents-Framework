using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace i5.VirtualAgents.Speech
{
    public interface ISpeechRecognizer
    {

        public Language Language { get; set; }

        /// <summary>
        /// The mode of the recognizer, either Single-Shot (one short sentence) or Countinuous (multiple sentences).
        /// </summary>
        public AzureRecognitionMode Mode { get; set; }

        /// <summary>
        /// Check if the recognizer is currently applicable. For example, cloud service are not applicable without internet connection.
        /// </summary>
        public bool IsApplicable { get;}

        /// <summary>
        /// Start recording and return the result.
        /// Please subscribe the OnRecognitionResultReceived event and avoid using the return value of StartRecordingAsync(), 
        /// because the result is empty for successful continuous recognition for some (Azure) recognizers.
        /// </summary>
        /// <returns>The recognition result, avoid using it.</returns>
        public Task<RecognitionResult> StartRecordingAsync();

        /// <summary>
        /// Only use to stop recording but return nothing.
        /// For some use cases that the recording stops automatically (e.g. when silence is detected), you can leave it empty.
        /// </summary>
        public Task StopRecordingAsync();

        public delegate void RecognitionResultHandler(RecognitionResult result);

        /// <summary>
        /// Fires when the recognizer receives the result.
        /// Please subscribe the OnRecognitionResultReceived event and avoid using the return value of StartRecordingAsync(), 
        /// because the result is empty for successful continuous recognition for some (Azure) recognizers.
        /// </summary>
        public event RecognitionResultHandler OnRecognitionResultReceived;
    }
}
