using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private GameObject ball;
    private GameObject aiPaddle;
    private int playerScore;
    private int aiScore;
    private Hud hud;

    public enum GameState {
        Playing,
        GameOver,
        Paused,
        Launched
    }

    public GameState gameState = GameState.Launched;

    public int winningScore = 10;
    // Start is called before the first frame update
    void Start()
    {
        hud = GameObject.Find("Canvas").GetComponent<Hud>();
        aiPaddle = GameObject.Find("AIPaddle");

        hud.aiWin.enabled = false;
        hud.playerWin.enabled = false;

        hud.info.text = "Press spacebar to play";

    }

    // Update is called once per frame
    void Update()
    {
        CheckScore();
        CheckInput();
    }

    void CheckInput() {
        if(gameState == GameState.Launched || gameState == GameState.GameOver) {
            if (Input.GetKey(KeyCode.Space)) {
                StartGame();
            }
        } else if(gameState == GameState.Paused || gameState == GameState.Playing) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                PauseResumeGame();
            }
        }
    }

    void CheckScore() {
        if (playerScore >= winningScore || aiScore >= winningScore) {
            if (playerScore >= winningScore && aiScore < playerScore - 1) {
                PlayerWins();
            } else if (aiScore >= winningScore && playerScore < aiScore - 1) {
                AiWins();
            }
        }
    }

    void SpawnBall() {
        ball = GameObject.Instantiate((GameObject) Resources.Load("Prefabs/Ball", typeof(GameObject)));
        ball.transform.localPosition = new Vector3(12f, 0, 0);
    }

    public void AiPoint() {
        aiScore++;
        hud.aiScore.text = $"{aiScore}";
        NextRound();
    }

    private void PlayerWins() {
        hud.playerWin.enabled = true;
        GameOver();
    }
    private void AiWins() {
        hud.aiWin.enabled = true;
        GameOver();
    }

    public void PlayerPoint() {
        playerScore++;
        hud.playerScore.text = $"{playerScore}";
        NextRound();
    }

    public void NextRound() {
        if (gameState != GameState.Playing) { return; }
        aiPaddle.transform.position = new Vector3(aiPaddle.transform.localPosition.x, 0, aiPaddle.transform.localPosition.z);
        Destroy(ball);
        SpawnBall();
    }

    private void GameOver() {
        gameState = GameState.GameOver;
        Destroy(ball);
        hud.info.text = "Press spacebar to play again";
        hud.info.enabled = true;
    }

    private void StartGame() {
        playerScore = 0;
        aiScore = 0;
        hud.playerScore.text = "0";
        hud.aiScore.text = "0";
        hud.aiWin.enabled = false;
        hud.playerWin.enabled = false;
        hud.info.enabled = false;

        aiPaddle.transform.position = new Vector3(aiPaddle.transform.localPosition.x, 0, aiPaddle.transform.localPosition.z);

        gameState = GameState.Playing;

        SpawnBall();
    }

    private void PauseResumeGame() {
        if (gameState == GameState.Paused) {
            gameState = GameState.Playing;
            hud.info.enabled = false;
        } else {
            gameState = GameState.Paused;
            hud.info.text = "Press spacebar to continue playing";
            hud.info.enabled = true;
        }
    }
}
