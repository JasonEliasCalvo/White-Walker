using UnityEngine;
using UnityEditor;
using System.Linq;

public class AutoIDGenerator : AssetPostprocessor
{
    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        foreach (string assetPath in importedAssets)
        {
            if (assetPath.EndsWith(".asset"))
            {
                var attack = AssetDatabase.LoadAssetAtPath<AttackBase>(assetPath);
                if (attack != null && attack.attackID == 0)
                {
                    AssignUniqueID(attack);
                    EditorUtility.SetDirty(attack);
                    AssetDatabase.SaveAssets();
                }
            }
        }
    }

    static void AssignUniqueID(AttackBase newAttack)
    {
        // Obtener todos los AttackBase existentes
        var guids = AssetDatabase.FindAssets("t:AttackBase");
        var existingIDs = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<AttackBase>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(a => a != null && a != newAttack)
            .Select(a => a.attackID)
            .ToList();

        // Encontrar el siguiente ID libre
        int nextID = 1;
        while (existingIDs.Contains(nextID))
            nextID++;

        newAttack.attackID = nextID;
    }
}
