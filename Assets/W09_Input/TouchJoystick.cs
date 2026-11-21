using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 화면을 누른 채 끌면 그 방향으로 플레이어가 움직이는 가상 조이스틱.
/// 방향(-1~1)을 PlayerController.SetMoveInput()에 넘겨줌. 키보드 입력은 그대로 동작함.
///
/// 붙이는 곳: 조이스틱 배경 Image(동그라미). 손잡이 Image는 배경의 자식으로 둠.
/// 배경과 손잡이의 Pivot은 가운데(0.5, 0.5), 손잡이의 Anchor도 가운데.
/// 배경 Image의 Raycast Target은 켜 둠 (눌림을 받는 곳).
///
/// [예외] 플레이어가 아직 없으면 아무 일도 하지 않음
/// [예외] 누른 채로 씬이 바뀌거나 조이스틱이 꺼지면 멈춤 상태로 돌려놓음
/// </summary>
public class TouchJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Tooltip("손잡이 Image (배경의 자식)")]
    public RectTransform handle;

    [Tooltip("손잡이가 가운데에서 움직일 수 있는 거리 (기준 해상도 단위). 보통 배경 지름의 절반 정도")]
    public float handleRange = 100f;

    [Tooltip("켜면 PC(브라우저·데스크톱 앱)에서는 조이스틱을 숨김. 에디터에서는 항상 보임")]
    public bool hideOnDesktop = false;

    RectTransform _background;
    PlayerController _player;

    void Awake()
    {
        _background = (RectTransform)transform;
    }

    void Start()
    {
        // Application.isMobilePlatform: Android 앱과 휴대폰 브라우저에서 true, PC에서 false
        if (hideOnDesktop && !Application.isMobilePlatform && !Application.isEditor)
            gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);

    public void OnDrag(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _background, eventData.position, eventData.pressEventCamera, out Vector2 local))
            return;

        Vector2 clamped = Vector2.ClampMagnitude(local, handleRange);
        if (handle != null) handle.anchoredPosition = clamped;

        if (_player == null) _player = FindFirstObjectByType<PlayerController>();
        if (_player != null) _player.SetMoveInput(clamped / handleRange);
    }

    public void OnPointerUp(PointerEventData eventData) => Release();

    void OnDisable() => Release();

    void Release()
    {
        if (handle != null) handle.anchoredPosition = Vector2.zero;
        if (_player != null) _player.SetMoveInput(Vector2.zero);
    }
}