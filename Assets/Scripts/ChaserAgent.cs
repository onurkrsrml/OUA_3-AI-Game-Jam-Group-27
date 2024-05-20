using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class ChaserAgent : Agent
{
    [SerializeField] private AgentType agentType;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private SpawnManager spawnManager;

    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private float heuristicMoveInput;
    private float heuristicJumpInput;

    private void Update()
    {
        heuristicMoveInput = Input.GetAxisRaw("Horizontal");
        heuristicJumpInput = Input.GetButton("Jump") ? 1 : 0;
    }

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public override void OnEpisodeBegin()
    {
        if (agentType == AgentType.Chaser)
        {
            spawnManager.SpawnAgents();
        }

        rb.velocity = Vector2.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector2)transform.localPosition);
        sensor.AddObservation((Vector2)targetTransform.localPosition);
        sensor.AddObservation(rb.velocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveInput = actions.DiscreteActions[0] - 1;
        float jumpInput = actions.DiscreteActions[1];
        playerMovement.SetInput(moveInput, jumpInput);

        if (rb.velocity.y > 0)
        {
            AddReward(.001f * jumpInput);
        }
        else
        {
            AddReward(-.02f * jumpInput);
        }

        if (Mathf.Abs(rb.velocity.x) < 1)
        {
            AddReward(-.005f);
        }

        if (StepCount == MaxStep)
        {
            AddReward(agentType == AgentType.Evader ? 1f : -1f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = (int)heuristicMoveInput + 1;
        discreteActions[1] = (int)heuristicJumpInput;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((targetLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            AddReward(agentType == AgentType.Chaser ? 1f : -1f);
            EndEpisode();
        }
    }
}

public enum AgentType
{
    Chaser,
    Evader
}