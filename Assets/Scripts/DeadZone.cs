using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<CharacterStat>() != null)
        {
            collision.GetComponent<CharacterStat>().killEntity();
        }
        else
            Destroy(collision.gameObject);
    }
}
