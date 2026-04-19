using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class NpcDriver : MonoBehaviour
{
    public PlayerMovement player;

    private float laneWidth => player.laneWidth;
    public float laneChangeSpeed = 5f;

    public Material[] materials;
    public MeshRenderer bodyMeshRenderer;

    private float speed;

    private int currentLane;
    private int targetLane;

    private float changeTimer;
    private float warningTimer;
    private bool isChanging;

    public GameObject leftTurnSignal;
    public GameObject rightTurnSignal;


    void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>();

        speed = 25 * Random.Range(0.5f, 0.75f);

        ScheduleNextLaneChange();
        AssignMaterial();
    }

    void AssignMaterial()
    {
        if (materials.Length == 0) return;
        var mats = bodyMeshRenderer.sharedMaterials;
        mats[0] = materials[Random.Range(0, materials.Length)];
        bodyMeshRenderer.sharedMaterials = mats;
    }

    void Update()
    {
        MoveForward();
        HandleLaneMovement();
        HandleLaneChangeLogic();
    }

    void MoveForward()
    {
        transform.position += Vector3.forward * speed * Time.deltaTime;
    }

    void HandleLaneMovement()
    {
        float targetX = currentLane * laneWidth;

        Vector3 targetPos = new Vector3(
            targetX,
            transform.position.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            laneChangeSpeed * Time.deltaTime
        );
    }

    void HandleLaneChangeLogic()
    {
        if (isChanging)
        {
            warningTimer -= Time.deltaTime;

            if (targetLane > currentLane)
            {
                rightTurnSignal.GetComponent<Animator>().SetBool("Blinking", true);
            }
            else if (targetLane < currentLane)
            {
                leftTurnSignal.GetComponent<Animator>().SetBool("Blinking", true);
            }

            if (warningTimer <= 0f)
            {
                currentLane = targetLane;
                isChanging = false;

                leftTurnSignal.GetComponent<Animator>().SetBool("Blinking", false);
                rightTurnSignal.GetComponent<Animator>().SetBool("Blinking", false);


                ScheduleNextLaneChange();
            }

            return;
        }

        changeTimer -= Time.deltaTime;

        if (changeTimer <= 0f)
            StartLaneChange();
    }

    void StartLaneChange()
    {
        int newLane = currentLane;

        while (newLane == currentLane || Math.Abs(newLane - currentLane) != 1)
            newLane = Random.Range(-1, 2);

        targetLane = newLane;

        isChanging = true;
        warningTimer = Random.Range(1f, 2f);
    }

    void ScheduleNextLaneChange()
    {
        changeTimer = Random.Range(2f, 5f);
    }
}