using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AAA.OpenAI;

public class SendMessage : MonoBehaviour
{
    [SerializeField] private Text message;

    public void OnClick()
    {
        var openAIApiKey = "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";
        var chatGPTConnection = new ChatGPTConnection(openAIApiKey);
        chatGPTConnection.RequestAsync("{{"+ message.text + "}}");
       
    }
}
