using System.Collections;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [Header("Attributes")]
    public float smoothing;
    public Vector3 offset = Vector3.zero;

    [Header("References")]
    [SerializeField] private MainCamera mainCamera;
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody2D currentRb;

    // Static instance
    static MainCamera globalMainCamera;

    private void Start()
    {
        globalMainCamera = mainCamera;
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        // Vector3 newPosition = Vector3.Lerp(transform.position, target.transform.position + offset, smoothing);
        Vector3 newPosition = target.transform.position + offset;

        currentRb.MovePosition(newPosition);
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
