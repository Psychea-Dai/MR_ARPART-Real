using UnityEngine;
using TMPro;

// 管理计分和胜负判定
public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshPro scoreText;
    public TextMeshPro liveText;
    public TextMeshPro messageText;// 3D消息文字

    [Header("设置")]
    public int totalEnemies = 23;    // 需消灭总数（20普通+Boss+余量）

    private int currentScore = 0;
    private int killedCount = 0;

    public void AddScore(int points)
    {
        currentScore += points;
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + currentScore;
        }
    }

    public void OnEnemyKilled()
    {
        killedCount++;
        if (killedCount >= totalEnemies)
        {
            if (messageText != null)
            {
                messageText.text = "RIFT SEALED!\nScore: " + currentScore;
                // \n是换行符
            }
            // 关掉SpawnManager停止生成
            SpawnManager sm = FindFirstObjectByType<SpawnManager>();
            if (sm != null) sm.enabled = false;
        }
    }
}