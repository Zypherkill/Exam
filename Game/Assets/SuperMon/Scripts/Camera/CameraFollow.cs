using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 5f;

    private float fixedY;
    private float fixedZ;

    void Start()
    {
        fixedY = transform.position.y;
        fixedZ = transform.position.z;
    }

    void LateUpdate()
    {
        float targetX = Mathf.Lerp(transform.position.x, player.position.x, smoothSpeed * Time.deltaTime);
        transform.position = new Vector3(targetX, fixedY, fixedZ);
    }
}
