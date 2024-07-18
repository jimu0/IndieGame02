using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;
using UnityEditor;

namespace UITemplate
{
    [ExecuteInEditMode]
    public class UIManager : MonoBehaviour
    {
        [Serializable]
        public class ButtonEvent : UnityEvent { }


        [Space(10)]
        [Header("Game Properties")]
        public bool hasLevels = true;
        public bool hasMiniGames = false;
        public bool useTextAsLogo = true;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(useTextAsLogo))]
        public string gameTitle = "Title";
        public bool autoManageLogo = true;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(autoManageLogo))]
        public Sprite gameLogoSprite;

        [Space(10)]
        [Header("Theme Settings")]
        public bool autoManageColors = true;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(autoManageColors))]
        public Color accentColor, panelColor, textColor, popUpBackgroundColor;
        public bool controlRoundCorners = true;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(controlRoundCorners))]
        [Range(0.5f, 10.0f)]
        public float cornerRadius = 2;
        public bool autoManageFonts = true;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(autoManageFonts))]
        public TMP_FontAsset mainFont;

        [Space(10)]
        [Header("Loading and Animation")]
        [Range(0.0f, 2.0f)]
        public float popupAnimationSpeed = 0.5f;
        [Range(0.0f, 2.0f)]
        public float fadeAnimationSpeed = 0.5f;
        public Color fadeBackgroundColor;
        public Sprite loadingSprite;
        [Range(-500f, 500f)]
        public float loadingIconSpeed;

        [Space(10)]
        [Header("Main Menu Pattern")]
        public bool useMainMenuPattern = false;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(useMainMenuPattern))]
        public Sprite patternSprite;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(useMainMenuPattern))]
        public Color patternColor;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(useMainMenuPattern))]
        [Range(0.2f, 10.0f)]
        public float patternSize = 2;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(useMainMenuPattern))]
        public bool patternAnimation = false;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(useMainMenuPattern),
    nameof(patternAnimation))]
        [Range(-3f, 3f)]
        public float xScrollSpeed = 1.0f;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(useMainMenuPattern),
    nameof(patternAnimation))]
        [Range(-3f, 3f)]
        public float yScrollSpeed = 1.0f;

        [Space(10)]
        [Header("Buttons")]
        public string playButtonLabel = "Play";
        public string playButtonScene = "Scene";

        public string miniGamesButtonLabel = "Mini Games";
        public bool hasOtherButton = false;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(hasOtherButton))]
        public string otherButtonLabel = "Shop";

        public bool useCustomButtonSprite = false;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(useCustomButtonSprite))]
        public Sprite buttonSprite;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(useCustomButtonSprite))]
        [Range(30, 200)]
        public float buttonHeight = 100.0f;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(useCustomButtonSprite))]
        [Range(20, 300)]
        public float buttonMinWidth = 50;

        public bool showAboutButton = true;
        public bool showTutorialButton = true;
        public bool showStoryButton = true;
        public bool showScoreButton = true;

        [Space(10)]
        [Header("Levels")]
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(hasLevels))]
        public string levelsPanelTitle = "Levels";
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(hasLevels))]
        public int numberOfLevels = 10;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(hasLevels))]
        public String levelNamesInScene = "";
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(hasLevels))]
        public Sprite levelImage;

        [Space(10)]
        [Header("Mini Games Settings")]
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(hasMiniGames))]
        public string miniGamesPanelTitle = "Mini Games";
        public List<buttons> miniGamesButtonsLists = new List<buttons>();

        [Space(10)]
        [Header("GameOver Titles")]
        public string[] levelCompleteTitles;
        public string[] levelLostTitles;
        public string[] gameOverTitles;

        [Space(40)]
        [Header("Prefabs")]
        public GameObject levelButtonsObj;
        public GameObject miniGamesButtonsObj;

        [Header("Sprites")]
        public Sprite soundOffSprite;
        public Sprite musicOffSprite, soundOnSprite, musicOnSprite;

        [Header("Components")]
        public Transform levelContent;
        public Transform miniGamesContent;
        public Image gameLogoImg, patternImage, soundImage, musicImage;
        public GameObject playButtonObj, miniGamesButtonObj, otherButtonObj;
        public GameObject scoreButtonObj, aboutButtonObj, tutorialButtonObj, storyButtonObj;
        public Image[] roundedCornersImages;
        public Image[] panelImages;
        public Image[] accentColorImages;
        public Image[] buttonIconImages;
        public Image[] buttonImages;
        public Image[] smallButtonImages;
        public TextMeshProUGUI[] allTexts;
        public Image blackPanel, loadingImage;
        public TextMeshProUGUI playButtonText, miniGamesButtonText, otherButtonText, levelIntroText, messagePopupText, gameOverText, pauseLevelStatus, gameTitleText;
        public TextMeshProUGUI levelsPanelTitleText, miniGamesPanelTitleText;
        public GameObject pauseButton, mainMenuResumeButton, mainMenuPanel;
        public GameObject levelsPanelObj, pausePanelObj, messagePopupObj, restartPanelObj, miniGamesPanelObj;
        public Image levelsBackgroundImage, pauseBackgroundImage, messagePopupBackgroundImage, levelIntroImage, restartBackgroundImage, miniGamesBackgroundImage;
        public RectTransform gameOverUI;
        public GameObject nextLevelButton;
        public GridLayoutGroup layoutGroupMenu, layoutGroupPause, layoutGroupMiniGames, LayoutGroupLevels;
        private List<Button> allLevelsButtons;
        private Material imageMaterial;
        private Vector2 offset = Vector2.zero;
        [HideInInspector]
        public int currentLevel = 0;
        [HideInInspector]
        public int currentMiniGame = 0;
        [HideInInspector]
        public bool isLevel = false;
        [HideInInspector]
        public bool finished = true;
        public static UIManager Instance { set; get; }
        public ScrollRect scrollView;
        void Awake()
        {
            if (Application.isPlaying)
            {
                if (Instance == null)
                {
                    Instance = this;
                    Init();
                }
                else
                {
                    Destroy(gameObject);
                    return;
                }
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Update()
        {
            if (Application.isPlaying && imageMaterial != null && patternAnimation)
            {
                // Calculate the new offsets based on time and scroll speed
                offset.x += xScrollSpeed * Time.unscaledDeltaTime;
                offset.y += yScrollSpeed * Time.unscaledDeltaTime;

                // Apply the offsets to the material
                imageMaterial.SetTextureOffset("_MainTex", offset);

                // If the offsets go beyond 1 or 0, reset them to create a looping effect
                if (offset.x >= 1)
                {
                    offset.x -= 1;
                }
                else if (offset.x <= 0)
                {
                    offset.x += 1;
                }

                if (offset.y >= 1)
                {
                    offset.y -= 1;
                }
                else if (offset.y <= 0)
                {
                    offset.y += 1;
                }
            }
        }

        void Init()
        {
            if (PlayerPrefs.GetInt("first", 0) == 0)
            {
                PlayerPrefs.SetInt("music", 1);
                PlayerPrefs.SetInt("sound", 1);
                PlayerPrefs.SetInt("first", 1);
            }
            Application.targetFrameRate = 300;
            Time.timeScale = 0;
            if (loadingSprite != null)
            {
                loadingImage.sprite = loadingSprite;
            }
            else
            {
                loadingImage?.gameObject.SetActive(false);
            }
            loadingImage.color = new Color(accentColor.r, accentColor.g, accentColor.b, 0);
            blackPanel.color = new Color(fadeBackgroundColor.r, fadeBackgroundColor.g, fadeBackgroundColor.b, 0);
            if (hasLevels)
            {
                populateLevels();
            }
            if (hasMiniGames)
            {
                populateMiniGames();
            }
            statusToggle();
            // Clone the material to ensure it's exclusive to this Image
            if (patternImage != null)
            {
                imageMaterial = new Material(patternImage.material);
                patternImage.material = imageMaterial;
            }
        }

        private void Start()
        {
            if (Application.isPlaying)
            {
                if (PlayerPrefs.GetInt("music") == 1)
                {
                    FindObjectOfType<AudioManager>().Play("music");
                }
            }
        }


        public void loadLevel(int level)
        {
            string sceneName = levelNamesInScene + level;
            currentLevel = level;
            isLevel = true;
            loadScene(sceneName);
        }

        public void loadGame(int id)
        {
            isLevel = false;
            currentMiniGame = id;
            loadScene(miniGamesButtonsLists[id].scene);
        }

        void loadScene(string name)
        {
            if (Application.CanStreamedLevelBeLoaded(name))
            {
                fadeStart();
                StartCoroutine(loadSceneNow(name));
            }
            else
            {
                messageAppear("This scene doesn't exist: " + name);
            }
        }

        IEnumerator loadSceneNow(string name)
        {
            yield return new WaitForSecondsRealtime(fadeAnimationSpeed + 0.05f);
            SceneManager.LoadSceneAsync(name).completed += (operation) =>
       {
           OnLevelLoadComplete(operation); // Pass the 'name' variable
       };
        }

        private void OnLevelLoadComplete(AsyncOperation operation)
        {
            closeLevelsFunction();
            StartCoroutine(ScaleTransform(pausePanelObj.transform, 0, 0));
            StartCoroutine(ChangeImageColor(pauseBackgroundImage, 0, Color.clear));
            pauseBackgroundImage.raycastTarget = false;
            StartCoroutine(MoveToPosition(gameOverUI, new Vector2(0, -233), 0));
            pauseButton.SetActive(true);
            mainMenuResumeButton.SetActive(true);
            if (isLevel)
            {
                levelIntro("Level " + currentLevel);
            }
            finished = false;
            StartCoroutine(ScaleTransform(gameOverText.transform, 0, 0));
            Time.timeScale = 1;
            mainMenuPanel.SetActive(false);
            removeFade();
        }

        Vector2 setButtonSize(float height)
        {
            // Calculate the aspect ratio of the sprite.
            float aspectRatio = buttonSprite.rect.width / buttonSprite.rect.height;

            // Calculate the width based on the desired height and aspect ratio.
            float desiredWidth = aspectRatio * height;

            // Apply the minimum width if desiredWidth is less than minWidth.
            if (desiredWidth < buttonMinWidth)
            {
                desiredWidth = buttonMinWidth;
                // Recalculate desiredHeight to maintain the aspect ratio.
                height = desiredWidth / aspectRatio;
            }

            // Set the size of the image while keeping the height constant.
            // RectTransform imageRectTransform = imageToAdjust.GetComponent<RectTransform>();
            // imageRectTransform.sizeDelta = new Vector2(desiredWidth, desiredHeight);

            return new Vector2(desiredWidth, height);

        }

        private void populateLevels()
        {
            allLevelsButtons = new List<Button>();
            for (int i = 0; i < numberOfLevels; i++)
            {
                GameObject go = Instantiate(levelButtonsObj, Vector3.zero, Quaternion.identity, levelContent);
                go.name = (i + 1).ToString();
                go.GetComponent<Image>().color = accentColor;
                if (levelImage != null)
                {
                    go.GetComponent<Image>().sprite = levelImage;
                }
                allLevelsButtons.Add(go.GetComponent<Button>());
            }
        }

        private void populateMiniGames()
        {
            for (int i = 0; i < miniGamesButtonsLists.Count; i++)
            {
                GameObject go = Instantiate(miniGamesButtonsObj, Vector3.zero, Quaternion.identity, miniGamesContent);
                go.name = miniGamesButtonsLists[i].name;
                go.GetComponent<MiniGamesButton>().id = i;
                if (miniGamesButtonsLists[i].color == Color.clear)
                {
                    go.GetComponent<Image>().color = accentColor;
                }
                else
                {
                    go.GetComponent<Image>().color = miniGamesButtonsLists[i].color;
                }
                if (controlRoundCorners)
                {
                    go.GetComponent<Image>().pixelsPerUnitMultiplier = cornerRadius;
                }
                go.GetComponent<MiniGamesButton>().sceneName = miniGamesButtonsLists[i].scene;
                if (buttonSprite != null)
                {
                    go.GetComponent<Image>().sprite = buttonSprite;
                }
            }
        }

        private void OnValidate()
        {
            otherButtonObj?.SetActive(hasOtherButton);
            miniGamesButtonObj?.SetActive(hasMiniGames);
            if (playButtonText != null)
                playButtonText.text = playButtonLabel;
            if (miniGamesButtonText != null)
                miniGamesButtonText.text = miniGamesButtonLabel;
            if (otherButtonText != null)
                otherButtonText.text = otherButtonLabel;

            storyButtonObj?.SetActive(showStoryButton);
            scoreButtonObj?.SetActive(showScoreButton);
            aboutButtonObj?.SetActive(showAboutButton);
            tutorialButtonObj?.SetActive(showTutorialButton);

            if (gameTitleText != null)
            {
                gameTitleText.gameObject.SetActive(useTextAsLogo);
                gameTitleText.text = gameTitle;
            }

            if (levelsPanelTitleText != null)
            {
                levelsPanelTitleText.text = levelsPanelTitle;
            }
            if (miniGamesPanelTitleText != null)
            {
                miniGamesPanelTitleText.text = miniGamesPanelTitle;
            }

            if (buttonSprite != null && useCustomButtonSprite)
            {
                if (layoutGroupMenu != null)
                    layoutGroupMenu.cellSize = setButtonSize(buttonHeight);
                if (layoutGroupPause != null)
                    layoutGroupPause.cellSize = setButtonSize(buttonHeight);
                if (layoutGroupMiniGames != null)
                    layoutGroupMiniGames.cellSize = setButtonSize(buttonHeight);
                for (int i = 0; i < smallButtonImages.Length; i++)
                {
                    if (smallButtonImages[i] != null)
                        smallButtonImages[i].rectTransform.sizeDelta = setButtonSize(50);
                }

                for (int i = 0; i < buttonImages.Length; i++)
                {
                    buttonImages[i].sprite = buttonSprite;
                }

                for (int i = 0; i < smallButtonImages.Length; i++)
                {
                    smallButtonImages[i].sprite = buttonSprite;
                }
            }
            if (autoManageLogo)
            {
                if (gameLogoSprite != null && gameLogoImg != null)
                {
                    gameLogoImg.gameObject.SetActive(!useTextAsLogo);
                    gameLogoImg.sprite = gameLogoSprite;
                    gameLogoImg.rectTransform.sizeDelta = new Vector2(450, 170);
                    gameLogoImg.preserveAspect = true;
                }
            }
            if (controlRoundCorners)
            {
                for (int i = 0; i < roundedCornersImages.Length; i++)
                {
                    if (roundedCornersImages[i] != null)
                        roundedCornersImages[i].pixelsPerUnitMultiplier = cornerRadius;

                }
            }
            if (autoManageColors)
            {
                for (int i = 0; i < panelImages.Length; i++)
                {
                    if (panelImages[i] != null)
                        panelImages[i].color = panelColor;
                }
                for (int i = 0; i < accentColorImages.Length; i++)
                {
                    if (accentColorImages[i] != null)
                        accentColorImages[i].color = accentColor;
                }
                for (int i = 0; i < allTexts.Length; i++)
                {
                    if (allTexts[i] != null)
                        allTexts[i].color = textColor;
                }
                for (int i = 0; i < buttonIconImages.Length; i++)
                {
                    if (buttonIconImages[i] != null)
                        buttonIconImages[i].color = textColor;
                }
            }
            if (autoManageFonts && mainFont != null)
            {
                for (int i = 0; i < allTexts.Length; i++)
                {
                    if (allTexts[i] != null)
                        allTexts[i].font = mainFont;
                }
            }
            patternImage?.gameObject.SetActive(useMainMenuPattern);
            if (useMainMenuPattern && patternImage != null)
            {
                patternImage.sprite = patternSprite;
                patternImage.color = patternColor;
                patternImage.pixelsPerUnitMultiplier = patternSize;
            }
        }

        public void completeLevel(bool won)
        {
            if (!finished)
            {
                if (won)
                {
                    gameOverText.text = levelCompleteTitles[UnityEngine.Random.Range(0, levelCompleteTitles.Length)];
                    PlayerPrefs.SetInt("level" + currentLevel.ToString(), 1);
                    nextLevelButton.SetActive(true);
                }
                else
                {
                    gameOverText.text = levelLostTitles[UnityEngine.Random.Range(0, levelLostTitles.Length)];
                    nextLevelButton.SetActive(false);
                }
                pauseButton.SetActive(false);
                StartCoroutine(MoveToPosition(gameOverUI, new Vector2(0, 0), popupAnimationSpeed));
                StartCoroutine(ScaleTransform(gameOverText.transform, popupAnimationSpeed, 1));
                finished = true;
            }
        }

        public void gameOver()
        {
            if (!finished)
            {
                pauseButton.SetActive(false);
                gameOverText.text = gameOverTitles[UnityEngine.Random.Range(0, gameOverTitles.Length)];
                StartCoroutine(ScaleTransform(gameOverText.transform, popupAnimationSpeed, 1));
                nextLevelButton.SetActive(false);
                StartCoroutine(MoveToPosition(gameOverUI, new Vector2(0, 0), popupAnimationSpeed));
                finished = true;
            }
        }

        public void checkLevelButtons()
        {
            int levels = 0;
            for (int i = 0; i < allLevelsButtons.Count; i++)
            {
                if (PlayerPrefs.GetInt("level" + (i).ToString()) == 1)
                {
                    allLevelsButtons[i].interactable = true;
                    levels++;
                }
                else
                {
                    allLevelsButtons[i].interactable = false;
                }
            }
            allLevelsButtons[0].interactable = true;
            ScrollToLevel(levels);

        }

        public void ScrollToLevel(int levelIndex)
        {
            if (levelIndex < 6)
            {
                scrollView.verticalNormalizedPosition = 1;
            }
            else
            {
                scrollView.verticalNormalizedPosition = 1.1f - (float)levelIndex / (allLevelsButtons.Count - 1);
            }
        }

        public void nextLevel()
        {
            clickSound();
            if (currentLevel < numberOfLevels)
            {
                currentLevel++;
                loadLevel(currentLevel);
            }
            else
            {
                mainMenuFunction();
            }
        }


        #region buttons Function

        public void playGameFunction()
        {
            clickSound();
            if (hasLevels)
            {
                //open levels panel
                openLevels();
            }
            else
            {
                isLevel = false;
                loadScene(playButtonScene);
            }
        }

        public void enterMiniGames()
        {
            clickSound();
            if (miniGamesButtonsLists.Count == 1)
            {
                loadGame(0);
            }
            else if (miniGamesButtonsLists.Count > 1)
            {
                openMiniGames();
            }
        }

        public void restartFunction()
        {
            backSound();
            fadeStart();
            closeRestartFunction();
            StartCoroutine(restartNow());
        }

        IEnumerator restartNow()
        {
            yield return new WaitForSecondsRealtime(fadeAnimationSpeed);
            if (isLevel)
            {
                loadLevel(currentLevel);
            }
            else
            {
                loadGame(currentMiniGame);
            }
        }

        public void mainMenuFunction()
        {
            clickSound();
            fadeStart();
            StartCoroutine(enableMainMenuAfter());
        }

        IEnumerator enableMainMenuAfter()
        {
            yield return new WaitForSecondsRealtime(fadeAnimationSpeed);
            mainMenuPanel.SetActive(true);
            removeFade();
        }

        public void pauseGameFunc()
        {
            clickSound();
            openPopupAnimation(pausePanelObj, pauseBackgroundImage);
            pauseButton.SetActive(false);
            Time.timeScale = 0;
            if (isLevel)
            {
                pauseLevelStatus.gameObject.SetActive(true);
                pauseLevelStatus.text = "Level " + currentLevel;
            }
            else
            {
                if (miniGamesButtonsLists.Count == 1)
                {
                    pauseLevelStatus.gameObject.SetActive(false);
                }
                else
                {
                    pauseLevelStatus.text = miniGamesButtonsLists[currentMiniGame].name;
                }
            }
        }

        public void resumeGameFunc()
        {
            clickSound();
            closePopupAnimation(pausePanelObj, pauseBackgroundImage);
            StartCoroutine(canPause());
        }

        IEnumerator canPause()
        {
            yield return new WaitForSecondsRealtime(popupAnimationSpeed + 0.05f);
            pauseButton.SetActive(true);
            Time.timeScale = 1;
        }

        public void mainMenuResume()
        {
            clickSound();
            fadeStart();
            closePopupAnimation(pausePanelObj, pauseBackgroundImage);
            StartCoroutine(disableMainMenuAfter());
        }

        IEnumerator disableMainMenuAfter()
        {
            yield return new WaitForSecondsRealtime(fadeAnimationSpeed);
            mainMenuPanel.SetActive(false);
            removeFade();
            pauseButton.SetActive(true);
            Time.timeScale = 1;
        }

        public void openRestart()
        {
            clickSound();
            openPopupAnimation(restartPanelObj, restartBackgroundImage);
        }

        public void closeRestart()
        {
            backSound();
            closeRestartFunction();
        }

        public void closeRestartFunction()
        {
            closePopupAnimation(restartPanelObj, restartBackgroundImage);
        }

        public void openLevels()
        {
            openPopupAnimation(levelsPanelObj, levelsBackgroundImage);
            StartCoroutine(waitForLevelToOpen());
        }

        IEnumerator waitForLevelToOpen()
        {
            yield return new WaitForSecondsRealtime(0.05f);
            checkLevelButtons();
        }

        public void closeLevels()
        {
            backSound();
            closeLevelsFunction();
        }

        public void closeLevelsFunction()
        {
            closePopupAnimation(levelsPanelObj, levelsBackgroundImage);
        }

        public void openMiniGames()
        {
            clickSound();
            openPopupAnimation(miniGamesPanelObj, miniGamesBackgroundImage);
        }

        public void closeMiniGames()
        {
            backSound();
            closeMiniGamesFunction();
        }

        public void closeMiniGamesFunction()
        {
            closePopupAnimation(miniGamesPanelObj, miniGamesBackgroundImage);
        }

        public void closePopupMessage()
        {
            backSound();
            closePopupAnimation(messagePopupObj, messagePopupBackgroundImage);
        }

        #endregion

        #region toggles
        public void toggleMusic()
        {
            if (PlayerPrefs.GetInt("music") == 0)
            {
                PlayerPrefs.SetInt("music", 1);
                FindObjectOfType<AudioManager>().Play("music");
            }
            else if (PlayerPrefs.GetInt("music") == 1)
            {
                PlayerPrefs.SetInt("music", 0);
                FindObjectOfType<AudioManager>().Stop("music");
            }
            clickSound();
            statusToggle();
        }

        public void clickSound()
        {
            FindObjectOfType<AudioManager>().Play("click");
        }

        public void backSound()
        {
            FindObjectOfType<AudioManager>().Play("back");
        }

        public void toggleSound()
        {
            if (PlayerPrefs.GetInt("sound") == 0)
            {
                PlayerPrefs.SetInt("sound", 1);
            }
            else if (PlayerPrefs.GetInt("sound") == 1)
            {
                PlayerPrefs.SetInt("sound", 0);
            }
            clickSound();
            statusToggle();
        }

        public void statusToggle()
        {
            if (PlayerPrefs.GetInt("sound") == 0)
            {
                soundImage.sprite = soundOffSprite;
            }
            else if (PlayerPrefs.GetInt("sound") == 1)
            {
                soundImage.sprite = soundOnSprite;
            }
            if (PlayerPrefs.GetInt("music") == 0)
            {
                musicImage.sprite = musicOffSprite;
            }
            else if (PlayerPrefs.GetInt("music") == 1)
            {
                musicImage.sprite = musicOnSprite;
            }
        }

        #endregion


        #region animation

        private IEnumerator ChangeImageColor(Image image, float duration, Color targetColor)
        {
            if (image != null)
            {
                float elapsedTime = 0f;
                Color startColor = image.color;

                while (elapsedTime < duration)
                {
                    Color lerpedColor = Color.Lerp(startColor, targetColor, elapsedTime / duration);
                    image.color = lerpedColor;
                    elapsedTime += Time.unscaledDeltaTime;
                    yield return null;
                }

                image.color = targetColor; // Ensure the target color is set precisely
            }
        }

        private IEnumerator ScaleTransform(Transform targetTransform, float duration, float targetScale)
        {
            if (targetTransform != null)
            {
                Vector3 initialScale = targetTransform.localScale;
                float startTime = Time.unscaledTime;
                float elapsedTime = 0f;

                while (elapsedTime < duration)
                {
                    float t = elapsedTime / duration;
                    targetTransform.localScale = Vector3.Lerp(initialScale, new Vector3(targetScale, targetScale, targetScale), t);
                    elapsedTime = Time.unscaledTime - startTime;
                    yield return null;
                }

                targetTransform.localScale = new Vector3(targetScale, targetScale, targetScale); // Ensure the target scale is set precisely
            }
        }

        public IEnumerator MoveToPosition(RectTransform rectTransform, Vector2 targetPosition, float duration)
        {

            Vector2 initialPosition = rectTransform.anchoredPosition;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float t = Mathf.SmoothStep(0, 1, elapsedTime / duration);
                rectTransform.anchoredPosition = Vector2.Lerp(initialPosition, targetPosition, t);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the final position is exact to avoid rounding errors
            rectTransform.anchoredPosition = targetPosition;
        }

        Coroutine levelIntroCoroutine1, levelIntroCoroutine2, levelIntroCoroutine3;

        public void levelIntro(string text)
        {
            levelIntroText.text = text;
            StartCoroutine(MoveToPosition(levelIntroImage.rectTransform, new Vector2(0, 65), 0));
            if (levelIntroCoroutine1 != null)
            {
                StopCoroutine(levelIntroCoroutine1);
            }
            if (levelIntroCoroutine2 != null)
            {
                StopCoroutine(levelIntroCoroutine2);
            }
            if (levelIntroCoroutine3 != null)
            {
                StopCoroutine(levelIntroCoroutine3);
            }
            levelIntroCoroutine1 = StartCoroutine(MoveToPosition(levelIntroImage.rectTransform, new Vector2(0, -65), popupAnimationSpeed));
            levelIntroCoroutine2 = StartCoroutine(removeLevelIntro());
        }


        IEnumerator removeLevelIntro()
        {
            yield return new WaitForSeconds(popupAnimationSpeed + 2);
            levelIntroCoroutine3 = StartCoroutine(MoveToPosition(levelIntroImage.rectTransform, new Vector2(0, 65), popupAnimationSpeed));
        }

        private void openPopupAnimation(GameObject windowPanel, Image BGImage)
        {
            StartCoroutine(ScaleTransform(windowPanel.transform, popupAnimationSpeed, 1));
            StartCoroutine(ChangeImageColor(BGImage, popupAnimationSpeed, popUpBackgroundColor));
            BGImage.raycastTarget = false;
            StartCoroutine(popupUpAnimationStopped(BGImage));
        }

        IEnumerator popupUpAnimationStopped(Image BGImage)
        {
            yield return new WaitForSecondsRealtime(popupAnimationSpeed + 0.05f);
            BGImage.raycastTarget = true;
        }

        private void closePopupAnimation(GameObject windowPanel, Image BGImage)
        {
            StartCoroutine(ScaleTransform(windowPanel.transform, popupAnimationSpeed, 0));
            StartCoroutine(ChangeImageColor(BGImage, popupAnimationSpeed, Color.clear));
            BGImage.raycastTarget = false;
        }

        public void messageAppear(string text)
        {
            messagePopupText.text = text;
            openPopupAnimation(messagePopupObj, messagePopupBackgroundImage);
        }

        void fadeStart()
        {
            blackPanel.raycastTarget = true;
            StartCoroutine(ChangeImageColor(loadingImage, fadeAnimationSpeed, accentColor));
            StartCoroutine(ChangeImageColor(blackPanel, fadeAnimationSpeed, fadeBackgroundColor));
        }

        void removeFade()
        {
            StartCoroutine(ChangeImageColor(loadingImage, fadeAnimationSpeed, Color.clear));
            StartCoroutine(ChangeImageColor(blackPanel, fadeAnimationSpeed, Color.clear));
            StartCoroutine(removeRaycast());
        }

        IEnumerator removeRaycast()
        {
            yield return new WaitForSecondsRealtime(fadeAnimationSpeed + 0.05f);
            blackPanel.raycastTarget = false;
        }

        #endregion

        [System.Serializable]
        public struct buttons
        {
            public string name;
            public string scene;
            public Color color;

            public buttons(string name, string scene, Color color)
            {
                this.name = name;
                this.scene = scene;
                this.color = color;
            }
        }



    }
}