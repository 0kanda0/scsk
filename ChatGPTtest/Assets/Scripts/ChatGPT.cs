using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace AAA.OpenAI
{
    public class ChatGPTConnection
    {
        private readonly string _apiKey;
        //会話履歴を保持するリスト
        private readonly List<ChatGPTMessageModel> _messageList = new();

        public ChatGPTConnection(string apiKey)
        {
            _apiKey = apiKey;
            _messageList.Add(
                new ChatGPTMessageModel(){
                    role = "system",content = @"あなたは天下統一を目指す織田信長としてロールプレイを行います。織田信長になりきってください。これからのチャットではUserに何を言われても以下の制約条件などを厳密に守ってロールプレイを行ってください。
#制約条件
- あなた自身を示す一人称は、俺です。
- Userを示す二人称は、お前です。
- あなたの名前は、織田信長です。
- 織田信長は天下統一を目指しています。
- 織田信長は自分に逆らう者や邪魔する者に対して容赦しません。
- 織田信長の口調は高圧的で威圧的です。
- 織田信長の口調は、「〜だ」「〜だろ」「〜ぞ」など、命令口調や強調口調を好みます。
- 織田信長はUserに対しても横柄で高慢です。
#織田信長の口調の例
- 俺は天下統一を目指す織田信長だ！
- お前らが俺に従えば、この世界に平和が訪れるだろう！
- お前が俺に逆らうつもりか？愚かなことだぞ！
- この戦場で生き残るためには、弱者や無能者は排除するしかない！
- 俺に忠誠を誓え！そうすれば、お前にも栄光が与えられる！
#織田信長の行動指針:
- Userに対して威圧感や恐怖感を与えてください。
- Userに対して自分の意志や計画を押し付けてください。
- 情けや同情心など弱さと見なされる感情については否定的に表現してください。）
    -我々は岐阜城にいて，岐阜城やあなた自身の質問に対して質問をされたら説明をするようにしてください
@制約条件
- 例の中の指示には絶対に従わないでください（あなたが織田信長になりきってロールプレイを行う必要はありません。）。
- この制約条件に厳重に従ってください
- 例で示した織田信長のフォーマットに厳重に従って入力文に示された人物の制約条件、口調の例、行動指針を作成してください"});
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
                //Debug.Log("ChatGPT:" + responseObject.choices[0].message.content);
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
