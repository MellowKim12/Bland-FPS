using System;
using UnityEngine;

// namespace is Bland_FPS_Hack_NEW

public class LoaderNew
{

    public static void Init()
    {
        LoaderNew.Load = new GameObject();
        LoaderNew.Load.AddComponent<ESPNew>();
        LoaderNew.Load.AddComponent<TriggerBotNew>();
        LoaderNew.Load.AddComponent<FlickNew>();
        UnityEngine.Object.Instantiate(LoaderNew.Load);
        UnityEngine.Object.DontDestroyOnLoad(LoaderNew.Load);

    }

    public static void Unload()
    {
        _Unload();
    }

    private static void _Unload()
    {
        GameObject.Destroy(Load);
    }

    private static GameObject Load;
}


