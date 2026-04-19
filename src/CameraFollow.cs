using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float yOffset;
    public float zOffset;

    private Vector3 shakeOffset;

    private float shakeTime;
    private float shakeMagnitude;

    void LateUpdate()
    {
        Vector3 basePosition = new Vector3(
            0f,
            target.position.y + yOffset,
            target.position.z - zOffset
        );

        transform.position = basePosition + shakeOffset;

        UpdateShake();
    }

    void UpdateShake()
    {
        if (shakeTime > 0)
        {
            shakeTime -= Time.deltaTime;

            shakeOffset = Random.insideUnitSphere * shakeMagnitude;
            shakeOffset.z = 0f;
        }
        else
        {
            shakeOffset = Vector3.zero;
        }
    }

    public void Shake(float duration, float magnitude)
    {
        shakeTime = duration;
        shakeMagnitude = magnitude;
    }
}