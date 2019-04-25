using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlotType
{
    Empty,
    Player1,
    Player2
} 

public class God : MonoBehaviour
{
    public GameObject[] checkerPrefabs;

    public static int numColumns = 7;
    public static int numRows = 6;
    public static float deltaX = 1.25f;
    public static float deltaY = 1.115f;
    
    private static Vector3 checkerStartPos = new Vector3(0.696f, 7.225f, 0);
    private static int NumCheckers { get; set; }
    private static SlotType[,] checkers = new SlotType[numRows, numColumns];
    private static God instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<God>();

        NumCheckers = numRows * numColumns;

        Instantiate(checkerPrefabs[0], checkerStartPos, Quaternion.Euler(90, 0, 0));
    }

    // Update is called once per frame
    void Awake()
    {
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                checkers[row, col] = SlotType.Empty;
            }
        }
    }

    private static bool IsGameOver()
    {
        SlotType player = FourInARow();
        if (player == SlotType.Player1)
        {
            Debug.Log("Player 1 won!");
            return true;
        }
        if (player == SlotType.Player2)
        {
            Debug.Log("Player 2 won!");
            return true;
        }
        if (NumCheckers == 0)
        {
            Debug.Log("Board has been filled!");
            return true;
        }
        return false;
    }

    private static SlotType FourInARow()
    {
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                SlotType playerNum = checkers[row, col];
                if (playerNum == SlotType.Empty) continue;

                if (col + 3 < numColumns &&
                    playerNum == checkers[row, col + 1] &&
                    playerNum == checkers[row, col + 2] &&
                    playerNum == checkers[row, col + 3])
                    return playerNum;
                if (row + 3 < numRows)
                {
                    if (playerNum == checkers[row + 1, col] &&
                        playerNum == checkers[row + 2, col] &&
                        playerNum == checkers[row + 3, col])
                        return playerNum;
                    if (col + 3 < numColumns &&
                        playerNum == checkers[row + 1, col + 1] &&
                        playerNum == checkers[row + 2, col + 2] &&
                        playerNum == checkers[row + 3, col + 3])
                        return playerNum;
                    if (col - 3 >= 0 &&
                        playerNum == checkers[row + 1, col - 1] &&
                        playerNum == checkers[row + 2, col - 2] &&
                        playerNum == checkers[row + 3, col - 3])
                        return playerNum;
                }
            }
        }
        return SlotType.Empty;
    }


    public static bool HandleCheckerPlace(Checker checker)
    {
        float currentXPosition = checker.instance.transform.position.x;
        int col = (int)((currentXPosition - checkerStartPos.x) / deltaX);

        Debug.Log("Player Number:");
        Debug.Log(checker.playerNumber);
        for (int row = 0; row < numRows; row++)
        {
            if (checkers[row, col] == SlotType.Empty)
            {
                instance.SetChecker(checker, row, col);
                return true;
            }
        }
        return false;
    }

    private void SetChecker(Checker checker, int row, int col)
    {
        checkers[row, col] = checker.playerNumber;
        NumCheckers--;
        float bottomRow = checkerStartPos.y - (deltaY * numRows);
        float leftmostCol = checkerStartPos.x;
        float r = bottomRow + (row * deltaY);
        float c = leftmostCol + (col * deltaX);
        checker.instance.transform.position = new Vector3(c, r, 0);

        if (!IsGameOver())
        {
            GameObject prefab;
            if (checker.playerNumber == SlotType.Player1)
            {
                prefab = this.checkerPrefabs[1];
            }
            else
            {
                prefab = this.checkerPrefabs[0];
            }
            Instantiate(prefab, checkerStartPos, Quaternion.Euler(90, 0, 0));
        }
    }
}