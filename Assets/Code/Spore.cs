using UnityEngine;

// 控制单个孢子的行为：生长出现、漂浮移动、受伤、死亡
public class Spore : MonoBehaviour
{
    [Header("属性设置")]
    // public变量会显示在Inspector面板中，可以直接用鼠标调整数值
    public int health = 1;           // 生命值
    public int scoreValue = 10;      // 击杀奖励分数
    public float moveSpeed = 0.3f;   // 移动速度（米/秒）
    public float floatRange = 0.08f; // 上下漂浮幅度（米）
    public float floatSpeed = 2f;    // 上下漂浮速度

    // private变量不显示在Inspector中，只在脚本内部使用
    private Vector3 moveDirection;   // 移动方向
    private float growTimer = 0f;    // 生长计时器
    private float growDuration = 1f; // 生长动画持续时间（秒）
    private Vector3 targetScale;     // 最终大小
    private bool isGrowing = true;   // 是否还在生长阶段

    // Start()在这个孢子被创建时执行一次
    void Start()
    {
        // 记住最终大小，然后把当前大小设为零
        targetScale = transform.localScale;
        transform.localScale = Vector3.zero;
        // 这样做的效果是：孢子会从一个看不见的小点慢慢膨胀到完整大小

        // 随机生成一个移动方向
        // Random.onUnitSphere返回球面上一个随机方向
        moveDirection = Random.onUnitSphere;
        moveDirection = moveDirection.normalized;
        // normalized确保方向向量的长度为1，只表示方向，不影响速度
    }

    // Update()每帧执行一次（每秒大约72帧）
    void Update()
    {
        // === 生长阶段 ===
        if (isGrowing)
        {
            growTimer += Time.deltaTime;
            // Time.deltaTime是上一帧到这一帧经过的秒数
            // 累加起来就得到了总共过了多久

            // 计算生长进度（0到1之间）
            float progress = Mathf.Clamp01(growTimer / growDuration);
            // Clamp01确保值不会超过1也不会小于0

            // SmoothStep让变化曲线是S形的（先慢后快再慢），看起来更自然
            float smooth = Mathf.SmoothStep(0f, 1f, progress);
            transform.localScale = targetScale * smooth;

            if (progress >= 1f)
            {
                isGrowing = false; // 长完了，开始移动
            }
            return; // 生长期间不执行下面的移动代码
        }

        // === 移动阶段 ===
        // 沿moveDirection方向移动
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        // 速度乘以Time.deltaTime保证不同帧率下速度一致

        // 上下漂浮效果
        // Sin函数产生-1到1之间的持续波动
        float floatOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
        transform.position += Vector3.up * floatOffset * Time.deltaTime;
    }

    // 外部脚本调用这个函数来对孢子造成伤害
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
           // Die();
        }
        else
        {
            // 没死就闪白一下，告诉玩家"打中了"
            StartCoroutine(FlashWhite());
        }
    }

    // 协程：实现闪白效果
    // 协程可以在执行中途"暂停"，等一段时间后继续
    private System.Collections.IEnumerator FlashWhite()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            Color original = rend.material.color;
            rend.material.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            // yield return在这里暂停0.1秒
            rend.material.color = original;
        }
    }

    //void Die()
        // {
        // 找到场景中的GameManager，通知它加分
        //GameManager gm = FindFirstObjectByType<GameManager>();
        //if (gm != null)
       // {
            //gm.AddScore(scoreValue);
           // gm.OnEnemyKilled();
       //

        // Destroy销毁当前物体
        // gameObject（小写g）指"这个脚本挂载的那个物体"
        //Destroy(gameObject);
   // }
}