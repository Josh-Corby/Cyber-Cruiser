using UnityEditor;

[CustomEditor(typeof(WeaponScriptableObject))]
public class WeaponEditor : Editor
{
    #region SerializedProperties

    SerializedProperty weaponName;
    SerializedProperty objectToFire;
    SerializedProperty timeBetweenShots;
    SerializedProperty holdToFire;
    SerializedProperty useSpread;
    SerializedProperty spreadAngle;
    SerializedProperty burstFire;
    SerializedProperty bursts;
    SerializedProperty timeBetweenBurstShots;
    SerializedProperty multiFire;
    SerializedProperty multiFireShots;
    SerializedProperty isMultiFireSpreadRandom;
    #endregion
    private void OnEnable()
    {
        weaponName = serializedObject.FindProperty(nameof(weaponName));
        objectToFire = serializedObject.FindProperty(nameof(objectToFire));
        timeBetweenShots = serializedObject.FindProperty(nameof(timeBetweenShots));
        holdToFire = serializedObject.FindProperty(nameof(holdToFire));
        useSpread = serializedObject.FindProperty(nameof(useSpread));
        spreadAngle = serializedObject.FindProperty(nameof(spreadAngle));
        burstFire = serializedObject.FindProperty(nameof(burstFire));
        bursts = serializedObject.FindProperty(nameof(bursts));
        timeBetweenBurstShots = serializedObject.FindProperty(nameof(timeBetweenBurstShots));
        multiFire = serializedObject.FindProperty(nameof(multiFire));
        multiFireShots = serializedObject.FindProperty(nameof(multiFireShots));
        isMultiFireSpreadRandom = serializedObject.FindProperty(nameof(isMultiFireSpreadRandom));
    }

    public override void OnInspectorGUI()
    {
        WeaponScriptableObject weaponSO = (WeaponScriptableObject)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(weaponName);
        EditorGUILayout.PropertyField(objectToFire);
        EditorGUILayout.PropertyField(timeBetweenShots);
        EditorGUILayout.PropertyField(holdToFire);

        EditorGUILayout.PropertyField(burstFire);
        if (weaponSO.burstFire)
        {
            EditorGUILayout.PropertyField(bursts);
            EditorGUILayout.PropertyField(timeBetweenBurstShots);
        }

        EditorGUILayout.PropertyField(multiFire);
        if (weaponSO.multiFire)
        {
            weaponSO.useSpread = true;
            EditorGUILayout.PropertyField(multiFireShots);
            EditorGUILayout.PropertyField(isMultiFireSpreadRandom);
        }

        EditorGUILayout.PropertyField(useSpread);
        if (weaponSO.useSpread)
        {
            EditorGUILayout.PropertyField(spreadAngle);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
