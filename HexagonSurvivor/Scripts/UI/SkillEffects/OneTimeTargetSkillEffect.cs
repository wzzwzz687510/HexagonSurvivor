namespace HexagonUtils {
    public class OneTimeTargetSkillEffect : SkillEffect
    {
        public int duration = 1;
        int endRound;

        void Start() { endRound = SystemManager._instance.battleManager.currentRound + duration; }

        void Update()
        {
            // follow the target's position (because we can't make a NetworkIdentity
            // a child of another NetworkIdentity)
            if (target != null)
                transform.position = target.collider.bounds.center;

            // destroy self if target disappeared or time elapsed
            if (target == null || SystemManager._instance.battleManager.currentRound > endRound)
                Destroy(gameObject);
        }
    }
}