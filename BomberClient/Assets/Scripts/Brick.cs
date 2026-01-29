using UnityEngine;

public class Brick : MonoBehaviour
{
    public int x;
    public int y;

    Animator anim;
    BoxCollider2D col;

    bool destroyed = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();
    }

    public void DestroyBrick()
    {
        Debug.Log($"Destroy brick at ({x}, {y})");
        if (destroyed) return;
        destroyed = true;

        // TẮT COLLIDER 
        if (col != null)
            col.enabled = false;

        // PLAY ANIMATION
        if (anim != null)
            anim.Play("brick_destroy");
    }

    public void OnDestroyFinished()
    {
        Destroy(gameObject);
    }
}
