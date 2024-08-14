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
        /// ��ʴ
        /// </summary>
        /// <param name="block">ʩ����</param>
        /// <param name="intruderID">������</param>
        public void OnBeEncroach(Block block,int intruderID)
        {
            block.chessboard.objsDic[intruderID].SetHp(-1); //ʹ������Hp-1
        }
        public void OnPassive(Block block,Chessboard chessboard) { }
        public void OnEveryTurn(Block block) { }
    }
}
