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
                    return new Skill3Star();
                case 4:
                    return new Skill4Erosion();
                case 5:
                    return new Skill5Fragile();
                case 6:
                    return new Skill6SpringPad();
                case 7:
                    return new Skill7Soft();
                case 8:
                    return null;
                case 9:
                    return new Skill9StartPoint();
                case 10:
                    return new Skill10EndPoint();
                // Return null for blocks with no skills
                default:
                    return null;
            }
        }
    }
}
