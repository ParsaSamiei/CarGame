using UnityEngine;

/// <summary>Third-person chase camera with speed-based FOV and shake.</summary>
[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(0f, 4.5f, -8f);
    [SerializeField] float xFollow = 0.6f;
    [SerializeField] float followSharpness = 8f;
    [SerializeField] Vector2 fovRange = new Vector2(60f, 78f);

    Camera cam;
    Vector3 basePos;
    float shakeTime, shakeDuration, shakeMagnitude;

    void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
        basePos = transform.position;
    }

    public void Shake(float duration, float magnitude)
    {
        shakeDuration = shakeTime = duration;
        shakeMagnitude = magnitude;
    }

    void LateUpdate()
    {
        Vector3 desired = new Vector3(target.position.x * xFollow, offset.y, offset.z);
        basePos = Vector3.Lerp(basePos, desired, 1f - Mathf.Exp(-followSharpness * Time.unscaledDeltaTime));

        Vector3 shake = Vector3.zero;
        if (shakeTime > 0f)
        {
            shake = Random.insideUnitSphere * (shakeMagnitude * (shakeTime / shakeDuration));
            shakeTime -= Time.unscaledDeltaTime;
        }
        transform.position = basePos + shake;

        Vector3 look = new Vector3(target.position.x * xFollow, 1.2f, 15f);
        transform.rotation = Quaternion.LookRotation(look - basePos);

        float targetFov = Mathf.Lerp(fovRange.x, fovRange.y, GameManager.Instance.Speed01);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, Time.unscaledDeltaTime * 2f);
    }
}
