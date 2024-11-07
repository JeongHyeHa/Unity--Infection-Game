using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class GraphManager : MonoBehaviour
{
    public Transform graphContainer;    // 그래프를 그릴 부모 객체
    public Toggle totalToggle, doctorToggle, nurseToggle, inpatientToggle, outpatientToggle, emergencyToggle;

    Dictionary<string, List<GameObject>> graphLines = new Dictionary<string, List<GameObject>>();

    void Start()
    {
        graphContainer = GameObject.Find("graphContainer").transform;
        totalToggle = GameObject.Find("TotalToggle").GetComponent<Toggle>();
        doctorToggle = GameObject.Find("DoctorlToggle").GetComponent<Toggle>();
        nurseToggle = GameObject.Find("NurseToggle").GetComponent<Toggle>();
        inpatientToggle = GameObject.Find("InpatientToggle").GetComponent<Toggle>();
        outpatientToggle = GameObject.Find("OutpatientToggle").GetComponent<Toggle>();
        emergencyToggle = GameObject.Find("EmergencyToggle").GetComponent<Toggle>();

        // 토글별 그래프 리스트 초기화
        graphLines["total"] = new List<GameObject>();
        graphLines["doctor"] = new List<GameObject>();
        graphLines["nurse"] = new List<GameObject>();
        graphLines["inpatients"] = new List<GameObject>();
        graphLines["outpatients"] = new List<GameObject>();
        graphLines["emergencyPatients"] = new List<GameObject>();

        // 각 토글에 이벤트 리스너 추가
        totalToggle.onValueChanged.AddListener(isOn => ToggleGraphVisibility("total", isOn));
        doctorToggle.onValueChanged.AddListener(isOn => ToggleGraphVisibility("doctor", isOn));
        nurseToggle.onValueChanged.AddListener(isOn => ToggleGraphVisibility("nurse", isOn));
        inpatientToggle.onValueChanged.AddListener(isOn => ToggleGraphVisibility("inpatients", isOn));
        outpatientToggle.onValueChanged.AddListener(isOn => ToggleGraphVisibility("outpatients", isOn));
        emergencyToggle.onValueChanged.AddListener(isOn => ToggleGraphVisibility("emergencyPatients", isOn));
    }

    public void DrawGraph(List<float> scores, string role)
    {
        RectTransform graphRectTransform = graphContainer.GetComponent<RectTransform>();
        float graphWidth = graphRectTransform.sizeDelta.x;
        float graphHeight = graphRectTransform.sizeDelta.y;
        float yMax = 100f;                                   // y축 최댓값
        float xSpacing = graphWidth / (scores.Count - 1);    // 점 간의 x 간격 계산
        Vector2 previousPointPosition = Vector2.zero;

        // 점과 선 생성
        for (int i = 0; i < scores.Count; i++)
        {
            float xPosition = i * xSpacing;                         // x축 간격 
            float yPosition = (scores[i] / yMax) * graphHeight;     // 점의 y 위치
            Vector2 currentPointPosition = new Vector2(xPosition + graphWidth / 2 * (-1), yPosition + graphHeight / 2 * (-1));

            // 이전 점과 현재 점 사이에 선 그리기
            if (i > 0)
            {
                var line = CreateLine(previousPointPosition, currentPointPosition, role);
                graphLines[role].Add(line);
            }

            previousPointPosition = currentPointPosition;   // 이전 점 위치 업데이트
        }
    }

    // 선 생성하는 메소드
    GameObject CreateLine(Vector2 start, Vector2 end, string role)
    {
        GameObject linePrefab = Resources.Load<GameObject>($"Graph/{role}Line");
        GameObject line = Instantiate(linePrefab, graphContainer);
        RectTransform lineRectTransform = line.GetComponent<RectTransform>();

        Vector2 direction = (end - start).normalized;          // 선의 방향
        float distance = Vector2.Distance(start, end);         // 점 사이의 거리

        lineRectTransform.sizeDelta = new Vector2(distance, lineRectTransform.sizeDelta.y); // 선 길이 조정  * 1.5f
        lineRectTransform.anchoredPosition = start + (end - start) / 2;  // 두 점의 중앙에 선 배치

        // 회전 각도 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineRectTransform.rotation = Quaternion.Euler(0, 0, angle);

        return line;
    }

    // 토글 상태에 따라 그래프 라인 표시/숨김
    void ToggleGraphVisibility(string role, bool isVisible)
    {
        foreach (var line in graphLines[role])
        {
            line.SetActive(isVisible);
        }
    }
}
