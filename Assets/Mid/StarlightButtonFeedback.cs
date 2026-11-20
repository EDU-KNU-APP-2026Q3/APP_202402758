using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public sealed class StarlightButtonFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    StarlightButtonEffects effects;
    Button button;
    RectTransform art;
    Vector3 restingScale;
    bool hovered;
    float lift = 1;

    public void Bind(StarlightButtonEffects host, Button target)
    {
        effects = host;
        button = target;
        var graphic = button.targetGraphic;
        if (graphic != null && graphic.transform != transform)
        {
            art = graphic.rectTransform;
            restingScale = art.localScale;
        }
    }

    public void OnPointerEnter(PointerEventData data)
    {
        hovered = data is not ExtendedPointerEventData input || input.pointerType != UIPointerType.Touch;
    }

    public void OnPointerExit(PointerEventData data) => hovered = false;

    public void OnPointerDown(PointerEventData data)
    {
        if (button.IsInteractable() && data.button == PointerEventData.InputButton.Left)
            effects.Show(data.position, data.pressEventCamera);
    }

    void Update()
    {
        if (art == null)
            return;

        float target = hovered && button.IsInteractable() ? 1.035f : 1;
        if (lift == target)
            return;

        lift = Mathf.MoveTowards(lift, target, 0.45f * Time.unscaledDeltaTime);
        art.localScale = restingScale * lift;
    }

    void OnDisable()
    {
        hovered = false;
        lift = 1;
        if (art != null)
            art.localScale = restingScale;
    }
}
