using UnityEngine;
using UnityEngine.UI; //  UI 관련 라이브러리
using UnityEngine.SceneManagement; // 씬 관리 관련 라이브러리

public class GameManager : MonoBehaviour
{
    public GameObject gameoverText;
    public Text timeText;
    public Text recordText;

    private float surviveTime;
    private bool isGameover;
    void Start()
    {
        surviveTime = 0;
        isGameover = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameover)
        {
            surviveTime += Time.deltaTime;

            timeText.text = "Time: " + (int)surviveTime;
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("SampleScene");
            }
        }

    }

    public void EndGame()   //현재 게임을 게임오버 상태로 변경하는 메서드
    {
        isGameover = true;
        gameoverText.SetActive(true);

        float BestTime = PlayerPrefs.GetFloat("BestTime");
        if(surviveTime > BestTime)
        {
            BestTime = surviveTime;
            PlayerPrefs.SetFloat("BestTime", BestTime);
        }

        recordText.text = "Best Time: " + (int)BestTime;
    }
}
