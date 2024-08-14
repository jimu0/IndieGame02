using UnityEngine;

namespace File_jim.Script.BoxSkill.Skill
{
    public class ErosionSkill : IBoxSkill
    {
        private IBoxSkill skillImplementation;

        public void OnCreate(Block block) { }
        public void OnMoveEnd(Block block) { }
        public void OnDestroy(Block block) { }
        /// <summary>
        /// 侵蚀
        /// </summary>
        /// <param name="block">施法者</param>
        /// <param name="intruderID">入侵者</param>
        public void OnBeEncroach(Block block,int intruderID)
        {
            block.chessboard.objsDic[intruderID].SetHp(-1); //使入侵者Hp-1
        }
        public void OnPassive(Block block,Chessboard chessboard) { }
        public void OnEveryTurn(Block block) { }
    }
}
