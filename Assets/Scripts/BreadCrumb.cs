using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breadcrumb : MonoBehaviour
{
    [SerializeField] private float lifetime = 10f;
    [SerializeField] private float fadeDuration = 2f;

    private float timer;
    private Renderer rend;
    private Color originalColor;

    private void Start()
    {
        timer = lifetime;
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            originalColor = rend.material.color;
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= fadeDuration && rend != null)
        {
            float alpha = Mathf.Clamp01(timer / fadeDuration);
            Color fadedColor = originalColor;
            fadedColor.a = alpha;
            rend.material.color = fadedColor;
        }

        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }
}