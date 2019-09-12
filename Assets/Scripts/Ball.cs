using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float moveSpeed = 14.0f;
    public float topBounds = 9.4f;
    public float bottomBounds = -9.4f;
    public Vector2 ballDirection = Vector2.right;
    private float playerPaddleWidth, playerPaddleHeight, aiPaddleWidth, aiPaddleHeight;
    private float playerPaddleMinX, playerPaddleMinY, playerPaddleMaxX, playerPaddleMaxY;
    private float aiPaddleMinX, aiPaddleMinY, aiPaddleMaxX, aiPaddleMaxY;
    private float ballWidth, ballHeight;
    private float bounceAngle;
    private float vx, vy;
    private float maxAngle = 45.0f;

    private bool collidedWithPlayer, collidedWithAi, collidedWithWall;

    private GameObject playerPaddle, aiPaddle;
    private Game game;

    private bool assignedPoint;

    public int speedIncreaseInterval = 10;
    public float speedIncreaseBy = 1.0f;
    private float speedIncreaseTimer;

    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.Find("Game").GetComponent<Game>();

        if (moveSpeed < 0) {
            moveSpeed = -1 * moveSpeed;
        }

        playerPaddle = GameObject.Find("PlayerPaddle");
        aiPaddle = GameObject.Find("AIPaddle");

        playerPaddleHeight = playerPaddle.transform.GetComponent<SpriteRenderer>().bounds.size.y;
        playerPaddleWidth = playerPaddle.transform.GetComponent<SpriteRenderer>().bounds.size.x;

        aiPaddleHeight = aiPaddle.transform.GetComponent<SpriteRenderer>().bounds.size.y;
        aiPaddleWidth = aiPaddle.transform.GetComponent<SpriteRenderer>().bounds.size.x;

        ballHeight = transform.GetComponent<SpriteRenderer>().bounds.size.y;
        ballWidth = transform.GetComponent<SpriteRenderer>().bounds.size.x;

        playerPaddleMaxX = playerPaddle.transform.localPosition.x + playerPaddleWidth / 2;
        playerPaddleMinX = playerPaddle.transform.localPosition.x - playerPaddleWidth / 2;

        aiPaddleMaxX = aiPaddle.transform.localPosition.x - aiPaddleWidth / 2;
        aiPaddleMinX = aiPaddle.transform.localPosition.x + aiPaddleWidth / 2;

        bounceAngle = GetRandomBounceAngle(); // 180.0f * Mathf.Deg2Rad;

        vx = maxAngle * Mathf.Cos(bounceAngle);
        vy = maxAngle * -Mathf.Sin(bounceAngle);
    }

    // Update is called once per frame
    void Update()
    {
        if (game.gameState != Game.GameState.Playing) { return; }
        Move();
        UpdateSpeedIncrease();
    }

    bool CheckCollision () {
    
        // The top & bottom edges of the paddles in motion:
        playerPaddleMaxY   = playerPaddle.transform.localPosition.y + playerPaddleHeight / 2;
        playerPaddleMinY   = playerPaddle.transform.localPosition.y - playerPaddleHeight / 2;

        aiPaddleMaxY = aiPaddle.transform.localPosition.y + aiPaddleHeight / 2;
        aiPaddleMinY = aiPaddle.transform.localPosition.y - aiPaddleHeight / 2;

        // check for x collision
        if ( transform.localPosition.x - ballWidth / 2 < playerPaddleMaxX && transform.localPosition.x + ballWidth / 2 > playerPaddleMinX ) {        
            // then check for y collision
            if ( transform.localPosition.y - ballHeight / 2 < playerPaddleMaxY && transform.localPosition.y + ballHeight / 2 > playerPaddleMinY ) {
                collidedWithPlayer = true;
                ballDirection.x = ballDirection.x * -1;
                transform.localPosition = new Vector3(playerPaddleMaxX + ballWidth, transform.localPosition.y, transform.localPosition.z);
                return true;
            } else {
                if(!assignedPoint)  {
                    assignedPoint = true;
                    game.AiPoint();
                }
            }
        }

        if (transform.localPosition.x + ballWidth / 2 > aiPaddleMaxX && transform.localPosition.x - ballWidth / 2 < aiPaddleMinX ) {
            if ( transform.localPosition.y - ballHeight / 2 < aiPaddleMaxY && transform.localPosition.y + ballHeight / 2 > aiPaddleMinY ) {
                collidedWithAi = true;
                ballDirection.x = ballDirection.x * -1;
                transform.localPosition = new Vector3(aiPaddleMaxX - ballWidth, transform.localPosition.y, transform.localPosition.z);

                return true;
            } else {
                if(!assignedPoint)  {
                    assignedPoint = true;
                    game.PlayerPoint();
                }
            }
        }

        if (transform.localPosition.y > topBounds) {
            collidedWithWall = true;
            transform.localPosition = new Vector3(transform.localPosition.x, topBounds, transform.localPosition.z);
            return true;
        }

        if (transform.localPosition.y < bottomBounds) {
            collidedWithWall = true;
            transform.localPosition = new Vector3(transform.localPosition.x, bottomBounds, transform.localPosition.z);
            return true;
        }

        return false;
    }

    void Move() {
        if ( !CheckCollision() ) {
            vx = moveSpeed * Mathf.Cos(bounceAngle);
            if (moveSpeed > 0) {
                vy = moveSpeed * -Mathf.Sin(bounceAngle);
            }  else {
                vy = moveSpeed * Mathf.Sin(bounceAngle);
            }
            transform.localPosition += new Vector3(ballDirection.x * vx * Time.deltaTime, vy * Time.deltaTime, transform.localPosition.z);//(Vector3) ballDirection * moveSpeed * Time.deltaTime;
        } else {
            if (moveSpeed < 0) {
                moveSpeed = -1 * moveSpeed;
            }

            if (collidedWithPlayer) {
                collidedWithPlayer = false;
                float relY = playerPaddle.transform.localPosition.y - transform.position.y;
                float norm = (relY / (playerPaddleHeight / 2));
                bounceAngle = norm * (maxAngle * Mathf.Deg2Rad);
            } else if (collidedWithAi) {
                collidedWithAi = false;
                float relY = aiPaddle.transform.localPosition.y - transform.position.y;
                float norm = (relY / (aiPaddleHeight / 2));
                bounceAngle = norm * (maxAngle * Mathf.Deg2Rad);
            } else if (collidedWithWall) {
                collidedWithWall = false;
                bounceAngle = -bounceAngle;
            }
        }
    }

    float GetRandomBounceAngle(float min = 160.0f, float max = 260.0f) {
        return Random.Range(min * Mathf.Deg2Rad, max * Mathf.Deg2Rad);
    }

    void UpdateSpeedIncrease() {
        if(speedIncreaseTimer > speedIncreaseInterval) {
            speedIncreaseTimer = 0;
            if(moveSpeed > 0) {
                moveSpeed += speedIncreaseBy;
            } else {
                moveSpeed -= speedIncreaseBy;
            }
        } else {
            speedIncreaseTimer += Time.deltaTime;
        }
    }
}
