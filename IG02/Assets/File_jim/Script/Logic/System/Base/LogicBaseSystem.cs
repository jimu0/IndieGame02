namespace File_jim.Scripts.Logic.System.Base
{
    public class LogicBaseSystem<T> where T : class, new()
    {
        #region //instance

        private static T instance;
        public static T Instance => instance ?? (instance = new T());

        #endregion
    }
}