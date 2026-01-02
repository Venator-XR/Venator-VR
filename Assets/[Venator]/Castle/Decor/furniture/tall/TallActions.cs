using UnityEngine;

public class TallActions : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Move()
    {
        animator.SetTrigger("move");
    }
}
