using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDHandler : MonoBehaviour
{
    private const int LevelUpCost = 10;
    private const int HeroesCount = 3;
    private const int MoneyForCompletion = 20;

    [SerializeField]
    private PlayerState _playerState = null;

    [SerializeField]
    private MainBridge _bridge = null;

    [SerializeField]
    private Button _startButton = null;
    [SerializeField]
    private Button _nextButton = null;
    [SerializeField]
    private Button _retryButton = null;

    [SerializeField]
    private TMP_Text _moneyText = null;
    [SerializeField]
    private TMP_Text _greenHeroLevelText = null;
    [SerializeField]
    private TMP_Text _blueHeroLevelText = null;
    [SerializeField]
    private TMP_Text _yellowHeroLevelText = null;

    private int _currentLevel;
    private int _readyHeroes;
    private int _finishedHeroes;
    private int _reachedGoalHeroes;

    protected void Awake()
    {
        _bridge.OnHeroReady += HeroReady;
        _bridge.OnHeroReachGoal += HeroReachGoal;

        _currentLevel = 0;
        LoadNextLevel();
    }

    protected void Start()
    {
        UpdateMoneyText();

        _greenHeroLevelText.text = _playerState.GreenHero.Level.ToString();
        _blueHeroLevelText.text = _playerState.BlueHero.Level.ToString();
        _yellowHeroLevelText.text = _playerState.YellowHero.Level.ToString();
    }

    protected void OnDestroy()
    {
        _bridge.OnHeroReady -= HeroReady;
        _bridge.OnHeroReachGoal -= HeroReachGoal;
    }

    public void GreenLevelUp()
    {
        if (MakeTransaction())
        {
            _playerState.GreenHero.Level++;

            UpdateMoneyText();
            _greenHeroLevelText.text = _playerState.GreenHero.Level.ToString();
        }
    }

    public void BlueLevelUp()
    {
        if (MakeTransaction())
        {
            _playerState.BlueHero.Level++;

            UpdateMoneyText();
            _blueHeroLevelText.text = _playerState.BlueHero.Level.ToString();
        }
    }

    public void YellowLevelUp()
    {
        if (MakeTransaction())
        {
            _playerState.GreenHero.Level++;

            UpdateMoneyText();
            _yellowHeroLevelText.text = _playerState.YellowHero.Level.ToString();
        }
    }

    public void StartMovement()
    {
        _bridge.OnMovementStart.Invoke();
        _finishedHeroes = 0;
        _reachedGoalHeroes = 0;

        _startButton.gameObject.SetActive(false);
    }

    public void LoadNextLevel()
    {
        if (_currentLevel > 0)
            SceneManager.UnloadSceneAsync(_currentLevel);

        _currentLevel++;
        if (_currentLevel >= SceneManager.sceneCountInBuildSettings)
            _currentLevel = 1;

        SceneManager.LoadScene(_currentLevel, LoadSceneMode.Additive);
        _readyHeroes = 0;

        _nextButton.gameObject.SetActive(false);
        _startButton.gameObject.SetActive(true);
        _startButton.interactable = false;
    }

    public void ReloadCurrentLevel()
    {
        SceneManager.UnloadSceneAsync(_currentLevel);
        SceneManager.LoadScene(_currentLevel, LoadSceneMode.Additive);
        _readyHeroes = 0;

        _retryButton.gameObject.SetActive(false);
        _startButton.gameObject.SetActive(true);
        _startButton.interactable = false;
    }

    private void UpdateMoneyText()
    {
        _moneyText.text = _playerState.Money.ToString();
    }

    private void HeroReady()
    {
        _readyHeroes++;

        if (_readyHeroes == HeroesCount)
        {
            _startButton.interactable = true;
        }
    }

    private void HeroReachGoal(bool isReached)
    {
        _finishedHeroes++;
        if (isReached)
            _reachedGoalHeroes++;

        if (_finishedHeroes == HeroesCount)
        {
            if (_reachedGoalHeroes == HeroesCount)
            {
                _playerState.Money += MoneyForCompletion;
                UpdateMoneyText();

                _nextButton.gameObject.SetActive(true);
            }
            else
            {
                _retryButton.gameObject.SetActive(true);
            }
        }
    }

    private bool MakeTransaction()
    {
        if (_playerState.Money >= LevelUpCost)
        {
            _playerState.Money -= LevelUpCost;
            return true;
        }
        else
        {
            return false;
        }
    }
}
