using UnityEngine;

// 让蝙蝠在房间内随机飞行，纯氛围装饰
public class BatFly : MonoBehaviour
{
    public float flySpeed = 1.5f;      // 飞行速度
    public float changeInterval = 3f;  // 每隔几秒换一个飞行方向
    public float roomRadius = 3f;      // 活动范围半径（米）

    private Vector3 targetPoint;       // 当前飞向的目标点
    private Vector3 centerPoint;       // 活动范围的中心点
    private float timer = 0f;

    void Start()
    {
        // 以蝙蝠出生的位置为活动中心
        centerPoint = transform.position;
        PickNewTarget();
    }

    void Update()
    {
        // 朝目标点飞行
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint,
            flySpeed * Time.deltaTime
        );
        // MoveTowards：从当前位置向目标位置每帧移动一小步

        // 让蝙蝠面朝飞行方向
        Vector3 direction = targetPoint - transform.position;
        if (direction.magnitude > 0.1f)
        {
            // LookRotation把方向向量转换成旋转角度
            // 这样蝙蝠的正面就会朝着它飞行的方向
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * 3f
            );
            // Slerp做平滑旋转，不会突然转向，看起来更自然
        }

        // 定时换方向，或者快到目标了就换
        timer += Time.deltaTime;
        if (timer >= changeInterval ||
            Vector3.Distance(transform.position, targetPoint) < 0.3f)
        {
            // Distance计算两点之间的直线距离
            PickNewTarget();
            timer = 0f;
        }
    }

    // 在活动范围内随机选一个目标点
    void PickNewTarget()
    {
        // Random.insideUnitSphere返回一个半径为1的球体内的随机点
        // 乘以roomRadius扩大到你设定的活动范围
        targetPoint = centerPoint + Random.insideUnitSphere * roomRadius;

        // 限制高度在1米到2.5米之间（合理的室内飞行高度）
        targetPoint.y = Mathf.Clamp(targetPoint.y, 1f, 2.5f);
    }
}