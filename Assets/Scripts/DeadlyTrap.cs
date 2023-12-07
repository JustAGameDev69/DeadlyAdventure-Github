using UnityEngine;

public class DeadlyTrap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<CharacterStat>() != null)
        {
            collision.GetComponent<CharacterStat>().killEntity();
        }
    }
}
