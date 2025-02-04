using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // ���a����
    public Vector3 offset = new Vector3(0, 5, -3); // ��v���P���⪺�Z��
    public float followSpeed = 5f; // ��v�����H�t��
    public float rotationSpeed = 5f; // ��v����V�t��

    void LateUpdate()
    {
        if (target != null)
        {
            // ���Ʋ�����v����m
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

            // ������V���a
            Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
