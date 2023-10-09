using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// VOICEVOX��REST-API�N���C�A���g
/// </summary>
public class VoiceVoxApiClient
{
    /// <summary> ��{ URL </summary>
    private const string BASE = "http://127.0.0.1:50021";
    /// <summary> �����N�G���擾 URL </summary>
    private const string AUDIO_QUERY_URL = BASE + "/audio_query";
    /// <summary> �������� URL </summary>
    private const string SYNTHESIS_URL = BASE + "/synthesis";

    /// <summary> �����N�G���iByte�z��j </summary>
    private byte[] _audioQueryBytes;
    /// <summary> �����N�G���iJson������j </summary>
    private string _audioQuery;
    /// <summary> �����N���b�v </summary>
    private AudioClip _audioClip;

    /// <summary> �����N�G���iJson������j </summary>
    public string AudioQuery { get => _audioQuery; }
    /// <summary> �����N���b�v </summary>
    public AudioClip AudioClip { get => _audioClip; }

    public void ClearCache()
    {
        _audioQuery = "";
        _audioQueryBytes = null;
        _audioClip = null;
    }

    /// <summary>
    /// �w�肵���e�L�X�g�����������AAudioClip�Ƃ��ďo��
    /// </summary>
    /// <param name="speakerId">�b��ID</param>
    /// <param name="text">�e�L�X�g</param>
    /// <returns></returns>
    [Obsolete]
    public IEnumerator TextToAudioClip(int speakerId, string text)
    {
        // �����N�G���𐶐�
        yield return PostAudioQuery(speakerId, text);

        // �����N�G�����特������
        yield return PostSynthesis(speakerId, _audioQueryBytes);
    }

    /// <summary>
    /// ���������p�̃N�G������
    /// </summary>
    /// <param name="speakerId">�b��ID</param>
    /// <param name="text">�e�L�X�g</param>
    /// <returns></returns>
    public IEnumerator PostAudioQuery(int speakerId, string text)
    {
        _audioQuery = "";
        _audioQueryBytes = null;
        // URL
        string webUrl = $"{AUDIO_QUERY_URL}?speaker={speakerId}&text={text}";
        // POST�ʐM
        using (UnityWebRequest request = new UnityWebRequest(webUrl, "POST"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            // ���N�G�X�g�i���X�|���X������܂őҋ@�j
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                // �ڑ��G���[
                Debug.Log("AudioQuery:" + request.error);
            }
            else
            {
                if (request.responseCode == 200)
                {
                    // ���N�G�X�g����
                    _audioQuery = request.downloadHandler.text;
                    _audioQueryBytes = request.downloadHandler.data;
                    Debug.Log("AudioQuery:" + request.downloadHandler.text);
                }
                else
                {
                    // ���N�G�X�g���s
                    Debug.Log("AudioQuery:" + request.responseCode);
                }
            }
        }
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="speakerID">�b��ID</param>
    /// <param name="audioQuery">�����N�G��</param>
    /// <returns></returns>
    [Obsolete]
    public IEnumerator PostSynthesis(int speakerID, string audioQuery)
    {
        return PostSynthesis(speakerID, Encoding.UTF8.GetBytes(audioQuery));
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="speakerId">�b��ID</param>
    /// <param name="audioQuery">�����N�G��(Byte�z��)</param>
    /// <returns></returns>
    [Obsolete]
    private IEnumerator PostSynthesis(int speakerId, byte[] audioQuery)
    {
        _audioClip = null;
        // URL
        string webUrl = $"{SYNTHESIS_URL}?speaker={speakerId}";
        // �w�b�_�[���
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");

        using (WWW www = new WWW(webUrl, audioQuery, headers))
        {
            // ���X�|���X���Ԃ�܂őҋ@
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                // �G���[
                Debug.Log("Synthesis : " + www.error);
            }
            else
            {
                // ���X�|���X���ʂ�AudioClip�Ŏ擾
                _audioClip = www.GetAudioClip(false, false, AudioType.WAV);
            }
        }
    }
}