using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class BottomUIController : MonoBehaviour
    {

        [SerializeField] private GameObject _bottomUIPC;
        [SerializeField] private GameObject _bottomUIMobile;
        [SerializeField] private BoolReference _isPlatformPC;

        private void OnEnable()
        {
            if (_isPlatformPC.Value)
            {
                _bottomUIMobile.SetActive(false);
                _bottomUIPC.SetActive(true);
            }

            else
            {
                _bottomUIMobile.SetActive(true);
                _bottomUIPC.SetActive(false);
            }

        }
    }
}
