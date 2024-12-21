using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [Header("Attributes")]
    public float smoothing;
    public Vector3 offset = Vector3.zero;

    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody2D currentRb;

    // Static instance
    static MainCamera GMainCamera;

    private void Start()
    {
        GMainCamera = this;
        transform.position = target.position + offset;
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        currentRb.MovePosition(target.position + offset);
    }

    public static async Task Shake(float duration, float magnitude)
    {
        Vector3 originalPos = GMainCamera.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            GMainCamera.transform.localPosition = originalPos + new Vector3(Random.Range(-1f, 1f) * magnitude, Random.Range(-1f, 1f) * magnitude, originalPos.z);
            elapsed += Time.deltaTime;
            await Task.Yield();
        }

        GMainCamera.transform.localPosition = originalPos;
        await Task.Yield();
    }
}
