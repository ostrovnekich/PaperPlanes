using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 5f;

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + target.rotation * offset;

            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            Quaternion targetRotation = target.rotation;

            Vector3 eulerAngles = targetRotation.eulerAngles;
            eulerAngles.z = 0f;

            Quaternion constrainedRotation = Quaternion.Euler(eulerAngles);
            transform.rotation = Quaternion.Lerp(transform.rotation, constrainedRotation, smoothSpeed * Time.deltaTime);
        }
    }
}
