using UnityEngine;

[CreateAssetMenu(fileName = "Thunder Strike Effect", menuName = "Data/Item effect/Thunder Strike")]
public class ThunderStrike_Effect : ItemEffect
{
    [SerializeField] private GameObject thunderStrikePrefabs;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        GameObject newThunderStrike = Instantiate(thunderStrikePrefabs, _enemyPosition.position, Quaternion.identity);

        Destroy(newThunderStrike, 0.5f);
    }
}
