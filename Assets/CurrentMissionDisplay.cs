using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentMissionDisplay : GameBehaviour
{
    [SerializeField] private TMP_Text _missionDescription, _missionProgress;

    private void OnEnable()
    {
        if(MM.CurrentMission == null)
        {
            _missionDescription.text = "";
            _missionProgress.text = "";
        }
        else
        {
            _missionDescription.text = MM.CurrentMission.missionDescription;
            _missionProgress.text = "Current progress: " + MM.CurrentMissionProgress.ToString();
        }
    }
}
