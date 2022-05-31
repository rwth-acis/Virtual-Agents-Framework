using Microsoft.CognitiveServices.Speech;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace i5.VirtualAgents.Speech
{
    public interface ISpeechSynthesizer
    {
        public Language Language { get; set; }

        /// <summary>
        /// If it is set to ToSpeaker, the synthesizer should speak the audio out directly when it gets the result.
        /// If it is set to AsByteStream, the synthesizer should pass the raw audio data as Byte[] to the SynthesisResult for downstream modules, e.g. converting to an audio clip.
        /// Except for compatibility consideration, as a byte stream allows the audio to be played by an Audio Source and especially as a 3D audio clip.
        /// </summary>
        public AudioDataOutputForm OutputForm { get; set; }

        /// <summary>
        /// Check if the synthesizer is currently applicable. For example, cloud service are not applicable without internet connection. 
        /// It is encouraged to have a native solution which works without internet connection.
        /// </summary>
        public bool IsApplicable { get; }
        
        /// <summary>
        /// Synthesizing the given text and speaking the audio out.
        /// </summary>
        /// <param name="text"> The text to synthesize</param>
        public Task<SynthesisResult> StartSynthesizingAndSpeakingAsync(string text);

        public delegate void SynthesisResultHandler(SynthesisResult result);
        /// <summary>
        /// Fires when the synthesis is complete.
        /// </summary>
        public event SynthesisResultHandler OnSynthesisResultReceived;
    }
}
