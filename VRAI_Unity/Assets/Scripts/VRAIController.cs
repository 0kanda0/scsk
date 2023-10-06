using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRAIController : MonoBehaviour
{
    [SerializeField] VoiceInputter voiceInputer;
    [SerializeField] WhisperManager whisperManager;
    [SerializeField] ChatGPTManager chatGPTManager;
    [SerializeField] VoiceVoxManager voiceVoxManager;
    [SerializeField] VoiceOutputter voiceOutputer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// マイクの音声入力 → 入力を文字列へ変換 → 返答作成 → 返答を音声へ変換 → 音声を出力
    /// </summary>
    void Talk()
    {
        AudioClip inputAudio = voiceInputer.InputVoice();
        string inputText = whisperManager.SpeechToText(inputAudio);
        string outputText = chatGPTManager.MakeResponse(inputText);
        AudioClip voiceVox = voiceVoxManager.TextToSpeech(outputText);
        voiceOutputer.OutputVoice(voiceVox);
    }
}
