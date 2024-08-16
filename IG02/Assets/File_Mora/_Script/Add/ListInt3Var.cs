using EditorPlus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Add
{
    [CreateAssetMenu(fileName = "New ListInt3Var", menuName = "Adds/ListInt3Var SO")]
    public class ListInt3Var : ScriptableObject
    {
        [SerializeField] public int[,,] Value;
    }
}
