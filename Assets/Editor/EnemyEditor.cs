using UnityEditor;

[CustomEditor(typeof(EnemyScriptableObject))]
public class EnemyEditor : Editor
{
    #region SerializedProperties
    SerializedProperty unitPrefab;
    SerializedProperty unitName;
    SerializedProperty maxHealth;
    SerializedProperty explodeOnDeath;
    SerializedProperty explosionRadius;
    SerializedProperty explosionDamage;
    SerializedProperty explosionEffect;

    SerializedProperty clusterOnDeath;
    SerializedProperty spawnEnemy;
    SerializedProperty enemyToSpawn;
    SerializedProperty objectToSpawn;
    SerializedProperty amountOfObjects;

    SerializedProperty speed;
    SerializedProperty crashSpeed;
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
        unitPrefab = serializedObject.FindProperty(nameof(unitPrefab));
        unitName = serializedObject.FindProperty(nameof(unitName));
        maxHealth = serializedObject.FindProperty(nameof(maxHealth));
        explodeOnDeath = serializedObject.FindProperty(nameof(explodeOnDeath));
        explosionRadius = serializedObject.FindProperty(nameof(explosionRadius));
        explosionDamage = serializedObject.FindProperty(nameof(explosionDamage));
        explosionEffect = serializedObject.FindProperty(nameof(explosionEffect));

        clusterOnDeath = serializedObject.FindProperty(nameof(clusterOnDeath));
        spawnEnemy = serializedObject.FindProperty(nameof(spawnEnemy));
        enemyToSpawn = serializedObject.FindProperty(nameof(enemyToSpawn));
        objectToSpawn = serializedObject.FindProperty(nameof(objectToSpawn));
        amountOfObjects = serializedObject.FindProperty(nameof(amountOfObjects));

        moveTypes = serializedObject.FindProperty(nameof(moveTypes));
        speed = serializedObject.FindProperty(nameof(speed));
        crashSpeed = serializedObject.FindProperty(nameof(crashSpeed));
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

        EditorGUILayout.PropertyField(unitPrefab);
        EditorGUILayout.PropertyField(unitName);
        EditorGUILayout.PropertyField(maxHealth);
        EditorGUILayout.PropertyField(explodeOnDeath);

        if (enemySO.explodeOnDeath)
        {
            EditorGUILayout.PropertyField(explosionRadius);
            EditorGUILayout.PropertyField(explosionDamage);
            EditorGUILayout.PropertyField(explosionEffect);
        }

        else
        {
            EditorGUILayout.PropertyField(crashSpeed);
        }

        EditorGUILayout.PropertyField(clusterOnDeath);
        if (enemySO.clusterOnDeath)
        {
            EditorGUILayout.PropertyField(spawnEnemy);
            if (enemySO.spawnEnemy)
            {
                EditorGUILayout.PropertyField(enemyToSpawn);
            }
            else
            {
                EditorGUILayout.PropertyField(objectToSpawn);
            }
            EditorGUILayout.PropertyField(amountOfObjects);
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
