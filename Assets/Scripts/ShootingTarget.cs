using UnityEngine;
using System.Collections;

public class ShootingTarget : MonoBehaviour
{
    public int health = 3;
    private Material mat;
    private Color originalColor;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        originalColor = mat.color;
    }

    public void TakeHit()
    {
        health--;
        StartCoroutine(FlashWhite());
        if (health <= 0) StartCoroutine(DestroyTarget());
    }

    IEnumerator FlashWhite()
    {
        mat.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        mat.color = originalColor;
    }

    IEnumerator DestroyTarget()
    {
        mat.color = Color.black;
        yield return new WaitForSeconds(0.3f);
        gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        health = 3;
        mat.color = originalColor;
        gameObject.SetActive(true);
    }
}