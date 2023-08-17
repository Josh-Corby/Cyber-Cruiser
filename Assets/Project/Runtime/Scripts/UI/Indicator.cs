using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    private float tweenTime = 2f;
    private Transform myTransform;
    private Tween myTween;
    [HideInInspector] public float timer;

    private void Start()
    {
        myTransform = transform;

        float distance = 1f;
        myTween = myTransform.DOLocalMove(myTransform.localPosition + (myTransform.up * distance), tweenTime).SetEase(Ease.InOutSine).SetLoops(-1);
        StartCoroutine(IndicatorTimer());
    }


    public IEnumerator IndicatorTimer()
    {
        yield return new WaitForSeconds(timer);
        myTween.Kill();
        Destroy(gameObject);
    }
}
