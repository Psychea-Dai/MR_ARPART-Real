using UnityEngine;

public class Spore : MonoBehaviour
{
    [Header("属性设置")]
    public int scoreValue = 10;
    public float moveSpeed = 0.3f;
    public float floatRange = 0.08f;
    public float floatSpeed = 2f;

    private Vector3 moveDirection;
    private float growTimer = 0f;
    private float growDuration = 1f;
    private Vector3 targetScale;
    private bool isGrowing = true;

    void Start()
    {
        targetScale = transform.localScale;
        transform.localScale = Vector3.zero;
        moveDirection = Random.onUnitSphere;
        moveDirection = moveDirection.normalized;
    }

    void Update()
    {
        if (isGrowing)
        {
            growTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(growTimer / growDuration);
            float smooth = Mathf.SmoothStep(0f, 1f, progress);
            transform.localScale = targetScale * smooth;
            if (progress >= 1f) isGrowing = false;
            return;
        }

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        float floatOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
        transform.position += Vector3.up * floatOffset * Time.deltaTime;
    }
}