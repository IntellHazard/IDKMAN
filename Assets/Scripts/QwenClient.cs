
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class QwenClient : MonoBehaviour
{
    private const string ChatUrl = "http://127.0.0.1:5000/chat";

    [Serializable]
    private class ChatRequest
    {
        public string prompt;
    }

    [Serializable]
    private class StreamChunk
    {
        public string token;
        public string error;
        public bool done;
    }

    private class StreamingDownloadHandler : DownloadHandlerScript
    {
        private readonly Action<string> onChunk;
        private readonly Action<string> onError;
        private readonly List<byte> pendingBytes = new List<byte>();

        public StreamingDownloadHandler(
            Action<string> chunkCallback,
            Action<string> errorCallback
        ) : base(new byte[4096])
        {
            onChunk = chunkCallback;
            onError = errorCallback;
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (data == null || dataLength <= 0)
                return true;

            for (int i = 0; i < dataLength; i++)
            {
                if (data[i] == (byte)'\n')
                {
                    ProcessPendingLine();
                }
                else
                {
                    pendingBytes.Add(data[i]);
                }
            }

            return true;
        }

        public void ProcessPendingLine()
        {
            if (pendingBytes.Count == 0)
                return;

            string line = Encoding.UTF8.GetString(pendingBytes.ToArray());
            pendingBytes.Clear();

            line = line.Trim();

            if (string.IsNullOrEmpty(line))
                return;

            try
            {
                StreamChunk chunk = JsonUtility.FromJson<StreamChunk>(line);

                if (chunk == null)
                    return;

                if (!string.IsNullOrEmpty(chunk.error))
                {
                    onError?.Invoke(chunk.error);
                    return;
                }

                if (!string.IsNullOrEmpty(chunk.token))
                {
                    onChunk?.Invoke(chunk.token);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Could not parse Qwen stream line: " + ex.Message);
            }
        }
    }

    // Streaming version used by the chat controller.
    public void SendMessageToQwen(
    string prompt,
    Action<string> onChunk,
    Action onComplete,
    Action<string> onError
    )
    {
        StartCoroutine(
            SendStreamingRequest(prompt, onChunk, onComplete, onError)
        );
    }

    // Backwards-compatible version for any existing scripts
    // that expect one complete response.
    public void SendMessageToQwen(
        string prompt,
        Action<string> onComplete
    )
    {
        StringBuilder completeResponse = new StringBuilder();

        SendMessageToQwen(
            prompt,
            chunk => completeResponse.Append(chunk),
            () => onComplete?.Invoke(completeResponse.ToString()),
            error => onComplete?.Invoke("Sorry, I ran into an error: " + error)
        );
    }

    private IEnumerator SendStreamingRequest(
    string prompt,
    Action<string> onChunk,
    Action onComplete,
    Action<string> onError
    )
    {
        string json = JsonUtility.ToJson(
            new ChatRequest { prompt = prompt }
        );

        bool streamErrorReceived = false;

        StreamingDownloadHandler handler = new StreamingDownloadHandler(
            onChunk,
            error =>
            {
                streamErrorReceived = true;
                onError?.Invoke(error);
            }
        );

        using (UnityWebRequest request = new UnityWebRequest(ChatUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(
                Encoding.UTF8.GetBytes(json)
            );

            request.downloadHandler = handler;
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/x-ndjson");

            yield return request.SendWebRequest();

            handler.ProcessPendingLine();

            if (streamErrorReceived)
                yield break;

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(request.error);
                yield break;
            }

            onComplete?.Invoke();
        }
    }
}
