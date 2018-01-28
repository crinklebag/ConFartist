using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Fart : MonoBehaviour {
    [Header ("Farticle Controls")]
    [SerializeField] float fadeSpeed = 0.1f;
    [SerializeField] float growSpeed = 0.01f;
    SpriteRenderer spriteRenderer;

    [Header ("Fart Sounds")] //lists to contain the sounds for both quiet and loud farts
    [SerializeField] List<AudioClip> quietFarts;
    [SerializeField] List<AudioClip> loudFarts;
    [SerializeField] AudioSource fartAudioSource;
    bool fartLoud = false;


    void Start()
    {
        spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
        PlayFart(fartLoud);
    }

    void Update()
    {
        GrowObject();
        FadeSprite();
    }

    void PlayFart (bool loud) 
    {
        if (loud)
        {
            fartAudioSource.clip = loudFarts[Random.Range(0, loudFarts.Count - 1)];
        }
        else
        {
            fartAudioSource.clip = quietFarts[Random.Range(0, quietFarts.Count - 1)];
        }

        fartAudioSource.Play();
    }

    public void SetVolume(bool passedVol)
    {
        fartLoud = passedVol;       
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.name.Contains("NPC"))
        {
            collision.GetComponentInChildren<NPCController>().SmellFart();
        }
    }
}
