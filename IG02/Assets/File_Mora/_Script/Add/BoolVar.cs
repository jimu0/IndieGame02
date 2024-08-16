using EditorPlus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Add
{
    [CreateAssetMenu(fileName = "New FlaotVar", menuName = "Adds/Bool Var SO")]
    public class BoolVar : ScriptableObject
    {
        public bool DefaultValue = false;
        public bool Value
        {
            get { return value; }
            set { Save(value); }
        }

        [ReadOnly]
        [SerializeField] private bool value;

        public void Save(bool v)
        {
            value = v;
        }
    }
}
