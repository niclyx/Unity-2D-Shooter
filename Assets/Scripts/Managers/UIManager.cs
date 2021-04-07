using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Image _livesDisplayImage;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartText;
    [SerializeField]
    private Sprite[] _livesSprite;

    private GameManager _gameManager;
    [Tooltip("Score: ")]
    private string _baseScoreText = "Score: ";

    void Start()
    {
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _livesDisplayImage.sprite = _livesSprite[3];
        _scoreText.text = "Score: " + 0;

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("Game Manager is NULL");
        }
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = _baseScoreText + score;
    }

    public void UpdateLives(int livesLeft)
    {
        _livesDisplayImage.sprite = _livesSprite[livesLeft];
        if(livesLeft==0)
        {
            GameOverSequence();
        }
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while(true)
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.75f);
            _gameOverText.enabled = false;
            yield return new WaitForSeconds(0.75f);
            _gameOverText.enabled = true;
        }
    }
}
