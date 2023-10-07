using System.Collections;
using UnityEngine;

public class VoiceVoxTest : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource;


    //ここで発言を格納・事前にcmdでローカルサーバ立ち上げの必要あり
    public string AIresponse = "こんにちは";
    //話者番号格納
    public int speaker = 8;


    void Start()
    {
        StartCoroutine(SpeakTest(AIresponse));
    }

    IEnumerator SpeakTest(string text)
    {
        // VOICEVOXのREST-APIクライアント
        VoiceVoxApiClient client = new VoiceVoxApiClient();

        // テキストからAudioClipを生成（話者は「8:春日部つむぎ」）
        yield return client.TextToAudioClip(8, text);

        if (client.AudioClip != null)
        {
            // AudioClipを取得し、AudioSourceにアタッチ
            _audioSource.clip = client.AudioClip;
            // AudioSourceで再生
            _audioSource.Play();
        }
    }
}