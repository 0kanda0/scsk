using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AAA.OpenAI;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var openAIApiKey = "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";
        var chatGPTConnection = new ChatGPTConnection(openAIApiKey);
        chatGPTConnection.RequestAsync("{好きな魚料理を一つ教えて}}");
        //好きな魚料理を1つ教えて など
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
