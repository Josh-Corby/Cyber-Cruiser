using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(EnemySpawnerManager))]
public class ESMEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EnemySpawnerManager ESM = (EnemySpawnerManager)target;

        serializedObject.Update();

        if (GUILayout.Button("Reset Weights"))
        {
            ESM.ResetWeights();
            serializedObject.ApplyModifiedProperties();
            return;
        }
        serializedObject.ApplyModifiedProperties();
        ESM.OnInspectorUpdate();
    }
}
