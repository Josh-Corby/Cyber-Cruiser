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
    SerializedProperty bulletsInBurst;
    SerializedProperty timeBetweenBurstShots;
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
        bulletsInBurst = serializedObject.FindProperty(nameof(bulletsInBurst));
        timeBetweenBurstShots = serializedObject.FindProperty(nameof(bulletsInBurst));
    }

    public override void OnInspectorGUI()
    {
        WeaponScriptableObject weaponSO = (WeaponScriptableObject)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(weaponName);
        EditorGUILayout.PropertyField(objectToFire);
        EditorGUILayout.PropertyField(timeBetweenShots);
        EditorGUILayout.PropertyField(holdToFire);

        EditorGUILayout.PropertyField(useSpread);
        if (weaponSO.useSpread)
        {
            EditorGUILayout.PropertyField(spreadAngle);
        }

        EditorGUILayout.PropertyField(burstFire);
        if (weaponSO.burstFire)
        {
            EditorGUILayout.PropertyField(bulletsInBurst);
            EditorGUILayout.PropertyField(timeBetweenBurstShots);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
