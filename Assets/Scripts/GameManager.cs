using UnityEngine;
using System;
public class GameManager : MonoBehaviour
{
    public static event Action OnLevelCountDownStart = null;


    private void Start()
    {
        OnLevelCountDownStart?.Invoke();
    }


}
