using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [Header("Game Elements")]                                         //reference to the different game elements
    [SerializeField] private GameObject player;                        
    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject deathZone;
    [SerializeField] private GameObject background;

    [Header("UI Components")] 
    [SerializeField] private UIManager uiManager;

    [Header("Game values")]
    [SerializeField] private float scrollSpeed = 0.2f;                  //how much does the camara move to follow the player
    
    [SerializeField] private GameObject platformPrefab;                 //reference to the platform object to create more
    private List<GameObject> platformList = new List<GameObject>();     //list to store created platforms
    private GameObject lastPlatformCreated;                             //reference to the last lastplatformCreated


    private float startingDistance;             //reference to where the player started
    private int maxDistance;                    //variable to store that max distance the player has reached in the current game

    private bool gameStarted;                   //bool that determines if the game has started
    private bool gameOver;                      //bool that turns true when the player dies
    
    // Start is called before the first frame update
    void Start()
    {
        startingDistance = player.transform.position.x;                     //saves the player starting distance

        GameObject startingPlatform = GameObject.FindWithTag("Platform");   //Finds the platform in the scene
        platformList.Add(startingPlatform);                                 //Adds it to the platform list
        lastPlatformCreated = startingPlatform;                             //and sets it as the last platform created

        CreatePlatform();                                                   //Then creates other 2 platforms
        CreatePlatform();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!gameStarted && Input.GetMouseButtonUp(0)) //If the game hasn't started and the players touches the screen
            StartGame();                                //starts game
        
        else if(!gameOver && player.Equals(null)) //If the player died, calls game over
            GameOver();
        
        else if (gameStarted && !gameOver)// else, the game continues as usual
        {
            //changes current score if the player has gone forward
            int playerProgress = (int)(player.transform.position.x - startingDistance);
            if (playerProgress > maxDistance)
            {
                maxDistance = playerProgress;
                uiManager.ChangeScoreText(playerProgress); 
            }
            
            //Move other elements of the scene so they follow the player
            MoveElements();
            
            //Erases platforms that are too far back
            EraseOldPlatforms();
        
            //And creates new ones if more platforms are needed
            if (platformList.Count < 5)
                CreatePlatform();
            
            
        }
    }

    private void StartGame()
    {
        uiManager.HideInstructions();                               //hides the instructions UI
        player.GetComponent<PlayerController>().StartPlayer();      //Calls method to start player physics

        gameStarted = true;                                         //sets game started as true
    }

    private void MoveElements()
    {
        float playerXPos = player.transform.position.x;

        //Makes the camera follow the player depending on the distance between them
        float cameraDistanceFromPlayer = camera.transform.position.x - playerXPos;
        
        if (cameraDistanceFromPlayer < 1.44f)
        {
            camera.transform.position = new Vector3(camera.transform.position.x + scrollSpeed, 2.22f, -10);
        }
        else if(cameraDistanceFromPlayer > 1.58f)
        {
            camera.transform.position = new Vector3(camera.transform.position.x - scrollSpeed, 2.22f, -10);
        }
        
        //Makes death zone and background follow player
        deathZone.transform.position = new Vector3(playerXPos, -10.5f, 0);
        background.transform.position = new Vector3(camera.transform.position.x, -0.37f, 0.6f);
        
    }
    
    private void CreatePlatform()
    {
        Vector3 platformPosition =  new Vector3();                                  //creates a vector for the position of the new platform
        platformPosition.x = lastPlatformCreated.transform.position.x + 4.05f;      //adds 4.05 units to the x position of the last platform created
        platformPosition.y = Random.Range(-4.3f, -2.25f);                           //gives it a random y value
        platformPosition.z = 0;
        
        GameObject newPlatform = Instantiate(platformPrefab, platformPosition, Quaternion.identity); //creates a new platform with the given position
        lastPlatformCreated = newPlatform;                                                      //saves a reference to the platform
        platformList.Add(newPlatform);                                                          //and adds it to the list
    }

    private void EraseOldPlatforms()  //removes and destroys the platforms that are already too far behind
    {
        foreach (var platform in platformList) 
        {
            if (platform.transform.position.x -camera.transform.position.x <= -7.85f)
            {
                platformList.Remove(platform);
                Destroy(platform);
            }
        }
    }

    private void GameOver()
    {
        //show game over screen
        uiManager.ShowGameOverScreen();
        gameOver = true;
    }
}
