
namespace File_jim.Script.BoxSkill
{
    public interface IBoxSkill
    {
        void OnCreate(Block block);
        void OnMoveEnd(Block block);
        void OnDestroy(Block block);
        void OnBeEncroach(Block block, int intruderID);
        void OnPassive(Block block,Chessboard chessboard);
        void OnEveryTurn(Block block);
    }
}
