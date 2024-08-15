using UnityEngine;

namespace File_jim.Script.BoxSkill.Skill
{
    public class Skill7Soft : IBoxSkill
    {
        private IBoxSkill skillImplementation;

        public void OnCreate(Block block) { }
        public void OnMoveEnd(Block block) { }
        public void OnDestroy(Block block) { }
        public void OnBeEncroach(Block block, Chessboard chessboard, int intruderID) { }

        public void OnPassive(Block block, Chessboard chessboard)
        {
            
        }
        public void OnEveryTurn(Block block, Chessboard chessboard) { }
    }
}
