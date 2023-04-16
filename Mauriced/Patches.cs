using GameConsole;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using Console = System.Console;

namespace Mauriced;

public class Patches
{
    [HarmonyPatch(typeof(ActivateNextWave), "Awake")]
    [HarmonyPrefix]
    private static void ArenaAwakePre(ActivateNextWave __instance)
    {
        PatchWave(__instance);
    }
    
    [HarmonyPatch(typeof(ActivateArena), "Activate")]
    [HarmonyPrefix]
    private static void AArenaAwakePre(ActivateArena __instance)
    {
        PatchArena(__instance);
    }

    private static void PatchArena(ActivateArena aa)
    {
        for (int i = 0; i < aa.enemies.Length; i++)
        {
            aa.enemies[i] = ReplaceEnemy(aa.enemies[i]);
        }
    }

    private static void PatchWave(ActivateNextWave anw)
    {
        GameObject[] anwNextEnemies = anw.nextEnemies;
        for (int i = 0; i < anwNextEnemies.Length; i++)
        {
            anwNextEnemies[i] = ReplaceEnemy(anwNextEnemies[i]);
        }
    }
    
    private static void EnemyPositionFix(GameObject ne, GameObject enemy)
    {
        if (enemy.TryGetComponent(out V2 _) && SceneManager.GetActiveScene().name == "Level 1-4")
        {
            ne.transform.position = new Vector3(0, -19.5f, 627);
        }
    }

    private static void CheckEnemiesStuff(GameObject ne, GameObject enemy)
    {
        if (ne.TryGetComponent(out EnemyIdentifier ei) && enemy.TryGetComponent(out EventOnDestroy eod))
        {
            ei.onDeath.AddListener(eod.stuff.Invoke);
        }

        if (ne.TryGetComponent(out KeepInBounds kib))
        {
            Object.Destroy(kib);
        }

        if (enemy.TryGetComponent(out LeviathanController lc))
        {
            lc.DeathEnd();
        }
    }

    private static GameObject ReplaceEnemy(GameObject old)
    {
        // maurice is slot 15, index 14
        SpawnableObject objectsDatabaseEnemy = Plugin.ObjectsDatabase.enemies[14];
        Console.WriteLine($"{old} -> {objectsDatabaseEnemy.gameObject}");
        
        GameObject ne = Object.Instantiate(objectsDatabaseEnemy.gameObject, old.transform.parent, true);
        
        ne.SetActive(old.activeSelf);
        
        CheckEnemiesStuff(ne, old);
        
        ne.transform.position = old.transform.position;

        // enemy.name += " mauriced";
        EnemyPositionFix(ne, old);
            
        Object.Destroy(old);
        return ne;
    }
}