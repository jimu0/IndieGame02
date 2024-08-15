using System.Collections;
using UITemplate;
using UnityEngine;

namespace File_jim.Script
{
    public class GameService:MonoBehaviour
    {
        private static int endPosId;
        private static PlayerController2 player;
        private static Chessboard chessboard;
        public static void SetEndPosId(int id)
        {
            endPosId = id;
        }

        private void OnEnable()
        {
            Chessboard.OnGameService += InspectCompLeteLevel;
            Chessboard.OnGameService += PlayerDeath;
        }
        

        private void OnDisable()
        {
            Chessboard.OnGameService -= InspectCompLeteLevel;
            Chessboard.OnGameService -= PlayerDeath;
        }
        
        private static void GameCompleteLevel(bool b)
        {
            UIManager.Instance.completeLevel(b);
        }

        IEnumerator GameOver()
        {
            yield return new WaitForSeconds(2.0f);
            UIManager.Instance.gameOver();
        }

        private static void InspectCompLeteLevel()
        {
            if (chessboard.playerOldPos == ChessboardSys.Instance.positions[endPosId])
            {
                GameCompleteLevel(true);
            }
        }
        
        private void PlayerDeath()
        {
            if(chessboard.playerDead) StartCoroutine(GameOver());
        }

    }
}
