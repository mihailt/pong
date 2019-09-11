using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public float moveSpeed = 7.0f;
    public float topBounds = 8.3f;
    public float bottomBounds = -8.3f;
    private GameObject ball;
    private Ball ballComponent;
    private Vector2 ballPos;

    public Vector2 startPosition = new Vector2(13.0f, 0.0f);
    private Game game;

    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = (Vector3) startPosition;
        game = GameObject.Find("Game").GetComponent<Game>();

    }

    // Update is called once per frame
    void Update()
    {
        if (game.gameState != Game.GameState.Playing) { return; }
        Move();
    }

    void Move() {
        if(!ball) {
            ball = GameObject.FindGameObjectWithTag("Ball");
            ballComponent = ball.GetComponent<Ball>();
        }
        if (ballComponent.ballDirection == Vector2.right) {
            ballPos = ball.transform.localPosition;

            if (transform.localPosition.y > bottomBounds && ballPos.y < transform.localPosition.y) {
                transform.localPosition += new Vector3(0, -moveSpeed * Time.deltaTime, transform.localPosition.z); 
            } else if (transform.localPosition.y < topBounds && ballPos.y > transform.localPosition.y) {
                transform.localPosition += new Vector3(0, moveSpeed * Time.deltaTime, transform.localPosition.z); 
            }
        }
    }
}
