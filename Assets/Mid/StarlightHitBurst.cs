using UnityEngine;

public sealed class StarlightHitBurst : MonoBehaviour
{
    const float Duration = .24f;
    const int ShardCount = 5;

    SpriteRenderer ring;
    readonly SpriteRenderer[] shards = new SpriteRenderer[ShardCount];
    readonly Vector2[] directions = new Vector2[ShardCount];
    Color color;
    float age;

    public void Play(SpriteRenderer body, Sprite ringArt, Sprite shardArt, bool player)
    {
        color = player ? new Color32(255, 124, 125, 255) : new Color32(123, 220, 255, 255);
        ring = NewSprite("Impact Ring", ringArt, body, 1);
        float offset = Random.Range(0f, 360f);
        for (int i = 0; i < ShardCount; i++)
        {
            float angle = offset + i * 360f / ShardCount;
            float radians = angle * Mathf.Deg2Rad;
            directions[i] = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
            shards[i] = NewSprite("Impact Shard", shardArt, body, 2);
            shards[i].transform.localRotation = Quaternion.Euler(0, 0, angle - 90f);
        }
        Draw(0);
    }

    SpriteRenderer NewSprite(string name, Sprite sprite, SpriteRenderer body, int order)
    {
        var art = new GameObject(name).AddComponent<SpriteRenderer>();
        art.transform.SetParent(transform, false);
        art.sprite = sprite;
        art.sortingLayerID = body.sortingLayerID;
        art.sortingOrder = body.sortingOrder + order;
        return art;
    }

    void Update()
    {
        age += Time.deltaTime;
        if (age >= Duration)
        {
            Destroy(gameObject);
            return;
        }
        Draw(age / Duration);
    }

    void Draw(float progress)
    {
        float spread = 1 - (1 - progress) * (1 - progress);
        Color tint = color;
        tint.a = .78f * (1 - progress);
        ring.color = tint;
        ring.transform.localScale = Vector3.one * Mathf.Lerp(.49f, 1.33f, spread);
        tint.a = 1 - progress * progress;
        for (int i = 0; i < shards.Length; i++)
        {
            shards[i].color = tint;
            shards[i].transform.localPosition = directions[i] * Mathf.Lerp(.2f, i % 2 == 0 ? .79f : .59f, spread);
            shards[i].transform.localScale = Vector3.one * Mathf.Lerp(.25f, .11f, progress);
        }
    }
}
