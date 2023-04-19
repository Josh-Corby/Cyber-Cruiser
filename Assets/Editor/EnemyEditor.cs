using UnityEditor;

[CustomEditor(typeof(EnemyScriptableObject))]
public class EnemyEditor : Editor
{
    #region SerializedProperties
    SerializedProperty unitName;
    SerializedProperty maxHealth;
    SerializedProperty explodeOnDeath;
    SerializedProperty explosionRadius;
    SerializedProperty explosionDamage;
    SerializedProperty explosionEffect;

    SerializedProperty speed;
    SerializedProperty upDownMovement;
    SerializedProperty upDownSpeed;
    SerializedProperty upDownDistance;
    SerializedProperty seekPlayer;
    SerializedProperty seekPlayerY;
    SerializedProperty seekPlayerX;
    SerializedProperty seekSpeed;
    SerializedProperty sinUpDownMovement;
    SerializedProperty sinFrequency;
    SerializedProperty sinMagnitude;
    SerializedProperty homeOnPlayer;
    SerializedProperty homeTurnSpeed;
    SerializedProperty homeTime;
    SerializedProperty homeDelay;
    SerializedProperty homeDelayTime;
    SerializedProperty moveTypes;
    #endregion

    private void OnEnable()
    {
        unitName = serializedObject.FindProperty(nameof(unitName));
        maxHealth = serializedObject.FindProperty(nameof(maxHealth));
        explodeOnDeath = serializedObject.FindProperty(nameof(explodeOnDeath));
        explosionRadius = serializedObject.FindProperty(nameof(explosionRadius));
        explosionDamage = serializedObject.FindProperty(nameof(explosionDamage));
        explosionEffect = serializedObject.FindProperty(nameof(explosionEffect));

        moveTypes = serializedObject.FindProperty(nameof(moveTypes));
        speed = serializedObject.FindProperty(nameof(speed));
        upDownMovement = serializedObject.FindProperty(nameof(upDownMovement));
        upDownSpeed = serializedObject.FindProperty(nameof(upDownSpeed));
        upDownDistance = serializedObject.FindProperty(nameof(upDownDistance));

        seekPlayer = serializedObject.FindProperty(nameof(seekPlayer));
        seekPlayerY = serializedObject.FindProperty(nameof(seekPlayerY));
        seekPlayerX = serializedObject.FindProperty(nameof(seekPlayerX));
        seekSpeed = serializedObject.FindProperty(nameof(seekSpeed));
      
        sinUpDownMovement = serializedObject.FindProperty(nameof(sinUpDownMovement));
        sinFrequency = serializedObject.FindProperty(nameof(sinFrequency));
        sinMagnitude = serializedObject.FindProperty(nameof(sinMagnitude));
      
        homeOnPlayer = serializedObject.FindProperty(nameof(homeOnPlayer));
        homeTurnSpeed = serializedObject.FindProperty(nameof(homeTurnSpeed));
        homeTime = serializedObject.FindProperty(nameof(homeTime));
        homeDelay = serializedObject.FindProperty(nameof(homeDelay));
        homeDelayTime = serializedObject.FindProperty(nameof(homeDelayTime));
    }

    public override void OnInspectorGUI()
    {
        EnemyScriptableObject enemySO = (EnemyScriptableObject)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(unitName);
        EditorGUILayout.PropertyField(maxHealth);
        EditorGUILayout.PropertyField(explodeOnDeath);
        if (enemySO.explodeOnDeath)
        {
            EditorGUILayout.PropertyField(explosionRadius);
            EditorGUILayout.PropertyField(explosionDamage);
            EditorGUILayout.PropertyField(explosionEffect);
        }

        EditorGUILayout.PropertyField(moveTypes);
        EditorGUILayout.PropertyField(speed);

        switch (enemySO.moveTypes)
        {
            case MovementTypes.UpDown:
                EditorGUILayout.PropertyField(upDownSpeed);
                EditorGUILayout.PropertyField(upDownDistance);
                break;
            case MovementTypes.SeekPlayer:
                EditorGUILayout.PropertyField(seekPlayerY);
                EditorGUILayout.PropertyField(seekPlayerX);
                EditorGUILayout.PropertyField(seekSpeed);
                break;
            case MovementTypes.SinUpDown:
                EditorGUILayout.PropertyField(sinFrequency);
                EditorGUILayout.PropertyField(sinMagnitude);
                break;
            case MovementTypes.HomeOnPlayer:
                EditorGUILayout.PropertyField(homeTurnSpeed);
                EditorGUILayout.PropertyField(homeTime);
                EditorGUILayout.PropertyField(homeDelay);
                if (enemySO.homeDelay)
                {
                    EditorGUILayout.PropertyField(homeDelayTime);
                }
                break;
        }
        enemySO.upDownMovement = enemySO.moveTypes == MovementTypes.UpDown;
        enemySO.seekPlayer = enemySO.moveTypes == MovementTypes.SeekPlayer;
        enemySO.sinUpDownMovement = enemySO.moveTypes == MovementTypes.SinUpDown;
        enemySO.homeOnPlayer = enemySO.moveTypes == MovementTypes.HomeOnPlayer;

        serializedObject.ApplyModifiedProperties();
    }
}
