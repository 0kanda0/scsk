using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRAIController : MonoBehaviour
{
    [SerializeField] VoiceInputter voiceInputer;
    //[SerializeField] WhisperSpeechToText whisperSpeechToText;
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
    /// �}�C�N�̉������� �� ���͂𕶎���֕ϊ� �� �ԓ��쐬 �� �ԓ��������֕ϊ� �� �������o��
    /// </summary>
    void Talk()
    {
        AudioClip inputAudio = voiceInputer.InputVoice();
        //string inputText = whisperSpeechToText.SpeechToText(inputAudio);
        //string outputText = chatGPTManager.MakeResponse(inputText);
        //AudioClip voiceVox = voiceVoxManager.TextToSpeech(outputText);
        //voiceOutputer.OutputVoice(voiceVox);
    }
}
