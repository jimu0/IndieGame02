using UITemplate;
using UnityEngine;

namespace File_jim.Script.BoxSkill.Skill
{
    public class Skill6SpringPad : IBoxSkill
    {
        private IBoxSkill skillImplementation;

        public void OnCreate(Block block) { }
        public void OnMoveEnd(Block block) { }
        public void OnDestroy(Block block) { }

        public void OnBeEncroach(Block block, Chessboard chessboard, int intruderID)
        {
                        
            Vector3Int posU = block.objPos;
            posU.y = block.objPos.y + 1;
            int idU = ChessboardSys.Instance.GetMatrixValue(posU.x, posU.y, posU.z);
            if (idU is 0 or > 2000000000)
            {
                
            }
            else if (idU == 10)
            {
                chessboard.player.AddForce(Vector3.up, 43, ForceMode.Impulse, true);
                AudioManager.instance.Play("jump2");

            }
            else
            {
                //Debug.Log($"{idU}");
                //Block target = chessboard.objsDic[intruderID];
            }

            //rb.AddForce(Vector3.up * 10, ForceMode.Impulse);
        }

        public void OnPassive(Block block, Chessboard chessboard)
        {

        }
        public void OnEveryTurn(Block block, Chessboard chessboard) { }
    }
}
