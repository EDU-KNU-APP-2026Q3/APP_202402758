using UnityEngine;

/// <summary>
/// 플레이어가 카메라에 보이는 영역 밖으로 나가지 않게 막음.
/// 화면 비율이 바뀌면 보이는 영역도 바뀌므로, 매 프레임 카메라 크기와 비율로 경계를 다시 계산함.
///
/// 상하좌우 여백(%)으로 플레이어가 들어가지 못하는 띠를 둘 수 있음
/// → 점수 표시, 버튼, 가상 조이스틱 같은 UI를 놓을 자리를 확보
///
/// 붙이는 곳: PlayerController가 붙어 있는 오브젝트 (Player)
/// 전제: 카메라는 Tag가 MainCamera, Projection이 Orthographic이고 플레이어를 따라가지 않음
///
/// [예외] 위 전제가 맞지 않으면 Console에 한 번 경고를 남기고 아무 일도 하지 않음
/// [예외] 여백을 너무 크게 잡아 영역이 없어지면 플레이어를 가운데에 둠
/// Play 중 Scene 뷰에서 Player를 선택하면 노란 사각형으로 경계가 보임
/// </summary>
[DefaultExecutionOrder(100)] // PlayerController(기본 순서 0)가 속도를 정한 뒤에 위치를 바로잡음
public class StayInCameraView : MonoBehaviour
{
    [Header("화면 여백 (화면 크기의 %)")]
    [Tooltip("위쪽 여백. 예: 위에 점수 표시가 있으면 10")]
    [Range(0f, 50f)] public float marginTop = 0f;

    [Tooltip("아래쪽 여백. 예: 아래에 버튼이나 가상 조이스틱이 있으면 20")]
    [Range(0f, 50f)] public float marginBottom = 0f;

    [Tooltip("왼쪽 여백")]
    [Range(0f, 50f)] public float marginLeft = 0f;

    [Tooltip("오른쪽 여백")]
    [Range(0f, 50f)] public float marginRight = 0f;

    [Header("플레이어 크기")]
    [Tooltip("경계에서 더 띄울 거리 (월드 단위). 플레이어 그림의 절반 크기 정도")]
    public float padding = 0.5f;

    Rigidbody2D _rb;
    Camera _cam;
    bool _warned;
    Rect _bounds;
    bool _hasBounds;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null || GetComponent<PlayerController>() == null)
            Debug.LogWarning($"[StayInCameraView] '{name}'에 PlayerController/Rigidbody2D가 없음 → PlayerController가 붙은 오브젝트에 붙여야 함", this);
    }

    void FixedUpdate()
    {
        if (_rb == null) return;

        if (_cam == null) _cam = Camera.main;
        if (_cam == null)
        {
            WarnOnce("Tag가 MainCamera인 카메라가 없음 → GamePlay 씬 카메라의 Tag를 MainCamera로");
            return;
        }
        if (!_cam.orthographic)
        {
            WarnOnce($"카메라 '{_cam.name}'의 Projection이 Orthographic이 아님");
            return;
        }

        float height = _cam.orthographicSize * 2f;
        float width = height * _cam.aspect;
        Vector2 center = _cam.transform.position;

        float left = center.x - width / 2f;
        float bottom = center.y - height / 2f;

        float minX = left + width * marginLeft / 100f + padding;
        float maxX = left + width * (1f - marginRight / 100f) - padding;
        float minY = bottom + height * marginBottom / 100f + padding;
        float maxY = bottom + height * (1f - marginTop / 100f) - padding;

        // 여백이 너무 커서 영역이 없어지면 남은 영역의 가운데에 둠
        if (minX > maxX) minX = maxX = (minX + maxX) / 2f;
        if (minY > maxY) minY = maxY = (minY + maxY) / 2f;

        _bounds = Rect.MinMaxRect(minX, minY, maxX, maxY);
        _hasBounds = true;

        Vector2 position = _rb.position;
        Vector2 inside = new Vector2(Mathf.Clamp(position.x, minX, maxX), Mathf.Clamp(position.y, minY, maxY));

        // Correct an initial position outside the view, for example after resizing.
        if (inside != position)
            _rb.position = inside;

        // Limit velocity before the physics step so holding input against an edge
        // cannot push the body outside and snap it back every other frame.
        float step = Time.fixedDeltaTime;
        Vector2 velocity = _rb.linearVelocity;
        _rb.linearVelocity = new Vector2(
            Mathf.Clamp(velocity.x, (minX - inside.x) / step, (maxX - inside.x) / step),
            Mathf.Clamp(velocity.y, (minY - inside.y) / step, (maxY - inside.y) / step));
    }

    void WarnOnce(string message)
    {
        if (_warned) return;
        _warned = true;
        Debug.LogWarning("[StayInCameraView] " + message, this);
    }

    void OnDrawGizmosSelected()
    {
        if (!_hasBounds) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_bounds.center, _bounds.size);
    }
}