using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class ScoreManager : MonoBehaviour
{
    private float score = 0;
    private float timeLeftSaved = 90;
    private float timeLeft;
    private float timeOffset = 0;
    private float objective = 5;
    private List<Vector3> movableTransformInitialPos = new List<Vector3>();
    
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI timeText;
    public GameObject boxSpawnLocations;
    public GameObject boxPrefab;
    public GameObject playButton;
    public Transform[] movableObject;
    
    private void Start()
    {
        timeLeft = timeLeftSaved;
        SpawnNewBoxes();
        
        foreach(Transform obj in movableObject)
        {
            movableTransformInitialPos.Add(obj.position);
        }
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        timeText.text = Mathf.RoundToInt(timeLeft) + " Seconds left";
        
        if (timeLeft <= 0)
        {
            ResetGame();
        }
    }

    private void ResetGame()
    {
        timeLeft = timeLeftSaved;
        score = 0;
        objective = 5;
        scoreText.text = "Score : " + score;
        objectiveText.text = "next objective : " + objective;

        for (int i = 0; i < movableObject.Length; i++)
        {
            movableObject[i].transform.position = movableTransformInitialPos[i];
        }

        movableObject[0].GetComponent<CharacterController>().Move(Vector3.zero);
        
        foreach (GameObject box in GameObject.FindGameObjectsWithTag("box"))
        {
            Destroy(box);
        }
        
        SpawnNewBoxes();

        playButton.SetActive(true);
        Time.timeScale = 0;
        
        Debug.Log("Game Ended !");
    }

    public void BoxPlaced(float yPos)
    {
        if (yPos > score)
        {
            score = Mathf.Round(yPos);
            scoreText.text = "Score : " + score;
            RefreshObjective(score);
        }
    }

    private void RefreshObjective(float score)
    {
        if (score >= objective)
        {
            objective += 5;
            objectiveText.text = "next objective : " + objective;
            SpawnNewBoxes();
        }
    }
    
    private void SpawnNewBoxes()
    {

        foreach (Transform spawn in boxSpawnLocations.transform)
        {
            Instantiate(boxPrefab, spawn.position, Quaternion.identity).transform.localScale = new Vector3(genRandScale(), genRandScale(), genRandScale());
        }
    }

    private float genRandScale()
    {
        float randScale = Random.Range(1f, 2f);
        return (float) Math.Round(randScale, 1);
    }
        
}
