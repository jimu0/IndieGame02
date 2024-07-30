using EditorPlus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Add
{
    [CreateAssetMenu(fileName = "New FlaotVar", menuName = "Adds/Float Var SO")]
    public class FloatVar : ScriptableObject
    {
        public float Value
        {
            get { return value; }
            set { Save(value); }
        }

        [ReadOnly]
        [SerializeField]private float value;

        public void Save(float v)
        {
            value = v;
        }
    }
}
