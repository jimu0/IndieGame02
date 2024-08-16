using UnityEngine;

namespace File_jim.Script.BoxSkill.Skill
{
    public class Skill5Fragile : IBoxSkill
    {
        private IBoxSkill skillImplementation;

        public void OnCreate(Block block) { }

        public void OnMoveEnd(Block block)
        {
            Vector3Int posD = block.objPos;
            posD.y = block.objPos.y - 1;
            int idD = ChessboardSys.Instance.GetMatrixValue(posD.x, posD.y, posD.z);
            //Debug.Log($"{idD},{idD/10000}");
            if (idD / 10000 == 10010 || idD / 10000 == 10011)
            {
                return;
            }

            if (idD == 0)
            {
            }
            else if (idD == 10)
            {
            }
            // else if (idD / 10000 == 10010 || idD / 10000 == 10011)
            // {
            // }
            // else if (idD > 2000000000)
            // {
            //     //落到了边界
            //     block.chessboard.objsDic[block.id].SetHp(-1); //使自身Hp-1
            // }
            else
            {
                block.chessboard.objsDic[block.id].SetHp(-1); //使自身Hp-1
            }
        }

        public void OnDestroy(Block block) { }

        public void OnBeEncroach(Block block, Chessboard chessboard, int intruderID) { }

        public void OnPassive(Block block, Chessboard chessboard) { }

        public void OnEveryTurn(Block block, Chessboard chessboard)
        {
        }
    }
}
