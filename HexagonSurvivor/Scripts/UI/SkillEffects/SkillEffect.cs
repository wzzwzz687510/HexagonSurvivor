namespace HexagonUtils
{
    using UnityEngine;

    public abstract class SkillEffect : MonoBehaviour
    {
        GameObject _target;
        public Entity target
        {
            get { return _target != null ? _target.GetComponent<Entity>() : null; }
            set { _target = value != null ? value.gameObject : null; }
        }

        GameObject _caster;
        public Entity caster
        {
            get { return _caster != null ? _caster.GetComponent<Entity>() : null; }
            set { _caster = value != null ? value.gameObject : null; }
        }
    }
}