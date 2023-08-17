using TMPro;
using UnityEngine;

namespace CyberCruiser
{
    public class PanelWithPages : MonoBehaviour
    {
        [SerializeField] private GameObject[] _pages;
        [SerializeField] private TMP_Text _pageCounter;
        private GameObject _currentPage;
        private int _currentPageIndex;

        private int _totalPages;

        private void Awake()       
        {
            _totalPages = _pages.Length;
            _currentPageIndex = 0;
            _currentPage = _pages[0];

            _pages[0].SetActive(true);
        }

        private void OnEnable()
        {
            _currentPageIndex = 0;
            ChangePage(0);
        }

        /// <summary>
        /// change pages by x amount
        /// </summary>
        /// <param name="targetIndex">How many pages you want to change by. Number can be positive or negative</param>
        public void ChangePage(int targetIndex)
        {
            _currentPageIndex += targetIndex;

            if(_currentPageIndex < 0)
            {
                _currentPageIndex = _totalPages - 1;
            }

            else if (_currentPageIndex >= _totalPages)
            {
                _currentPageIndex = 0;
            }

            if(_currentPage != _pages[_currentPageIndex])
            {
                _currentPage.SetActive(false);
                _currentPage = _pages[_currentPageIndex];
                _currentPage.SetActive(true);
            } 

            SetPageNumberUI();
        }

        private void SetPageNumberUI()
        {
            _pageCounter.text = (_currentPageIndex+1).ToString() + "/" + _totalPages.ToString();
        }
    }
}
