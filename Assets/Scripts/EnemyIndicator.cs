using UnityEngine;
using System.Collections;
using DG.Tweening;

public class EnemyIndicator : MonoBehaviour
{
    private float tweenTime = 1f;
    private Transform myTransform;

    private Tween myTween;

    private void Start()
    {
        myTransform = transform;

        float distance = 1f;
        myTween = myTransform.DOLocalMove(myTransform.localPosition + (myTransform.up * distance), tweenTime).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public IEnumerator IndicatorTimer(float time)
    {
        yield return new WaitForSeconds(time);
        myTween.Kill();
        Destroy(gameObject);
    }
}
