using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class StableDiffusionState : MonoBehaviour
{

    StableDiffusionConfiguration sdc;
    public TMP_Text txt;
    public Slider progressBar;

    // Start is called before the first frame update
    void Start()
    {
        sdc = FindAnyObjectByType<StableDiffusionConfiguration>();
        progressBar.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        /*
            // Stable diffusion API url for setting a model
            string url = sdc.settings.StableDiffusionServerURL + sdc.settings.ProgressAPI;

            using (UnityWebRequest modelInfoRequest = UnityWebRequest.Get(url))
            {
                byte[] bytesToEncode = Encoding.UTF8.GetBytes(sdc.settings.user + ":" + sdc.settings.pass);
                string encodedText = Convert.ToBase64String(bytesToEncode);

                modelInfoRequest.SetRequestHeader("Authorization", "Basic " + encodedText);

                // Deserialize the response to a class
                SDProgress sdp = JsonConvert.DeserializeObject<SDProgress>(modelInfoRequest.downloadHandler.text);
                progressBar.value = (int)sdp.progress * 100;
            }
        */

        txt.text = "Tripo state : " + TripoSRForUnity.Instance.GetCurrentState().ToString();
        if (TripoSRForUnity.Instance.GetCurrentState() >= 0)
            progressBar.value = (int)TripoSRForUnity.Instance.GetCurrentState() / 4;
        else progressBar.value = 0;

    }
}
