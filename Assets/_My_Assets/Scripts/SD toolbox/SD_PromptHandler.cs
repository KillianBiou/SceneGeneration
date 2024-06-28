using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SD_PromptHandler : MonoBehaviour
{

    private SdRequest request = new SdRequest();
    private string _model = "";

    [SerializeField]
    private GenerativeChoice genChoice;
    [SerializeField]
    private GameObject modelInput;


    public void SendGenerationRequest()
    {
        request.filename = request.prompt + DateTime.Now.ToString("_MMdd-HHmmss") + "_T.png";
        StableCompleteRequest sdcr = new StableCompleteRequest(request, _model, 1, "");
        DiffuserInterface.Instance.RequestGeneration(sdcr);
    }

    public void GenX(int n)
    {
        if (genChoice != null)
        {
            genChoice.GenerateChoices(request, n);
        }
    }


    public void SetPrompt(string s)
    {
        request.prompt = s;
    }

    public void SetNegPrompt(string s)
    {
        request.negPrompt = s;
    }


    public void SetWidth(float i)
    {
        request.width = (int)i;
    }

    public void SetHeight(float i)
    {
        request.height = (int)i;
    }



    public void SetWidth(string i)
    {
        request.width = int.Parse(i);
    }

    public void SetHeight(string i)
    {
        request.height = int.Parse(i);
    }

    public void SetWidth(int i)
    {
        request.width = i;
    }

    public void SetHeight(int i)
    {
        request.height = i;
    }



    public void SetSteps(string i)
    {
        request.steps = int.Parse(i);
    }
    public void SetCfg(string i)
    {
        request.cfgScale = int.Parse(i);
    }
    public void SetSteps(float i)
    {
        request.steps = (int)i;
    }
    public void SetCfg(float i)
    {
        request.cfgScale = (int)i;
    }




    public void SetSeed(string i)
    {
        request.seed = int.Parse(i);
    }



    public void SetFileName(string s)
    {
        request.filename = s;
    }
    public void SetDirectory(string s)
    {
        request.directory = s;
    }


    public void SetTile(bool b)
    {
        request.tilling = b;
    }
    public void SetTileX(bool b)
    {
        request.tileX = b;
    }
    public void SetTileY(bool b)
    {
        request.tileY = b;
    }

    public void SetGenerationStyle(int i)
    {
        modelInput.SetActive(false);

        switch (i)
        {
            case 0:
                request.hiddenPrompt = "realistic";
                //_model = "truc";
                break;
            case 1:
                request.hiddenPrompt = "stylized";
                break;
            case 2:
                request.hiddenPrompt = "fantasy";
                break;
            case 3:
                request.hiddenPrompt = "science fiction";
                break;
            case 4:
                request.hiddenPrompt = "abstract";
                break;
            case 5:
                request.hiddenPrompt = "modern";
                break;
            default:
                request.hiddenPrompt = "";
                modelInput.SetActive(true);
                break;
        }
    }

    public void SetModel(string s)
    {
        _model = s;
    }

}
