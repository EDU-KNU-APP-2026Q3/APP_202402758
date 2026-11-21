using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public sealed class StarlightArenaBackground : MonoBehaviour
{
    Camera arenaCamera;
    SpriteRenderer art;
    float lastAspect;
    float lastSize;

    void Awake()
    {
        art = GetComponent<SpriteRenderer>();
        arenaCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (arenaCamera == null || art.sprite == null)
            return;

        Vector3 center = arenaCamera.transform.position;
        transform.position = new Vector3(center.x, center.y, transform.position.z);

        float aspect = arenaCamera.aspect;
        float size = arenaCamera.orthographicSize;
        if (aspect == lastAspect && size == lastSize)
            return;

        lastAspect = aspect;
        lastSize = size;
        Vector2 nativeSize = art.sprite.bounds.size;
        float scale = Mathf.Max(2 * size * aspect / nativeSize.x, 2 * size / nativeSize.y);
        transform.localScale = new Vector3(scale, scale, 1);
    }
}
