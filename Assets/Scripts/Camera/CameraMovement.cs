using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject playerObject;

    void LateUpdate()
    {
        if (playerObject != null)
        {
            var playerTransform = playerObject.transform;

            var tempPos = transform.position;
            tempPos.x = playerTransform.position.x;
            tempPos.y = playerTransform.position.y;
            transform.position = tempPos;
        }
    }
}
