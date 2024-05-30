using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public Vector3 pointA; // 시작점
    public Vector3 pointB; // 끝점
    public float speed = 2.0f; // 왕복 속도

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 lastPosition;

    void Start()
    {
        startPosition = pointA;
        endPosition = pointB;
        lastPosition = transform.position;
    }

    void Update()
    {
        float time = Mathf.PingPong(Time.time * speed, 1);
        transform.position = Vector3.Lerp(startPosition, endPosition, time);
    }

    void FixedUpdate()
    {
        // 매 프레임마다 플랫폼의 이동 벡터를 계산
        Vector3 movement = transform.position - lastPosition;
        lastPosition = transform.position;

        // 충돌한 모든 플레이어를 찾음
        Collider[] hitColliders = Physics.OverlapBox(transform.position, transform.localScale / 2, transform.rotation);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                // 플레이어의 위치를 플랫폼의 이동 벡터만큼 이동
                hitCollider.transform.position += movement;
            }
        }
    }
}
