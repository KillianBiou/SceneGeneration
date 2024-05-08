using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SD_PromptHandler : MonoBehaviour
{
    public SdRequest request = new SdRequest();

    private StableHandler handler;

    public bool preserve;

    public GenerativeChoice genChoice;

    int n = 4;

    private void Awake()
    {
        handler = FindFirstObjectByType<StableHandler>();
    }

    public void SendGenerationRequest()
    {
        if (preserve)
            FixName(request.filename);

        if (handler)
            handler.RequestGeneration(request);
    }


    public void FixName(string s)
    {/*
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + request.directory);
        FileInfo[] info = dir.GetFiles(request.filename);

        if(info.Length > 0)
        {
            s = s.Split('_')[0];
            if ()
            request.filename.Replace("","");
        }*/
        
        request.filename = DateTime.Now.ToString("yyyyMMddHHmmss") + "_T.png";
    }

    public void GenX()
    {
        if (genChoice != null)
        {
            genChoice.req = request;
            genChoice.Test(n);
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


    public void SetHiddenPrompt(int i)
    {
        switch(i)
        {
            case 0:
                request.hiddenPrompt = "realistic";
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
                break;
        }
    }

}
