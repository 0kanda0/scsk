using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AAA.OpenAI;

public class SendMessage : MonoBehaviour
{
    [SerializeField] private Text message;
    [SerializeField] private WhisperSpeechToText whisperSpeechToText;


    public async void OnClick()
    {
        var openAIApiKey = "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";
        var ChatGPT = new ChatGPT(openAIApiKey);
        await ChatGPT.RequestAsync("{{" + whisperSpeechToText.RecognizedText + "}}");
 
    }
}
