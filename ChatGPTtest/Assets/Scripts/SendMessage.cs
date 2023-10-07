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
        var openAIApiKey = "OpenAiKey";
        var chatGPTConnection = new ChatGPTConnection(openAIApiKey);
        chatGPTConnection.RequestAsync("{{"+ message.text + "}}");
       
    }
}
