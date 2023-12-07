using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator animator;
    public string id;
    public bool activeStatus;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    [ContextMenu("Generate checkpoint id")]
    private void GenerateId()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            ActiveCheckpoint();
        }
    }

    public void ActiveCheckpoint()
    {
        if (!activeStatus)
            AudioManager.instance.PlaySFX(5, transform);

        activeStatus = true;
        animator.SetBool("active", true);
    }
}
