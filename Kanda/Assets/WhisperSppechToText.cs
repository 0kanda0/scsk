using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using System;
using System.IO;

public class WhisperSppechToText : MonoBehaviour
{
    //[SerializeField] private TextInterface _textInterface;
    public int frequency = 16000; 
    public int maxRecordingTime = 10; 

    private AudioClip clip;
    private float recordingTime;
    void Start (){
 
    }
    void Update()
    {
        if (IsRecording()) {
            Debug.Log("成功");

            recordingTime += Time.deltaTime;
            
            if (Mathf.FloorToInt(recordingTime) >= maxRecordingTime)
            {
                StopRecording();
            }
            
        }
        else {
            Debug.Log("失敗");
        }
    }

    public void StartRecording(){
        recordingTime=0;
        //_textInterface.Disable();
        
        string[] micDevices = Microphone.devices;
        if (IsRecording())
        {
            Microphone.End(null);
        }

        Debug.Log("RecordingStart");
        clip = Microphone.Start(null, true, 10, frequency);
    }

    public bool IsRecording()
    {
        return Microphone.IsRecording(null);
    }

    public void StopRecording() {
        Debug.Log("RecordingStop.");
        Microphone.End(null);
        var audioData = WavUtility.FromAudioClip(clip);
        
        string dateTimeString = DateTime.Now.ToString("yyyyMMddHHmmss");
        string fileName = $"recordedAudio_{dateTimeString}.wav";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllBytes(path, audioData);


        Debug.Log($"Saved recording to: {path}");
        StartCoroutine(SendRequest(audioData));
    }

    
    IEnumerator SendRequest(byte[] audioData)
    {
        string url = "https://api.openai.com/v1/audio/transcriptions";
        string accessToken = "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";
        
        var formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("model", "whisper-1"));
        formData.Add(new MultipartFormDataSection("language", "ja"));
        formData.Add(new MultipartFormFileSection("file", audioData, "audio.mp3", "multipart/form-data"));

        using (UnityWebRequest request = UnityWebRequest.Post(url, formData))
        {
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);
            yield return request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success) 
            {
                Debug.LogError(request.error);
                yield break;
            }

            string jsonResponse = request.downloadHandler.text;
            string recognizedText = "";
            try 
            {
                recognizedText = JsonUtility.FromJson<WhisperResponseModel>(jsonResponse).text;
            } 
            catch (System.Exception e) 
            {
                Debug.LogError(e.Message);
            }

            Debug.Log("Input Text: " + recognizedText);
            
            //_textInterface.InputField.text = recognizedText;
            //_textInterface.OnSubmit();
            
        }
    }
    
    

    public static class WavUtility 
    {
        public static byte[] FromAudioClip(AudioClip clip)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            // Write WAV header
            writer.Write(0x46464952);
            writer.Write(0); 
            writer.Write(0x45564157);
            writer.Write(0x20746d66);
            writer.Write(16); 
            writer.Write((ushort)1);
            writer.Write((ushort)clip.channels);
            writer.Write(clip.frequency); 
            writer.Write(clip.frequency * clip.channels * 2);
            writer.Write((ushort)(clip.channels * 2)); 
            writer.Write((ushort)16); 
            writer.Write(0x61746164); 
            writer.Write(0); 

            float[] samples = new float[clip.samples];
            clip.GetData(samples, 0);
            short[] intData = new short[samples.Length];
            for (int i = 0; i < samples.Length; i++) 
            {
                intData[i] = (short)(samples[i] * 32767f);
            }
            byte[] data = new byte[intData.Length * 2];
            Buffer.BlockCopy(intData, 0, data, 0, data.Length);
            writer.Write(data);

            writer.Seek(4, SeekOrigin.Begin);
            writer.Write((int)(stream.Length - 8));
            writer.Seek(40, SeekOrigin.Begin);
            writer.Write((int)(stream.Length - 44));
            writer.Close();
            stream.Close();
            return stream.ToArray();
        }
    }
}


public class WhisperResponseModel
{
    public string text;
}
