using System.Collections;
using UnityEngine;

public class VoiceVoxTest : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource;


    //�����Ŕ������i�[�E���O��cmd�Ń��[�J���T�[�o�����グ�̕K�v����
    public string AIresponse = "こんにちは";
    //�b�Ҕԍ��i�[
    public int speaker = 8;


    void Start()
    {
        StartCoroutine(SpeakTest(AIresponse));
    }

    IEnumerator SpeakTest(string text)
    {
        // VOICEVOX��REST-API�N���C�A���g
        VoiceVoxApiClient client = new VoiceVoxApiClient();

        // �e�L�X�g����AudioClip�𐶐��i�b�҂́u8:�t�����ނ��v�j
        yield return client.TextToAudioClip(8, text);

        if (client.AudioClip != null)
        {
            // AudioClip���擾���AAudioSource�ɃA�^�b�`
            _audioSource.clip = client.AudioClip;
            // AudioSource�ōĐ�
            _audioSource.Play();
        }
    }
}