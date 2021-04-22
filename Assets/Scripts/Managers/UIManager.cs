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
    private Text _ammoCountText;
    [SerializeField]
    private Image _livesDisplayImage;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartText;
    [SerializeField]
    private Text _waveText;
    [SerializeField]
    private Image _fuelBar;
    [SerializeField]
    private Sprite[] _livesSprite;

    private GameObject _camera;

    private GameManager _gameManager;
    private float fullFuel = 100f;
    private Vector3 _cameraOriginalPos;

    [Tooltip("Score: ")]
    private string _baseScoreText = "Score: ";

    void Start()
    {
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _livesDisplayImage.sprite = _livesSprite[3];
        _scoreText.text = "Score: " + 0;
        _ammoCountText.text = "Ammo: 15/15";

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("Game Manager is NULL");
        }
        _camera = GameObject.Find("Main Camera");
        if (_camera == null)
        {
            Debug.LogError("Main Camera is NULL");
        }
        else
        {
            _cameraOriginalPos = _camera.transform.position;
        }
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = _baseScoreText + score;
    }

    public void UpdateLives(int livesLeft)
    {
        _livesDisplayImage.sprite = _livesSprite[livesLeft];
        if (livesLeft == 0)
        {
            GameOverSequence();
        }
    }

    public void UpdateAmmo(int ammoRemaining)
    {
        _ammoCountText.text = "Ammo: " + ammoRemaining + "/15";
    }

    public void TriggerJamActive()
    {
        _ammoCountText.text = "Ammo: JAMMED";
    }

    public void UpdateFuel(float fuel)
    {
        _fuelBar.transform.localScale = new Vector3(fuel / fullFuel, 1f, 1f);
    }

    public void StartWaveTextRoutine(int wave)
    {
        StartCoroutine(DisplayWaveRoutine(wave));
    }

    IEnumerator DisplayWaveRoutine(int wave)
    {
        _waveText.gameObject.SetActive(true);
        _waveText.text = "Wave " + wave;
        yield return new WaitForSeconds(3f);
        _waveText.gameObject.SetActive(false);

    }

    void GameOverSequence()
    {
        _camera.transform.position = _cameraOriginalPos;
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

    //Phase 1:Framework -- Camera Shake
    public IEnumerator CameraShakeRoutine()
    {
        float duration = 0.5f;
        float elapsed = 0f;

        while(elapsed < duration)
        {
            float x = Random.Range(-0.5f, 0.5f);
            float y = Random.Range(0.5f, 1.5f);
            _camera.transform.position = new Vector3(x, y, -10f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _camera.transform.position = _cameraOriginalPos;
    }
}
