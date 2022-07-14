﻿using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Dependencies;
using Sessions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Debug = UnityEngine.Debug;

public static partial class Game
{
    internal static async Task Initialize()
    {
        var stopwatch = Stopwatch.StartNew();

        await IoC.RegisterAllInitialize();

        Debug.Log($"Ran initialize in {stopwatch.Elapsed.TotalSeconds:F4} seconds");
        
        await RegisterScriptableSessions();
    }

    internal static void Exiting()
    {
        IoC.Shutdown();
    }

    public static void ExitProgram()
    {
        Application.Quit();
    }

    private static async Task RegisterScriptableServices()
    {
        var assets = Directory.GetFiles("Assets/DGF/Services", "*.asset");
        foreach (var asset in assets)
        {
            var path = asset.Replace('\\', '/');
            var service = await Addressables.LoadAssetAsync<ScriptableService>(path).Task;
            IoC.Add(service);
        }
    }
    
    private static async Task RegisterScriptableSessions()
    {
        var assets = Directory.GetFiles("Assets/DGF/Sessions", "*.asset");
        var sessionService = IoC.Get<SessionService>();
        foreach (var asset in assets)
        {
            var path = asset.Replace('\\', '/');
            var session = await Addressables.LoadAssetAsync<Session>(path).Task;
            sessionService!.RegisterNewSessionType(session.GetType(), session);
        }
    }
}