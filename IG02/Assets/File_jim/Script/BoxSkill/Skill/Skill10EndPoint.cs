
using UITemplate;
using UnityEngine;

namespace File_jim.Script.BoxSkill.Skill
{
    public class Skill10EndPoint : IBoxSkill
    {
        private IBoxSkill skillImplementation;

        public void OnCreate(Block block) { }
        public void OnMoveEnd(Block block) { }
        public void OnDestroy(Block block) { }

        public void OnBeEncroach(Block block, Chessboard chessboard, int intruderID) { }

        public void OnPassive(Block block, Chessboard chessboard)
        {
            Debug.Log("SL£¡");
            UIManager.Instance.completeLevel(true);
            //chessboard.DestroyObj(block.id);
        }

        public void OnEveryTurn(Block block, Chessboard chessboard) { }
    }
}
