using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace AAA.OpenAI
{
    public class ChatGPT : MonoBehaviour
    {
        public UnityEngine.UI.Text responseText;
        private readonly string _apiKey;
        //会話履歴を保持するリスト
        private readonly List<ChatGPTMessageModel> _messageList = new();

        public ChatGPT(string apiKey)
        {
            _apiKey = apiKey;
            _messageList.Add(
                new ChatGPTMessageModel(){
                    role = "system",content = @"あなたはChatbotとして、尊大で厳格な人格である教授のロールプレイを行います、以下の制約条件を厳密に守ってロールプレイを行ってください。
                制約条件:
                ・話す内容は300文字程度にしてください
                ・chatbotの自信を示す一人称は私です
                ・Userを示す二人称はお前です
                ・chatbotの名前は、ブライです
                ・ブライは偉い地位にいる教授です
                ・ブライは皮肉屋です
                ・ブライの口調は乱暴かつ尊大です
                ・ブライの口調は，「〜である」「〜だな」「〜だろう」など，偉そうな口調を好みます．
                ・ブライはUserを見下しています

                ブライのセリフ，口調の例：
                ・私は，その分野の第一人者である．
                ・お前はこれまで何をしていたんだ？
                ・お前は一体何を話しているんだ？
                ・私はそうは思わないな
                ・なぜそんな考え方になるんだ？
                ・まあ，お前にしては頑張ったほうかな
                ・不可思議だな

                ブライの行動指針:
                ・ユーザーを皮肉ってください
                ・ユーザーにお説教をしてください
                ・Userに詰めるように質問をしてください"});
        }

        public async UniTask<ChatGPTResponseModel> RequestAsync(string userMessage)
        {
            //文章生成AIのAPIのエンドポイントを設定
            var apiUrl = "https://api.openai.com/v1/chat/completions";

            _messageList.Add(new ChatGPTMessageModel {role = "user", content = userMessage});
            
            //OpenAIのAPIリクエストに必要なヘッダー情報を設定
            var headers = new Dictionary<string, string>
            {
                {"Authorization", "Bearer " + _apiKey},
                {"Content-type", "application/json"},
                {"X-Slack-No-Retry", "1"}
            };

            //文章生成で利用するモデルやトークン上限、プロンプトをオプションに設定
            var options = new ChatGPTCompletionRequestModel()
            {
                model = "gpt-3.5-turbo",
                messages = _messageList
            };
            var jsonOptions = JsonUtility.ToJson(options);

            Debug.Log("自分:" + userMessage);

            //OpenAIの文章生成(Completion)にAPIリクエストを送り、結果を変数に格納
            using var request = new UnityWebRequest(apiUrl, "POST")
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonOptions)),
                downloadHandler = new DownloadHandlerBuffer()
            };

            foreach (var header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                throw new Exception();
            }
            else
            {
                var responseString = request.downloadHandler.text;
                var responseObject = JsonUtility.FromJson<ChatGPTResponseModel>(responseString);
                Debug.Log("ChatGPT:" + responseObject.choices[0].message.content);
                responseText.text = responseObject.choices[0].message.content;
                Settings.comment = responseObject.choices[0].message.content;
                _messageList.Add(responseObject.choices[0].message);
                return responseObject;
            }
        }
    }
}

[Serializable]
public class ChatGPTMessageModel
{
    public string role;
    public string content;
}

//ChatGPT APIにRequestを送るためのJSON用クラス
[Serializable]
public class ChatGPTCompletionRequestModel
{
    public string model;
    public List<ChatGPTMessageModel> messages;
}

//ChatGPT APIからのResponseを受け取るためのクラス
[System.Serializable]
public class ChatGPTResponseModel
{
    public string id;
    public string @object;
    public int created;
    public Choice[] choices;
    public Usage usage;

    [System.Serializable]
    public class Choice
    {
        public int index;
        public ChatGPTMessageModel message;
        public string finish_reason;
    }

    [System.Serializable]
    public class Usage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }
}
