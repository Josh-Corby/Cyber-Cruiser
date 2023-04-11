using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberKrakenTentacle : GameBehaviour
{
    public Transform goalTransform;
    [SerializeField] private float speed;
    private void Update()
    {
        if(goalTransform != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, goalTransform.position.y), speed * Time.deltaTime);
        }
    }
}
