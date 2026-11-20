using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class StarlightHealthBar : MonoBehaviour
{
    [SerializeField] PlayerHealth player;
    [SerializeField] TMP_FontAsset font;

    EnemyHealth enemy;
    RectTransform fill;
    Image fillImage;
    TMP_Text counter;
    int previousHealth = int.MinValue;
    int previousMax = int.MinValue;

    void Awake()
    {
        bool isPlayer = player != null;
        if (!isPlayer && !TryGetComponent(out enemy))
        {
            enabled = false;
            return;
        }

        RectTransform bar = MakeRect(isPlayer ? "Player Health HUD" : "Enemy Health HUD", transform);
        if (isPlayer)
        {
            bar.anchorMin = bar.anchorMax = new Vector2(.5f, 1);
            bar.anchoredPosition = new Vector2(0, -88);
            bar.sizeDelta = new Vector2(360, 82);
        }
        else
        {
            bar.localPosition = new Vector3(0, .59f, 0);
            bar.localScale = Vector3.one * .012f;
            bar.sizeDelta = new Vector2(88, 12);
            var canvas = bar.gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.overrideSorting = true;
            canvas.sortingOrder = 8;
        }

        Paint(bar, isPlayer ? new Color32(69, 126, 175, 255) : new Color32(88, 123, 162, 255));
        RectTransform background = MakeRect("Health Track", bar);
        Stretch(background);
        float border = isPlayer ? 3 : 1;
        background.offsetMin = new Vector2(border, border);
        background.offsetMax = new Vector2(-border, -border);
        Paint(background, new Color32(12, 27, 49, 255));

        RectTransform meter = background;
        if (isPlayer)
        {
            meter = MakeRect("Meter Track", background);
            meter.anchorMin = new Vector2(0, 0);
            meter.anchorMax = new Vector2(1, 0);
            meter.offsetMin = new Vector2(19, 13);
            meter.offsetMax = new Vector2(-19, 26);
            Paint(meter, new Color32(34, 70, 106, 255));

            RectTransform label = MakeRect("Health Value", background);
            label.anchorMin = new Vector2(0, 0);
            label.anchorMax = Vector2.one;
            label.offsetMin = new Vector2(12, 32);
            label.offsetMax = new Vector2(-12, -2);
            counter = label.gameObject.AddComponent<TextMeshProUGUI>();
            counter.font = font;
            counter.fontSize = 30;
            counter.color = new Color32(230, 245, 255, 255);
            counter.alignment = TextAlignmentOptions.Center;
            counter.raycastTarget = false;
            counter.textWrappingMode = TextWrappingModes.NoWrap;
        }

        fill = MakeRect("Health Fill", meter);
        Stretch(fill);
        fillImage = Paint(fill, Color.white);
    }

    void LateUpdate()
    {
        bool isPlayer = player != null;
        int max = isPlayer ? player.maxHealth : enemy.maxHealth;
        int health = isPlayer ? player.CurrentHealth : enemy.CurrentHealth;
        if (health == previousHealth && max == previousMax)
            return;

        previousHealth = health;
        previousMax = max;
        float ratio = Mathf.Clamp01((float)health / Mathf.Max(1, max));
        fill.anchorMax = new Vector2(ratio, 1);
        fillImage.color = isPlayer
            ? ratio > .3f ? new Color32(99, 212, 255, 255) : new Color32(255, 137, 126, 255)
            : new Color32(255, 135, 109, 255);
        if (counter != null)
            counter.text = $"HP  {Mathf.Max(0, health)} / {max}";
    }

    static RectTransform MakeRect(string name, Transform parent)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        return rect;
    }

    static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }

    static Image Paint(RectTransform rect, Color color)
    {
        var image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }
}
