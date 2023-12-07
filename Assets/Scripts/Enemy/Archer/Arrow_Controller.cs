using UnityEngine;

public class Arrow_Controller : MonoBehaviour
{
    [SerializeField] private string targetLayerName = "Player";
    [SerializeField] private float xVelocity;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private bool canMove;
    [SerializeField] private bool flipped;

    private CharacterStat myStat;

    private void Update()
    {
        if (canMove)
            rb.velocity = new Vector2(xVelocity, rb.velocity.y);
    }

    public void SetupArrow(float _speed, CharacterStat _myStat)
    {
        xVelocity = _speed;
        myStat = _myStat;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(targetLayerName))
        {
            myStat.DoDamage(collision.GetComponent<CharacterStat>());
            StuckInto(collision);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            StuckInto(collision);
    }

    private void StuckInto(Collider2D collision)
    {
        GetComponentInChildren<ParticleSystem>().Stop();
        GetComponent<CapsuleCollider2D>().enabled = false;
        canMove = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.parent = collision.transform;

        Destroy(gameObject, 5f);
    }

    public void FlipArrow()
    {
        if (flipped)
            return;

        xVelocity *= -1;
        flipped = true;
        transform.Rotate(0, 180, 0);
        targetLayerName = "Enemies";
    }
}
