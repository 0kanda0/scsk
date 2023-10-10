using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using AAA.OpenAI;
using System.Collections;

public class ChatToVoiceController : MonoBehaviour
{
    
    [SerializeField]
    private WhisperSpeechToText whisperSpeechToText;

    private ChatGPTConnection _chatGptConnection;
    private VoiceVoxApiClient _voiceVoxApiClient;
    private OVRLipSyncContextMorphTarget lipSyncController;


    private const string API_KEY = "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"; // あなたのOpenAIのAPIキーを設定してください
    private const int SPEAKER_ID = 13; // VOICEVOXの話者IDを設定してください (例: 1)
    


    private void Start()
    {
         _chatGptConnection = new ChatGPTConnection(API_KEY);
         _voiceVoxApiClient = new VoiceVoxApiClient();
        lipSyncController = GetComponent<OVRLipSyncContextMorphTarget>();

    }

    public void OnClick()
    {
        GetUserResponseAndPlay();
    }

  public async UniTaskVoid GetUserResponseAndPlay()
    {
        //Debug.Log("chatToVoiceControllerで認識したテキスト：" + whisperSpeechToText.RecognizedText);
        string userMessage = whisperSpeechToText.RecognizedText;
        string responseString = await _chatGptConnection.RequestAsync(userMessage);
        ChatGPTResponse gptResponse = JsonUtility.FromJson<ChatGPTResponse>(responseString);
        Debug.Log("ChatGPT Answer: " + gptResponse.message);
        //Debug.Log($"Emotions - Joy: {gptResponse.emotion.joy}, Fun: {gptResponse.emotion.fun}, Anger: {gptResponse.emotion.anger}, Sorrow: {gptResponse.emotion.sorrow}, Surprised: {gptResponse.emotion.surprised}");
        // OVRLipSyncContextMorphTargetのEmotionControlを呼び出し
        if (lipSyncController != null)
        {
            lipSyncController.EmotionControl(
                (int)gptResponse.emotion.fun, 
                (int)gptResponse.emotion.sorrow, 
                (int)gptResponse.emotion.anger, 
                (int)gptResponse.emotion.joy, 
                (int)gptResponse.emotion.surprised
            );
        }
        StartCoroutine(ConvertTextToSpeechAndPlay(gptResponse.message));
    }

    private IEnumerator ConvertTextToSpeechAndPlay(string gptResponse)
    {
        yield return _voiceVoxApiClient.TextToAudioClip(SPEAKER_ID, gptResponse);
        AudioSource audioSource = GetComponent<AudioSource>();
        
        audioSource.clip = _voiceVoxApiClient.AudioClip;
        audioSource.Play();
    }

    [System.Serializable]
    public class ChatGPTResponse
    {
        public Emotion emotion;
        public string message;

        [System.Serializable]
        public class Emotion
        {
            public float joy;
            public float fun;
            public float anger;
            public float sorrow;
            public float surprised;
        }
    }
}