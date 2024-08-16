
using UITemplate;
using UnityEngine;

namespace File_jim.Script.BoxSkill.Skill
{
    public class Skill3Star : IBoxSkill
    {
        private IBoxSkill skillImplementation;

        public void OnCreate(Block block) { }
        public void OnMoveEnd(Block block) { }

        public void OnDestroy(Block block)
        {
        }

        public void OnBeEncroach(Block block, Chessboard chessboard, int intruderID) { }

        public void OnPassive(Block block, Chessboard chessboard)
        {
            block.GameStar(block.id);
            block.chessboard.objsDic[block.id].SetHp(-999999); // π◊‘…ÌHp-999999

        }

        public void OnEveryTurn(Block block, Chessboard chessboard) { }
    }
}
