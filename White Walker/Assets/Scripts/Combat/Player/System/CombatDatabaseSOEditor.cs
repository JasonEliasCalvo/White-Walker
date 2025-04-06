using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CombatDatabaseSO))]
public class CombatDatabaseSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Update All Attacks"))
        {
            CombatDatabaseSO db = (CombatDatabaseSO)target;
            db.UpdateAllAttacks();

            EditorUtility.SetDirty(db);
            Debug.Log("All attacks list updated!");
        }
    }
}
