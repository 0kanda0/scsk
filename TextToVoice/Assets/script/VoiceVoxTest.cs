using System.Collections;
using UnityEngine;

public class VoiceVoxTest : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource;

    public string AIresponse = "ありがとう";
    public int speaker = 8;

    private VoiceVoxApiClient _voiceVoxClient;

    private void Awake()
    {
        _voiceVoxClient = new VoiceVoxApiClient();
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            StartCoroutine(SpeakTest(AIresponse));
        }
    }

    void Start()
    {
        StartCoroutine(SpeakTest(AIresponse));
    }

    IEnumerator SpeakTest(string text)
    {
        _voiceVoxClient.ClearCache();

        yield return _voiceVoxClient.TextToAudioClip(speaker, text);

        if (_voiceVoxClient.AudioClip != null)
        {
            _audioSource.clip = _voiceVoxClient.AudioClip;
            _audioSource.Play();
        }
    }
}
