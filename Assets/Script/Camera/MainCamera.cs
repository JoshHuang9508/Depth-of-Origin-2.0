using System.Collections;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [Header("Attributes")]
    public float smoothing;
    public Vector3 offset = Vector3.zero;

    [Header("Status")]
    public bool isFollowing_x = true;
    public bool isFollowing_y = true;
    public Vector3 newPosition;

    [Header("References")]
    [SerializeField] private MainCamera mainCamera;
    [SerializeField] private Transform target;
    [SerializeField] private Collider2D mapBounds;

    static MainCamera globalMainCamera;

    private void Start()
    {
        newPosition = target.position;
        globalMainCamera = mainCamera;
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        mapBounds = GameObject.FindWithTag("WorldEdge")?.GetComponent<Collider2D>() ?? null;

        newPosition = Vector3.Lerp(transform.position, target.transform.position + offset, smoothing);

        if (mapBounds != null)
        {
            Vector2 targetPos_x = new(newPosition.x, 0);
            Vector2 targetPos_y = new(0, newPosition.y);

            isFollowing_x = mapBounds.bounds.Contains(targetPos_x);
            isFollowing_y = mapBounds.bounds.Contains(targetPos_y);

            transform.position = new Vector3(
                isFollowing_x ? newPosition.x : transform.position.x,
                isFollowing_y ? newPosition.y : transform.position.y,
                -10
            );
        }
        else
        {
            transform.position = newPosition;
        }
    }

    public static IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = globalMainCamera.transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            globalMainCamera.transform.localPosition = originalPos + new Vector3(Random.Range(-1f, 1f) * magnitude, Random.Range(-1f, 1f) * magnitude, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        globalMainCamera.transform.localPosition = originalPos;

        yield return null;
    }
}
