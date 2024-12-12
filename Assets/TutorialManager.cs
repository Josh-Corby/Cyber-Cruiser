using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private BoolValue _isPlatformPC;

        [SerializeField] private int _plasmaTutorialsDone = 0;
        private int _plasmaTutorialsMax = 5;

        [SerializeField] private SpriteRenderer _PCShieldTutorial;
        [SerializeField] private SpriteRenderer _mobileShieldTutorial;
        [SerializeField] private int _tutorialDuration = 5;

        [SerializeField] private SpriteRenderer _PCMoveTutorial;
        [SerializeField] private SpriteRenderer _mobileMoveTutorial;

        [SerializeField] private SpriteRenderer _PCShootTutorial;
        [SerializeField] private SpriteRenderer _mobileShootTutorial;

        [SerializeField] private PlayerRankManager _rankManager;

        private void OnEnable()
        {
            Pickup.OnPlasmaPickupSpawned += PlasmaTutorial;
            PlayerManager.OnPlayerCanAffordShield += ShieldTutorial;
            GameManager.OnMissionStart += MoveTutorial;
        }

        private void OnDisable()
        {
            Pickup.OnPlasmaPickupSpawned -= PlasmaTutorial;
            PlayerManager.OnPlayerCanAffordShield -= ShieldTutorial;
            GameManager.OnMissionStart -= MoveTutorial;
        }


        // Shows Plasma Pickup first 4 instances on launch 
        // DOES NOT SAVE
        private void PlasmaTutorial(Pickup plasmaPickup)
        {
            if (_plasmaTutorialsDone <= _plasmaTutorialsMax) 
            {
                Debug.Log("Tutorial manager enabling plasma tutorial sprite");
                plasmaPickup.EnablePlasmaTutorial();
            }
            else
            {
                plasmaPickup.DisablePlasmaTutorial();
            }

            _plasmaTutorialsDone++;
        }

        private void ShieldTutorial()
        {
            if(_isPlatformPC.Value)
            {
                if(_rankManager.CurrentRank.RankID == 0)
                    StartCoroutine(TutorialCoroutine(_PCShieldTutorial, _tutorialDuration));
            }
            else
            {
                StartCoroutine(TutorialCoroutine(_mobileShieldTutorial, _tutorialDuration));
            }
        }

        private IEnumerator TutorialCoroutine(SpriteRenderer tutorialSprite, int duration)
        {
            tutorialSprite.enabled = true;
            yield return new WaitForSeconds(duration);
            tutorialSprite.enabled = false;
        }

        private void MoveTutorial()
        {
            if(_rankManager.CurrentRank.RankID == 0)
            {
                StartCoroutine(nameof(MoveTutorialCoroutine));
            }
        }

        private IEnumerator MoveTutorialCoroutine()
        {
            if (_isPlatformPC.Value)
            {
                StartCoroutine(TutorialCoroutine(_PCMoveTutorial, 2));
            }
            else
            {
                StartCoroutine(TutorialCoroutine(_mobileMoveTutorial, 2));
            }

            yield return new WaitForSeconds(2);
            ShootTutorial();
        }


        private void ShootTutorial()
        {
            if (_isPlatformPC.Value)
            {
                StartCoroutine(TutorialCoroutine(_PCShootTutorial, 2));
            }
            else
            {
                StartCoroutine(TutorialCoroutine(_mobileShootTutorial, 2));
            }
        }
    }
}
