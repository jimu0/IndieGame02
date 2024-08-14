using UnityEngine;

namespace File_jim.Script.BoxSkill.Skill
{
    public class BlackHoleSkill : IBoxSkill
    {
        private IBoxSkill skillImplementation;

        public void OnCreate(Block block)
        {
            Debug.Log("Fire skill activated on block creation!");
            // Implement skill logic here
            
        }
        
        public void OnMoveEnd(Block block)
        {
            Debug.Log("Fire skill activated on block movement!");
            // Implement skill logic here
        }

        public void OnDestroy(Block block)
        {
            Debug.Log("Fire skill activated on block destruction!");
            // Implement skill logic here
        }

        public void OnBeEncroach(Block block,int intruderID)
        {
            //block.chessboard.objsDic[intruderID].boxAbi.Hp -= 1;
        }

        public void OnPassive(Block block,Chessboard chessboard)
        {
            Debug.Log("Fire skill passive effect activated!");
            // Implement skill logic here
        }

        public void OnEveryTurn(Block block)
        {
            Debug.Log("Fire skill activated every turn!");
            // Implement skill logic here
        }
    }
}
