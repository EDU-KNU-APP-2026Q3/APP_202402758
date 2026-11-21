using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public sealed class StarlightHitEffect : MonoBehaviour
{
    [SerializeField] Sprite ring;
    [SerializeField] Sprite shard;

    SpriteRenderer body;
    EnemyHealth enemy;
    PlayerHealth player;
    Color baseColor;
    Color flashColor;
    float flashLeft;
    const float FlashDuration = .16f;

    void Awake()
    {
        body = GetComponent<SpriteRenderer>();
        baseColor = body.color;
        TryGetComponent(out player);
        TryGetComponent(out enemy);
    }

    void OnEnable()
    {
        if (player != null) player.Damaged += Flash;
        if (enemy != null) enemy.Damaged += Flash;
    }

    void OnDisable()
    {
        if (player != null) player.Damaged -= Flash;
        if (enemy != null) enemy.Damaged -= Flash;
        flashLeft = 0;
        if (body != null) body.color = baseColor;
    }

    void Flash()
    {
        bool isPlayer = player != null;
        StarlightAudio.PlayImpact(isPlayer);
        flashLeft = FlashDuration;
        flashColor = isPlayer ? new Color32(255, 114, 122, 255) : new Color32(153, 222, 255, 255);
        body.color = flashColor;

        var burst = new GameObject("Starlight Hit Burst");
        burst.transform.position = body.bounds.center;
        burst.AddComponent<StarlightHitBurst>().Play(body, ring, shard, isPlayer);
    }

    void LateUpdate()
    {
        if (flashLeft <= 0)
            return;
        flashLeft = Mathf.Max(0, flashLeft - Time.deltaTime);
        body.color = Color.Lerp(baseColor, flashColor, flashLeft / FlashDuration);
    }
}
