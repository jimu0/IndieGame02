using System.Collections;
using UITemplate;
using UnityEngine;

namespace File_jim.Script
{
    public class GameService:MonoBehaviour
    {
        private int endPosId;
        private PlayerController2 player;
        public Chessboard chessboard;
        private static LevelUIManager levelUIManager;
        private GameObject scoreRoot;
        public void SetEndPosId(int id)
        {
            endPosId = id;
        }

        private void OnEnable()
        {
            
            levelUIManager = FindObjectOfType<LevelUIManager>();
            scoreRoot = levelUIManager.transform.GetChild(0).gameObject;
            //EmptyScore();
            
            StartCoroutine(DelayPublishingTheRulesOfTheGame());
        }

        IEnumerator DelayPublishingTheRulesOfTheGame()
        {
            yield return new WaitForSeconds(3.0f);
            Chessboard.OnGameService += InspectCompLeteLevel;
            Chessboard.OnGameService += PlayerDeath;
            Block.OnGameStar += AddStar;
            Block.OnGameScore += AddScore;
            if (player == null)
            {
                player = FindObjectOfType<PlayerController2>();
            }
            player.playerLock = true;
            scoreRoot.gameObject.SetActive(player.playerLock);
            
        }
        

        private void OnDisable()
        {
            Chessboard.OnGameService -= InspectCompLeteLevel;
            Chessboard.OnGameService -= PlayerDeath;
        }
        
        private void GameCompleteLevel(bool b)
        {
            UIManager.Instance.completeLevel(b);
        }

        IEnumerator GameOver()
        {
            yield return new WaitForSeconds(2.0f);
            UIManager.Instance.gameOver();
        }

        private void InspectCompLeteLevel()
        {
            if (endPosId == 0) return;
            if (chessboard.playerOldPos == ChessboardSys.Instance.positions[endPosId])
            {
                if (player.playerLock) chessboard.GenerateNewEffect(chessboard.fx_end,chessboard.playerOldPos);
                if (player.playerLock)AudioManager.instance.Play("dead2");
                player.playerLock = false;
                GameCompleteLevel(true);
                scoreRoot.gameObject.SetActive(player.playerLock);
            }
        }
        
        private void PlayerDeath()
        {
            
            if (chessboard.playerDead)
            {
                player.playerLock = false;
                StartCoroutine(GameOver());
                scoreRoot.gameObject.SetActive(player.playerLock);
                AudioManager.instance.Play("dead");
            }
        }


        /// <summary>
        /// 加星星
        /// </summary>
        public void AddStar(int id)
        {
            //SaveStarStatus(chessboard.mapDataFileName,id);
            levelUIManager.SetStarBool(chessboard.levelId,true);
            levelUIManager.OnSetStarValue(1);
            AudioManager.instance.Play("star");
        }
        /// <summary>
        /// 加积分
        /// </summary>
        public void AddScore(int v)
        {
            levelUIManager.OnSetScoreValue(10*(20 + chessboard.levelId));
        }
        /// <summary>
        /// 清空积分
        /// </summary>
        private void EmptyScore()
        {
            levelUIManager.OnSetScoreValueEmpty();
        }


        private void SaveStarStatus(string dataName,int starId)
        {
            PlayerPrefs.SetInt(dataName, starId/10000);
            PlayerPrefs.Save();
        }

        public static bool LoadStarStatus(int l)
        {
            return levelUIManager.GetStarBool(l);
        }
    }
}
