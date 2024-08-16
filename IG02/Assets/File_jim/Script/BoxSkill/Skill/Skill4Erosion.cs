using UnityEngine;

namespace File_jim.Script.BoxSkill.Skill
{
    public class Skill4Erosion : IBoxSkill
    {
        private IBoxSkill skillImplementation;

        public void OnCreate(Block block) { }
        public void OnMoveEnd(Block block) { }
        public void OnDestroy(Block block) { }

        /// <summary>
        /// ��ʴ
        /// </summary>
        /// <param name="block">ʩ����</param>
        /// <param name="chessboard"></param>
        /// <param name="intruderID">������</param>
        public void OnBeEncroach(Block block,Chessboard chessboard, int intruderID)
        {
            if (intruderID == 10)
            {
                chessboard.player.SetPlayerHp(-1);
            }
            else if (intruderID/10000 == 10013)
            {
                
            }
            else
            {
                block.chessboard.objsDic[intruderID].SetHp(-1); //ʹ������Hp-1
            }

        }
        public void OnPassive(Block block, Chessboard chessboard) { }
        public void OnEveryTurn(Block block, Chessboard chessboard) { }
    }
}
