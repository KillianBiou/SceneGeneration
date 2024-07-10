using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Whisper;
using Whisper.Utils;

public class TTStoInputField : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField _inputField;

    [SerializeField]
    private MicrophoneRecord micRecord;

    private WhisperManager wm;



    private void Awake()
    {
        wm = FindFirstObjectByType<WhisperManager>();
        micRecord.OnRecordStop += OnRecordStop;
    }


    public void StartWaitingForTTS()
    {
        _inputField.interactable = false;
    }


    private async void OnRecordStop(AudioChunk recordedAudio)
    {
        //buttonText.text = "Record";
        //_buffer = "";

        var sw = new Stopwatch();
        sw.Start();

        var res = await wm.GetTextAsync(recordedAudio.Data, recordedAudio.Frequency, recordedAudio.Channels);
        if (res == null || !_inputField)
            return;

        var time = sw.ElapsedMilliseconds;
        var rate = recordedAudio.Length / (time * 0.001f);
        //timeText.text = $"Time: {time} ms\nRate: {rate:F1}x";

        var text = res.Result;
        //if (printLanguage)
        //    text += $"\n\nLanguage: {res.Language}";

        _inputField.text = text;
        _inputField.interactable = true;
        //UiUtils.ScrollDown(scroll);
    }

}
