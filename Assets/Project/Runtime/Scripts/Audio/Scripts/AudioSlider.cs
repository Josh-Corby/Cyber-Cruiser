using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class AudioSlider : MonoBehaviour
    {
        [SerializeField] private FloatValue _audioValueInDB;
        private Slider _slider;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }
        private void OnEnable()
        {
            _slider.value = MathF.Pow(10f, _audioValueInDB.Value/20f);
        }

    }
}
