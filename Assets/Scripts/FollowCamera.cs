using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 4f, -7f);
    public float smooth = 8f;
    public float lookHeight = 1.5f;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desired = target.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, desired, smooth * Time.deltaTime);

        Vector3 lookPos = target.position + Vector3.up * lookHeight;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(lookPos - transform.position),
            smooth * Time.deltaTime
        );
    }
}
