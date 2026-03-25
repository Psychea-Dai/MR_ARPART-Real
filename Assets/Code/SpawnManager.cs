using UnityEngine;
using System.Collections.Generic;

// 管理孢子生成和蝙蝠放置
public class SpawnManager : MonoBehaviour
{
    [Header("孢子Prefab（按等级顺序）")]
    public GameObject[] sporePrefabs;
    // Inspector中拖入3个：Element 0=Lv1, Element 1=Lv2, Element 2=Boss

    [Header("蝙蝠")]
    public GameObject batPrefab;     // 蝙蝠Prefab
    public int batCount = 3;         // 生成几只蝙蝠

    [Header("生成节奏")]
    public float startInterval = 2.5f;  // 初始生成间隔（秒）
    public float minInterval = 0.4f;    // 最快生成间隔
    public float speedUpRate = 0.93f;   // 每次生成后间隔缩短到原来的多少倍
    public int bossSpawnAfter = 20;     // 生成几个普通孢子后出Boss

    private float currentInterval;
    private float timer = 0f;
    private int spawnedCount = 0;
    private bool bossSpawned = false;
    private bool gameActive = false;
    private List<Transform> spawnSurfaces = new List<Transform>();
    // List是一个可以动态增减的列表

    void Start()
    {
        currentInterval = startInterval;
        // Invoke意思是"几秒后调用某个函数"
        // 等3秒再搜索表面，给OVRSceneManager足够的时间加载房间数据
        Invoke("FindSurfaces", 3f);
    }

    void FindSurfaces()
    {
        // 在场景中找到所有OVRScenePlane组件
        // 这些是SceneManager为每个检测到的墙壁、地板、天花板创建的
        OVRScenePlane[] planes =
            FindObjectsByType<OVRScenePlane>(FindObjectsSortMode.None);

        foreach (OVRScenePlane plane in planes)
        {
            // foreach循环遍历所有找到的表面，把每一个加入列表
            spawnSurfaces.Add(plane.transform);
        }

        if (spawnSurfaces.Count > 0)
        {
            gameActive = true;
            SpawnBats();   // 先放出蝙蝠
            Debug.Log("找到 " + spawnSurfaces.Count + " 个表面，开始！");
        }
        else
        {
            Debug.Log("未找到表面，1秒后重试...");
            Invoke("FindSurfaces", 1f);
        }
    }

    // 在房间内生成几只蝙蝠
    void SpawnBats()
    {
        if (batPrefab == null) return;

        for (int i = 0; i < batCount; i++)
        {
            // 在房间中央偏上的位置随机生成蝙蝠
            Vector3 batPos = new Vector3(
                Random.Range(-2f, 2f),  // 左右随机
                Random.Range(1.5f, 2.5f), // 高度1.5到2.5米
                Random.Range(-2f, 2f)   // 前后随机
            );
            Instantiate(batPrefab, batPos, Quaternion.identity);
            // Instantiate根据Prefab在指定位置创建一个新物体
        }
    }

    void Update()
    {
        if (!gameActive) return;
        if (bossSpawned) return;

        timer += Time.deltaTime;

        if (timer >= currentInterval)
        {
            timer = 0f;
            SpawnSpore();

            // 缩短下次生成间隔，但不低于最小值
            currentInterval = Mathf.Max(minInterval, currentInterval * speedUpRate);
            // 比如初始2.5秒，乘以0.93变成2.325秒，再乘变成2.16秒...越来越快
        }
    }

    void SpawnSpore()
    {
        if (spawnSurfaces.Count == 0) return;

        // 检查是否该出Boss
        if (spawnedCount >= bossSpawnAfter && !bossSpawned)
        {
            SpawnBoss();
            return;
        }

        // 根据已生成数量决定孢子等级
        int level = 0;
        if (spawnedCount < 12)
            level = 0;                              // 前12个是Lv1
        else
            level = 1;                              // 之后是Lv2

        // 防止数组越界（跳过最后的Boss）
        level = Mathf.Min(level, sporePrefabs.Length - 2);

        // 随机选一个表面
        Transform surface = spawnSurfaces[Random.Range(0, spawnSurfaces.Count)];
        // Random.Range(0, count)在0到count之间随机取一个整数

        // 在这个表面上随机取一个生成位置
        Vector3 spawnPos = GetRandomPointOnSurface(surface);

        Instantiate(sporePrefabs[level], spawnPos, Quaternion.identity);
        spawnedCount++;
    }

    void SpawnBoss()
    {
        bossSpawned = true;

        Transform surface = spawnSurfaces[Random.Range(0, spawnSurfaces.Count)];
        Vector3 spawnPos = surface.position + surface.forward * 0.5f;
        // 在表面前方0.5米处生成Boss

        // 数组最后一个元素是Boss
        Instantiate(sporePrefabs[sporePrefabs.Length - 1], spawnPos, Quaternion.identity);
        Debug.Log("母体孢子出现！");
    }

    // 在表面上随机取一个点
    Vector3 GetRandomPointOnSurface(Transform surface)
    {
        // surface.localScale包含了这个表面的宽高信息
        float halfW = surface.localScale.x * 0.35f; // 不取到最边缘
        float halfH = surface.localScale.y * 0.35f;

        float rX = Random.Range(-halfW, halfW);
        float rY = Random.Range(-halfH, halfH);

        // surface.position 表面中心点
        // surface.right 表面水平方向
        // surface.up 表面垂直方向
        // surface.forward 表面法线（垂直于表面朝外）
        Vector3 point = surface.position
            + surface.right * rX
            + surface.up * rY
            + surface.forward * 0.05f; // 偏离表面5cm，防止嵌入墙内

        return point;
    }
}