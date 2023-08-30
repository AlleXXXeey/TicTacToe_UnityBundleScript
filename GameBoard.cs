using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Studio29.TicTacToe
{
    public class GameBoard : MonoBehaviour
    {
        #region Variable

        [SerializeField] private Button[] cells;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button restartButton;

        private char currentPlayer = 'X';
        private bool isGameOver = false;
        private char[] boardState = new char[9];

        private bool isPlayerTurn = true; // ��������� ������� ������ ���

        #endregion

        #region Unity Callback

        private void Start()
        {
            InitializeGame();
            isPlayerTurn = true; // ϳ��� ����������� ������� ������
        }

        #endregion

        #region Initialize

        private void InitializeGame()
        {
            messageText.text = "������ ������� X";

            restartButton.onClick.AddListener(RestartGame);
            restartButton.gameObject.SetActive(false);

            for (int i = 0; i < cells.Length; i++)
            {
                int index = i;
                cells[i].onClick.AddListener(() => CellClick(index));
                TextMeshProUGUI cellText = cells[i].GetComponentInChildren<TextMeshProUGUI>();
                cellText.text = "";
            }

            ResetBoardState();
            isGameOver = false;
        }

        #endregion

        #region Game Logic

        private void CellClick(int cellIndex)
        {
            if (!isGameOver && cells[cellIndex].interactable)
            {
                if (currentPlayer == 'X' && isPlayerTurn) // ������� X ������ ���� �� ��� �������� �����
                {
                    PlayerMove(cellIndex);
                }
            }
        }


        private void PlayerMove(int cellIndex)
        {
            cells[cellIndex].interactable = false;
            TextMeshProUGUI cellText = cells[cellIndex].GetComponentInChildren<TextMeshProUGUI>();
            cellText.text = currentPlayer.ToString();

            boardState[cellIndex] = currentPlayer;

            if (CheckWinningMove(currentPlayer, boardState))
            {
                messageText.text = "������� " + currentPlayer + " ������!";
                restartButton.gameObject.SetActive(true);
                isGameOver = true;
            }
            else if (CheckDraw(boardState))
            {
                messageText.text = "ͳ���!";
                restartButton.gameObject.SetActive(true);
                isGameOver = true;
            }
            else
            {
                if (isPlayerTurn)
                {
                    isPlayerTurn = false; // ϳ��� ���� ������, ��������� ��� ���� � ����
                    currentPlayer = 'O';
                    messageText.text = "ճ� ������ " + currentPlayer;

                    // ������ ������� ��� ���� ����
                    BotMove();
                }
            }
        }

        private void BotMove()
        {
            if (!isPlayerTurn) // ��� (������� O) ������, ���� �� �������� �����
            {
                PlayerMove(FindBestMove());
                isPlayerTurn = true;
                currentPlayer = 'X';
                messageText.text = "ճ� ������ " + currentPlayer;                
            }
        }


        private bool CheckWinningMove(char player, char[] board)
        {
            // �������� �� �������� ������

            // �������� �� ����������
            for (int row = 0; row < 3; row++)
            {
                if (board[row * 3] == player && board[row * 3 + 1] == player && board[row * 3 + 2] == player)
                    return true;
            }

            // �������� �� ��������
            for (int col = 0; col < 3; col++)
            {
                if (board[col] == player && board[col + 3] == player && board[col + 6] == player)
                    return true;
            }

            // �������� �� ������� ���� �������
            if (board[0] == player && board[4] == player && board[8] == player)
                return true;

            // �������� �� ������� ������ �����
            if (board[2] == player && board[4] == player && board[6] == player)
                return true;

            return false;
        }

        private bool CheckDraw(char[] board)
        {
            // �������� �� ����

            foreach (char cell in board)
            {
                if (cell == '\0')
                    return false; // ���� � ������ ������, ��� �� ����
            }

            return true; // ���� �� ������ ��������, �� ����
        }


        private void RestartGame()
        {
            currentPlayer = 'X';
            isGameOver = false;
            messageText.text = "������ ������� X";
            restartButton.gameObject.SetActive(false);

            ResetBoardState();
        }

        private void ResetBoardState()
        {
            for (int i = 0; i < boardState.Length; i++)
            {
                boardState[i] = '\0';
                cells[i].interactable = true;
                TextMeshProUGUI cellText = cells[i].GetComponentInChildren<TextMeshProUGUI>();
                cellText.text = "";
            }
        }

        #endregion

        #region MiniMax

        private int FindBestMove()
        {
            int bestScore = int.MinValue;
            int bestMove = -1;
            int alpha = int.MinValue;
            int beta = int.MaxValue;

            for (int i = 0; i < boardState.Length; i++)
            {
                if (boardState[i] == '\0')
                {
                    boardState[i] = 'O';
                    int moveScore = Minimax(boardState, 0, false, alpha, beta);
                    boardState[i] = '\0';

                    if (moveScore > bestScore)
                    {
                        bestScore = moveScore;
                        bestMove = i;
                    }

                    alpha = Mathf.Max(alpha, bestScore);
                    if (beta <= alpha)
                        break;
                }
            }

            return bestMove;
        }

        private int Minimax(char[] board, int depth, bool isMaximizing, int alpha, int beta)
        {
            if (CheckWinningMove('O', board))
                return 1; 
            if (CheckWinningMove('X', board))
                return -1;
            if (CheckDraw(board) || depth >= 5)
                return 0;

            if (isMaximizing)
            {
                int bestScore = int.MinValue;
                for (int i = 0; i < board.Length; i++)
                {
                    if (board[i] == '\0')
                    {
                        board[i] = 'O'; // ������ 'X' �� 'O'
                        int score = Minimax(board, depth + 1, false, alpha, beta);
                        board[i] = '\0';
                        bestScore = Mathf.Max(bestScore, score);
                        alpha = Mathf.Max(alpha, bestScore);
                        if (beta <= alpha)
                            break;
                    }
                }
                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;
                for (int i = 0; i < board.Length; i++)
                {
                    if (board[i] == '\0')
                    {
                        board[i] = 'X'; // ������ 'O' �� 'X'
                        int score = Minimax(board, depth + 1, true, alpha, beta);
                        board[i] = '\0';
                        bestScore = Mathf.Min(bestScore, score);
                        beta = Mathf.Min(beta, bestScore);
                        if (beta <= alpha)
                            break;
                    }
                }
                return bestScore;
            }
        }

        #endregion
    }
} 