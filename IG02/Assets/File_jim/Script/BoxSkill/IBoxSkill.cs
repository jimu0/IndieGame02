
namespace File_jim.Script.BoxSkill
{
    public interface IBoxSkill
    {
        void OnCreate(Block block);
        void OnMoveEnd(Block block);
        void OnDestroy(Block block);
        void OnPassive(Block block);
        void OnEveryTurn(Block block);
    }
}
