using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Fart : MonoBehaviour {

    [SerializeField] float fadeSpeed = 0.1f;
    [SerializeField] float growSpeed = 0.01f;
    SpriteRenderer spriteRenderer;


    void Start()
    {
        spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        GrowObject();
        FadeSprite();
    }

    void GrowObject ()
    {
        float currentScale = this.transform.localScale.x;
        float newScale = currentScale + growSpeed;
        this.transform.localScale = newScale * Vector3.one;
    }

    void FadeSprite ()
    {
        float spriteAlpha = spriteRenderer.color.a;
        spriteAlpha -= fadeSpeed;

        if (spriteAlpha <= 0) { Destroy(this.gameObject); }

        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteAlpha);
    }
}
