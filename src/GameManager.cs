using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Before,
        Active,
        Lost
    }

    public Volume volume;
    private ChromaticAberration chromaticAberration;
    public float lerpSpeed = 1f;
    public float maxPlayerSpeed = 45f;
    public float minSpawnInterval = 1.5f;
    public PlayerMovement playerMovement;
    public NpcSpawner npcSpawner;
    public float score;
    public float gameTimer;
    public float difficultyIncreaseInterval = 20f;
    public GameState gameState;
    public Animator uiAnimator;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreTextBG;
    public TextMeshProUGUI scoreTextGameOver;
    
    private void Start()
    {
        npcSpawner.enabled = false;

        volume.profile = Instantiate(volume.profile);
        if (!volume.profile.TryGet(out chromaticAberration))
        {
            Debug.LogError("No Chromatic Aberration found in Volume!");
        }
        else
        {
            chromaticAberration.intensity.value = 0f;
            chromaticAberration.active = true;
        }
    }

    void Update()
    {
        gameTimer += Time.deltaTime;
        scoreText.text = score.ToString("0");
        scoreTextBG.text = score.ToString("0");
        scoreTextGameOver.text = "score: " + score.ToString("0");

        if (gameState == GameState.Before)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                HandleInput();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                HandleInput();
            }
        }

        if (chromaticAberration == null) return;

        float targetValue = 0f;
        switch (gameState)
        {
            case GameState.Before:
                targetValue = 0f;
                break;
            case GameState.Active:
                targetValue = 0.5f;
                break;
            case GameState.Lost:
                targetValue = 0f;
                break;
        }

        chromaticAberration.intensity.value = Mathf.Lerp(
            chromaticAberration.intensity.value,
            targetValue,
            Time.deltaTime * lerpSpeed
        );

        if (gameTimer >= difficultyIncreaseInterval)
        {
            gameTimer = 0f;
            if (npcSpawner.spawnInterval > minSpawnInterval)
            {
                npcSpawner.spawnInterval -= 0.3f;
            }
            // if uncomment, make sure not to do this when gameStarted = false
            // if (playerMovement.maxSpeed < maxPlayerSpeed)
            // {
            //     playerMovement.maxSpeed += 5f;
            // }
        }
    }

    void HandleInput()
    {
        gameState = GameState.Active;
        FindFirstObjectByType<MusicManager>().FadeIn();
        uiAnimator.SetTrigger("StartGame");
        playerMovement.maxSpeed = 25f;
        if (npcSpawner != null)
        {
            npcSpawner.enabled = true;
        }
    }
}