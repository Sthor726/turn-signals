using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    public Animator canvasAnimator;
    public Animator sceneTransitionAnimator;
    private GameManager gameManager;

    [Header("Movement")] public float forwardSpeed = 5f;
    public float maxSpeed = 25f;
    public float acceleration = 5f;

    [Header("Lane Settings")] public float laneWidth = 2f;

    [Header("Smoothing")] public float laneChangeSpeed = 10f;

    [Header("Effects")] public GameObject smokeParticleEffect;
    public ParticleSystem smokeParticleSystemRightWheel;
    public ParticleSystem smokeParticleSystemLeftWheel;

    [Header("Audio")] public AudioSource audioSource; // SFX (explosion etc)
    public AudioSource engineAudioSource; // LOOPING ENGINE SOUND
    public AudioClip explosionSoundEffect;
    public AudioClip coinSoundEffect;
    public AudioClip coinHighSoundEffect;

    [Header("Engine Settings")] public float minPitch = 0.8f;
    public float maxPitch = 1.8f;
    public float pitchSmoothing = 5f;

    private float currentPitch;

    public int maxCoinHighCounter = 12;
    public int coinHighCounter;
    private float coinHighTimer;
    public int minCoinHighCounter = 7;
    public float coinHighTimerDuration = 1f;
    private bool coinHighActive;
    public float coinHighSpeedBoost = 5f;
    public GameObject coinHighHighlight;
    
    
    private void Awake()
    {
        smokeParticleEffect.gameObject.SetActive(false);
        animator = GetComponent<Animator>();
        gameManager = FindFirstObjectByType<GameManager>();

        currentPitch = minPitch;
        
    }

    private void Start()
    {
        if (engineAudioSource != null)
        {
            engineAudioSource.loop = true;
            engineAudioSource.Play();
        }
    }

    void Update()
    {
        HandleInput();
        MoveForward();
        UpdateEngineSound();

        coinHighTimer += Time.deltaTime;

        while (coinHighTimer >= coinHighTimerDuration && coinHighCounter > 0)
        {
            if (coinHighCounter == minCoinHighCounter)
            {
                coinHighCounter = 0;
            }
            else
            {
                coinHighCounter -= 1;
            }
            coinHighTimer = 0;
        }

        if (coinHighCounter >= minCoinHighCounter && gameManager.gameState == GameManager.GameState.Active)
        {
            coinHighActive = true;
            coinHighHighlight.SetActive(true);
        }
        else
        {
            coinHighHighlight.SetActive(false);
            coinHighActive = false;
            canvasAnimator.SetBool("CoinHigh", false);
        }
        
        if (forwardSpeed == 0)
        {
            gameManager.uiAnimator.SetTrigger("Lose");
            if (Input.anyKeyDown)
            {
                sceneTransitionAnimator.SetTrigger("FadeOut");
            }
        }
    }

    void HandleInput()
    {
        if (gameManager.gameState == GameManager.GameState.Lost)
            return;

        if (forwardSpeed > 20f)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) && transform.position.x > -laneWidth)
            {
                smokeParticleSystemRightWheel.Play();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) && transform.position.x < laneWidth)
            {
                smokeParticleSystemLeftWheel.Play();
            }
        }

        if (Input.GetKey(KeyCode.LeftArrow) && transform.position.x > -laneWidth)
        {
            transform.position += Vector3.left * (laneChangeSpeed * Time.deltaTime);
            animator.SetBool("TurningLeft", true);
            animator.SetBool("TurningRight", false);
        }
        else if (Input.GetKey(KeyCode.RightArrow) && transform.position.x < laneWidth)
        {
            transform.position += Vector3.right * (laneChangeSpeed * Time.deltaTime);
            animator.SetBool("TurningRight", true);
            animator.SetBool("TurningLeft", false);
        }
        else
        {
            animator.SetBool("TurningLeft", false);
            animator.SetBool("TurningRight", false);
        }
    }

    void MoveForward()
    {
        float coinHighBonus = 0f;
        if (coinHighActive)
        {
            coinHighBonus = coinHighSpeedBoost;
        }
        forwardSpeed = Mathf.MoveTowards(forwardSpeed, maxSpeed + coinHighBonus, acceleration * Time.deltaTime);
        transform.position += Vector3.forward * forwardSpeed * Time.deltaTime;
    }

    void UpdateEngineSound()
    {
        switch (gameManager.gameState)
        {
            case GameManager.GameState.Before:
                break;
            case GameManager.GameState.Lost:
                engineAudioSource.enabled = false;
                break;
            default:
                engineAudioSource.enabled = true;
                break;
        }

        if (engineAudioSource == null) return;


        float speed01 = Mathf.InverseLerp(0f, 40f, forwardSpeed);

        float targetPitch =
            Mathf.Lerp(minPitch, maxPitch, Mathf.Pow(speed01, 1.3f));

        currentPitch = Mathf.Lerp(
            currentPitch,
            targetPitch,
            Time.deltaTime * pitchSmoothing
        );

        float wobble =
            (Mathf.PerlinNoise(Time.time * 2.5f, 0f) - 0.5f) * 0.05f;

        float loadBreathing =
            Mathf.Sin(Time.time * 3f) * 0.01f;

        engineAudioSource.pitch = currentPitch + wobble + loadBreathing;

        engineAudioSource.volume =
            Mathf.Lerp(0.9f, 1f, speed01)
            + (Mathf.Sin(Time.time * 2f) * 0.05f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstruction") &&
            gameManager.gameState == GameManager.GameState.Active)
        {
            loseLevel();
        }
        else if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            audioSource.PlayOneShot(coinSoundEffect);
            gameManager.score++;
            if (coinHighCounter < maxCoinHighCounter)
            {
                coinHighCounter++;
                if (coinHighCounter == minCoinHighCounter)
                {
                    // started a coin high
                    audioSource.PlayOneShot(coinHighSoundEffect);
                    coinHighCounter = maxCoinHighCounter;
                    canvasAnimator.SetBool("CoinHigh", true);
                }
            }
        }
    }

    void loseLevel()
    {
        audioSource.PlayOneShot(explosionSoundEffect);
        FindFirstObjectByType<MusicManager>().Stop();
        gameManager.gameState = GameManager.GameState.Lost;

        smokeParticleEffect.gameObject.SetActive(true);

        FindFirstObjectByType<CameraFollow>().Shake(0.2f, 0.3f);

        animator.SetTrigger("Lose");

        maxSpeed = 0f;
    }
}