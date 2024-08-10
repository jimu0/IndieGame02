using System.Collections;
using System.Collections.Generic;
using File_jim.Script.BoxSkill.Skill;
using UnityEngine;

namespace File_jim.Script.BoxSkill
{
    public static class SkillFactory
    {
        public static IBoxSkill CreateSkill(int boxId)
        {
            switch (boxId)
            {
                case 1:
                    return new BlackHoleSkill();
                case 2:
                    return new MagmaSkill();
                case 3:
                    return new FragileSkill();
                // Add more cases for different skills

                // Return null for blocks with no skills
                default:
                    return null;
            }
        }
    }
}
