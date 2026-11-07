using UnityEngine;

/// <summary>Underglow neon: intensity and glow scale with the car's current speed.</summary>
public class NeonController : MonoBehaviour
{
    [SerializeField] Light[] neonLights;
    [SerializeField] Renderer[] neonStrips;             // emissive strip meshes under the body
    [SerializeField] Color slowColor = new Color(0f, 0.8f, 1f);     // cyan
    [SerializeField] Color fastColor = new Color(1f, 0.1f, 0.8f);   // magenta
    [SerializeField] Vector2 lightIntensity = new Vector2(1.5f, 10f);
    [SerializeField] Vector2 emissionIntensity = new Vector2(1.5f, 6f);   // HDR multiplier (feeds Bloom)
    [SerializeField] float pulseAmount = 0.1f;

    static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    Material[] stripMats;

    void Awake()
    {
        stripMats = new Material[neonStrips.Length];
        for (int i = 0; i < neonStrips.Length; i++)
        {
            stripMats[i] = neonStrips[i].material;      // instance
            stripMats[i].EnableKeyword("_EMISSION");
        }
    }

    void Update()
    {
        float s = GameManager.Instance.Speed01;
        float pulse = 1f + Mathf.Sin(Time.time * 8f) * pulseAmount * s;

        // Interpolate in HSV so the glow stays saturated. A plain RGB lerp from
        // cyan to magenta passes through pale blue, which Bloom blows out to white.
        Color.RGBToHSV(slowColor, out float h1, out float s1, out float v1);
        Color.RGBToHSV(fastColor, out float h2, out float s2, out float v2);
        Color c = Color.HSVToRGB(Mathf.Lerp(h1, h2, s), Mathf.Lerp(s1, s2, s), Mathf.Lerp(v1, v2, s));

        float li = Mathf.Lerp(lightIntensity.x, lightIntensity.y, s) * pulse;
        foreach (var l in neonLights) { l.color = c; l.intensity = li; }

        float ei = Mathf.Lerp(emissionIntensity.x, emissionIntensity.y, s) * pulse;
        foreach (var m in stripMats) m.SetColor(EmissionColor, c * ei);
    }
}
