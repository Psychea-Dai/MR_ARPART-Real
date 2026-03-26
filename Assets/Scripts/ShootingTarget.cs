using UnityEngine;
using System.Collections;

public class ShootingTarget : MonoBehaviour
{
    public int health = 3;
    private Material mat;
    private Color originalColor;

    void Start()
    {
        // 先在自身找Renderer，找不到就在子物体中找
        Renderer rend = GetComponent<Renderer>();
        if (rend == null)
            rend = GetComponentInChildren<Renderer>();
        
        if (rend != null)
        {
            mat = rend.material;
            originalColor = mat.color;
        }

       
    }

    public void TakeHit()
    {
        health--;
        StartCoroutine(FlashWhite());
        if (health <= 0) StartCoroutine(DestroyTarget());
    }

    IEnumerator FlashWhite()
    {
        if (mat != null){
        mat.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        mat.color = originalColor;
        }
    }

    IEnumerator DestroyTarget()
    {
        if (mat != null)
            mat.color = Color.black;
        yield return new WaitForSeconds(0.3f);

        Spore spore = GetComponent<Spore>();
        if (spore != null)
        {
            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null)
            {
                gm.AddScore(spore.scoreValue);
                gm.OnEnemyKilled();
            }
        }

        Destroy(gameObject);
    }
}