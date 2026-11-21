using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 사용자 흐름을 만들 때 버튼에 붙이는 컴포넌트.
/// 아직 작동하지 않는 화면(제목과 버튼만 있는 빈 화면)을 오가는 용도로 씁니다.
/// - 큰 이동(씬 → 씬): Scene Name에 씬 이름을 적고, Button OnClick에 GoToScene 연결
/// - 겹쳐 뜨는 화면(설정, 일시정지, 확인창): Panel에 패널을 넣고, OpenPanel / ClosePanel 연결
/// [주의] 시작 → 플레이 → GameOver는 기존 버튼(GameManager, SceneFlowManager)을 그대로 쓰세요.
///        점수 초기화와 기록 저장이 그 흐름에서 일어납니다.
/// [예외 케이스] 씬이 Build Profiles의 Scene List에 없으면 이동하지 않고 Console에 오류가 납니다.
/// </summary>
public class FlowLink : MonoBehaviour
{
    [SerializeField] string sceneName;
    [SerializeField] GameObject panel;

    public void GoToScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning($"[FlowLink] {name}에 Scene Name이 비어 있습니다.");
            return;
        }
        SceneManager.LoadScene(sceneName);
    }

    public void OpenPanel()
    {
        if (panel != null) panel.SetActive(true);
    }

    public void ClosePanel()
    {
        if (panel != null) panel.SetActive(false);
    }
}