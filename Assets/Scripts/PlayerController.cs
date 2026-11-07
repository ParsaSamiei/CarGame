using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Input (New Input System)")]
    [SerializeField] InputActionAsset inputActions;

    [Header("Lanes")]
    [SerializeField] float laneWidth = 3.5f;
    [SerializeField] float laneChangeSharpness = 14f;

    [Header("Stats")]
    [SerializeField] int maxLives = 3;
    [SerializeField] float maxFuel = 100f;
    [SerializeField] float fuelDrainPerSecond = 2.5f;
    [SerializeField] float invulnerableTime = 1.5f;

    [Header("Feedback")]
    [SerializeField] Renderer[] bodyRenderers;        // blink on hit
    [SerializeField] ParticleSystem sparks;           // play on hit
    [SerializeField] ParticleSystem explosion;        // play on death

    public int Lives { get; private set; }
    public int MaxLives => maxLives;
    public float Fuel { get; private set; }
    public float Fuel01 => Fuel / maxFuel;

    int lane;                // -1, 0, +1
    bool invulnerable;
    InputAction leftAction, rightAction;

    void Awake()
    {
        Lives = maxLives;
        Fuel = maxFuel;
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        if (bodyRenderers == null || bodyRenderers.Length == 0)
         bodyRenderers = GetComponentsInChildren<Renderer>();
    }

    void OnEnable()
    {
        var map = inputActions.FindActionMap("Player", true);
        leftAction = map.FindAction("Left", true);
        rightAction = map.FindAction("Right", true);
        leftAction.performed += OnLeft;
        rightAction.performed += OnRight;
        map.Enable();
    }

    void OnDisable()
    {
        if (leftAction != null) leftAction.performed -= OnLeft;
        if (rightAction != null) rightAction.performed -= OnRight;
    }

    void OnLeft(InputAction.CallbackContext _) => ChangeLane(-1);
    void OnRight(InputAction.CallbackContext _) => ChangeLane(+1);

    void ChangeLane(int dir)
    {
        if (GameManager.Instance.State != GameState.Playing) return;
        lane = Mathf.Clamp(lane + dir, -1, 1);
    }

    void Update()
    {
        if (GameManager.Instance.State != GameState.Playing) return;

        // fuel
        Fuel -= fuelDrainPerSecond * Time.deltaTime;
        if (Fuel <= 0f) { Fuel = 0f; Die(false); return; }

        // smooth lane movement + tilt
        float targetX = lane * laneWidth;
        Vector3 p = transform.position;
        p.x = Mathf.Lerp(p.x, targetX, 1f - Mathf.Exp(-laneChangeSharpness * Time.deltaTime));
        transform.position = p;
        float dx = targetX - p.x;
        transform.rotation = Quaternion.Euler(0f, dx * 10f, -dx * 4f);
    }

    void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponentInParent<Interactable>();
        if (item != null) item.OnPlayerHit(this);
    }

    // ---------- called by Interactables ----------
    public void TakeHit(Vector3 hitPoint)
    {
        if (invulnerable || GameManager.Instance.State != GameState.Playing) return;

        Lives--;
        AudioManager.Instance.PlayCrash();
        CameraFollow.Instance.Shake(0.35f, 0.45f);
        if (sparks != null)
        {
            sparks.transform.position = hitPoint + Vector3.up * 0.5f;
            sparks.Play();
        }

        if (Lives <= 0) { Die(true); return; }
        StartCoroutine(InvulnerabilityRoutine());
    }

    public void AddFuel(float amount) => Fuel = Mathf.Min(maxFuel, Fuel + amount);

    void Die(bool crashed)
    {
        StopAllCoroutines();
        if (crashed)
        {
            SetVisible(false);
            if (explosion != null) explosion.Play();
        }
        GameManager.Instance.GameOver();
    }

    IEnumerator InvulnerabilityRoutine()
    {
        invulnerable = true;
        float t = 0f;
        bool visible = true;
        while (t < invulnerableTime)
        {
            visible = !visible;
            SetVisible(visible);
            yield return new WaitForSeconds(0.1f);
            t += 0.1f;
        }
        SetVisible(true);
        invulnerable = false;
    }

    void SetVisible(bool v)
    {
        foreach (var r in bodyRenderers) if (r != null) r.enabled = v;
    }
}
