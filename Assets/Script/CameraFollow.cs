using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 玩家角色
    public Vector3 offset = new Vector3(0, 5, -3); // 攝影機與角色的距離
    public float followSpeed = 5f; // 攝影機跟隨速度
    public float rotationSpeed = 5f; // 攝影機轉向速度

    void LateUpdate()
    {
        if (target != null)
        {
            // 平滑移動攝影機位置
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

            // 平滑轉向玩家
            Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
