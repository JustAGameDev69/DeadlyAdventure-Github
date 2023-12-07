public class Enemy_DeathBringerTrigger : EnemyAnimationTrigger
{

    private Enemy_DeathBringer enemyDeathBringer => GetComponentInParent<Enemy_DeathBringer>();
    private void Relocate() => enemyDeathBringer.FindPosition();
    private void MakeInvisible() => enemyDeathBringer.fx.MakeTransprent(true);
    private void MakeVisible() => enemyDeathBringer.fx.MakeTransprent(false);

}
