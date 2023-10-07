using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AAA.OpenAI;

public class SendMessage : MonoBehaviour
{
    [SerializeField] private Text message;
    [SerializeField] private WhisperSppechToText whisperSpeechToText;


    public async void OnClick()
    {
        var openAIApiKey = "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";
        var chatGPTConnection = new ChatGPTConnection(openAIApiKey);
        await chatGPTConnection.RequestAsync("{{" + whisperSpeechToText.RecognizedText + "}}");
 
    }
}
