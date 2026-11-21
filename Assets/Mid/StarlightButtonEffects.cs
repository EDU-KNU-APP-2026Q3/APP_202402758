using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public sealed class StarlightButtonEffects : MonoBehaviour
{
    [SerializeField] Sprite ringSprite;
    [SerializeField] Sprite crystalSprite;

    RectTransform surface;
    const float Duration = .23f;

    void Awake()
    {
        surface = (RectTransform)transform;
        foreach (Button button in GetComponentsInChildren<Button>(true))
            button.gameObject.AddComponent<StarlightButtonFeedback>().Bind(this, button);
    }

    public void Show(Vector2 screenPoint, Camera eventCamera)
    {
        StarlightAudio.PlayButton();
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(surface, screenPoint, eventCamera, out Vector2 point))
            StartCoroutine(Ripple(point));
    }

    IEnumerator Ripple(Vector2 point)
    {
        Image ring = MakeImage("Press Ripple", ringSprite, point);
        Image crystal = MakeImage("Press Spark", crystalSprite, point);
        Color cyan = new Color32(127, 222, 255, 255);

        for (float age = 0; age < Duration; age += Time.unscaledDeltaTime)
        {
            float t = age / Duration;
            float expansion = 1 - (1 - t) * (1 - t);
            ring.rectTransform.sizeDelta = Vector2.one * Mathf.Lerp(34, 132, expansion);
            crystal.rectTransform.sizeDelta = Vector2.one * Mathf.Lerp(29, 9, t);
            ring.color = new Color(cyan.r, cyan.g, cyan.b, .95f * (1 - t));
            crystal.color = new Color(1, 1, 1, 1 - t);
            yield return null;
        }

        Destroy(ring.gameObject);
        Destroy(crystal.gameObject);
    }

    Image MakeImage(string name, Sprite sprite, Vector2 point)
    {
        var image = new GameObject(name, typeof(RectTransform)).AddComponent<Image>();
        image.transform.SetParent(surface, false);
        var rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = surface.pivot;
        rect.anchoredPosition = point;
        image.sprite = sprite;
        image.raycastTarget = false;
        return image;
    }
}
