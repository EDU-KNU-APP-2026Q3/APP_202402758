using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public sealed class StarlightSafeArea : MonoBehaviour
{
    Rect lastArea;
    Vector2Int lastSize;

    void OnEnable() => Fit();
    void Update() => Fit();

    void Fit()
    {
        var size = new Vector2Int(Screen.width, Screen.height);
        Rect area = Screen.safeArea;
        if (size.x <= 0 || size.y <= 0 || area.width <= 0 || area.height <= 0)
            return;
        if (lastArea == area && lastSize == size)
            return;

        lastArea = area;
        lastSize = size;
        var bounds = (RectTransform)transform;
        var screen = (Vector2)size;
        bounds.anchorMin = area.min / screen;
        bounds.anchorMax = area.max / screen;
        bounds.offsetMin = bounds.offsetMax = Vector2.zero;
    }
}
