using System.Collections;
using System.Collections.Generic;
using File_jim.Script.BoxSkill.Skill;
using UnityEngine;

namespace File_jim.Script.BoxSkill
{
    public static class SkillFactory
    {
        public static IBoxSkill CreateSkill(int skillId)
        {
            switch (skillId)
            {
                case 1:
                    return null;
                case 2:
                    return null;
                case 3:
                    return null;
                case 4:
                    return new ErosionSkill();
                case 5:
                    return new FragileSkill();
                case 6:
                    return new SpringPadSkill();
                // Return null for blocks with no skills
                default:
                    return null;
            }
        }
    }
}
