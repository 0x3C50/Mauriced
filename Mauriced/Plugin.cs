using System;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mauriced;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private GameObject _player;

    public static SpawnableObjectsDatabase ObjectsDatabase;
    // private static readonly Random Random = new();
    private void Awake()
    {
        Harmony.CreateAndPatchAll(typeof(Patches));
        // Plugin startup logic
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    // ReSharper disable Unity.PerformanceAnalysis  SHUT UP
    private void Update()
    {
        Database();
    }

    
    private void Database()
    {
        _player ??= GameObject.Find("Player");
        if (_player is not null)
        {
            ObjectsDatabase ??= (SpawnableObjectsDatabase)GetInstanceField(typeof(SpawnMenu),
                _player.transform.GetChild(10).GetChild(21).gameObject.GetComponent<SpawnMenu>(), "objects");
        }
    }

    private static object GetInstanceField(IReflect type, object instance, string fieldName)
    {
        const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        FieldInfo field = type.GetField(fieldName, bindFlags);
        return field.GetValue(instance);
    }
}