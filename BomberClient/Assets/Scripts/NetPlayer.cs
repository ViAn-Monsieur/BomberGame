using System.Collections.Generic;
using UnityEngine;

public class NetPlayer : MonoBehaviour
{
    public int Id;

    Vector3 worldTarget;
    Animator anim;
    public List<AudioClip> Sounds;
    AudioSource audioSource;
    public bool isdead = false;
    void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        GetComponent<SpriteRenderer>().sortingOrder = 10;
    }
    
    public void Init(int id)
    {
        Id = id;
        worldTarget = transform.position;
    }


    public void SetPosition(int x, int y)
    {
        float ox = (MapLoader.Instance.Width - 1) / 2f;
        float oy = (MapLoader.Instance.Height - 1) / 2f;

        worldTarget = new Vector3(
            x - ox,
            oy - y,
            0
        );
    }


    void Update()
    {
        Vector3 delta = worldTarget - transform.position;

        bool moving = delta.magnitude > 0.01f;

        anim.SetBool("isMoving", moving);

        if (moving)
        {
            float dx = Mathf.Clamp(delta.x, -1f, 1f);
            float dy = Mathf.Clamp(delta.y, -1f, 1f);

            anim.SetFloat("dirX", dx);
            anim.SetFloat("dirY", dy);
        }

        transform.position = Vector3.Lerp(
            transform.position,
            worldTarget,
            Time.deltaTime * 12f
        );
        if (moving && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(Sounds[0]);
        }
    }
    public void Die()
    {
        if (isdead) return;
        isdead = true;

        if (audioSource && Sounds.Count > 0)
        {
            audioSource.PlayOneShot(Sounds[1]);
        }

        Destroy(gameObject, 1f);
    }
}
