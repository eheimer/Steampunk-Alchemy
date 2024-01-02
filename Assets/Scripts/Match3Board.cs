using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3Board : MonoBehaviour
{
    public int width = 6;
    public int height = 8;
    public GameObject[] itemPrefabs;
    public Spinach.Grid<Match3Item> gameBoard;
    public GameObject gameBoardGO;
    public List<GameObject> itemsToDestroy = new();
    public GameObject itemParent;

    public GameScene gameScene;

    [SerializeField]
    private Match3Item selectedItem;
    [SerializeField]
    private bool isProcessingMove;
    [SerializeField]
    List<Match3Item> itemsToRemove = new();

    public ParticleSystem matchParticlePrefab;
    public AudioClip matchClip;

    public ArrayLayout arrayLayout; // this is the layout of the board, true means unusable
    public static Match3Board Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        arrayLayout = new ArrayLayout(height, width);
        InitializeBoard();
    }

    private Vector2 touchStartPos, touchEndPos = default;

    void Update()
    {
        if (!isProcessingMove)
        {
            if (Input.touchCount > 0 || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
            {
                if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                    if (hit.collider != null && hit.collider.GetComponent<Match3Item>() != null)
                    {
                        Match3Item item = hit.collider.gameObject.GetComponent<Match3Item>();
                        selectedItem = item;
                        touchStartPos = Input.touchCount > 0 ? (Vector2)Input.GetTouch(0).position : (Vector2)Input.mousePosition;
                    }
                }
                if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp(0))
                {
                    if (touchStartPos == default || selectedItem == default) return;
                    touchEndPos = Input.touchCount > 0 ? (Vector2)Input.GetTouch(0).position : (Vector2)Input.mousePosition;
                    Vector2 swipeDirection = (touchEndPos - touchStartPos).normalized;
                    touchStartPos = touchEndPos = default;
                    Match3Item swapItem = null;
                    if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
                    {
                        if (swipeDirection.x > 0)
                        {
                            swapItem = gameBoard.GetValue(selectedItem.xIndex + 1, selectedItem.yIndex);
                        }
                        else
                        {
                            swapItem = gameBoard.GetValue(selectedItem.xIndex - 1, selectedItem.yIndex);
                        }
                    }
                    else
                    {
                        if (swipeDirection.y > 0)
                        {
                            swapItem = gameBoard.GetValue(selectedItem.xIndex, selectedItem.yIndex + 1);
                        }
                        else
                        {
                            swapItem = gameBoard.GetValue(selectedItem.xIndex, selectedItem.yIndex - 1);
                        }
                    }
                    if (swapItem != null)
                    {
                        StartProcessingMove();
                        SwapItem(swapItem, selectedItem);
                    }
                    selectedItem = null;
                }
            }
        }
    }

    void InitializeBoard()
    {
        if (gameBoard == null)
        {
            gameBoard = Spinach.Grid<Match3Item>.FitAndCenteredGrid(width, height);
            gameBoard.SetUsable(arrayLayout.ConvertToUsableArray());
        }
        DestroyItems();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = gameBoard.GetCellCenter(x, y);
                if (gameBoard.IsUsable(x, y))
                {
                    SpawnItemAtTop(x);
                }
            }
        }
        while (CheckBoard())
        {
            ReplaceMatchedItems();
        }
    }

    private void DestroyItems()
    {
        foreach (Match3Item item in gameBoard.Each())
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }
    }

    public bool CheckBoard()
    {
        bool hasMatched = false;

        itemsToRemove.Clear();

        foreach (Match3Item item in gameBoard.Each())
        {
            if (item != null)
            {
                item.isMatched = false;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (gameBoard.IsUsable(x, y))
                {
                    Match3Item item = gameBoard.GetValue(x, y);
                    if (!item.isMatched)
                    {
                        MatchResult matchedItems = IsConnected(item);
                        if (matchedItems.connectedItems.Count >= 3)
                        {
                            MatchResult superMatchedItems = SuperMatch(matchedItems);

                            itemsToRemove.AddRange(superMatchedItems.connectedItems);
                            foreach (Match3Item superItem in superMatchedItems.connectedItems)
                            {
                                superItem.isMatched = true;
                            }
                            hasMatched = true;
                        }
                    }
                }
            }
        }
        return hasMatched;
    }

    // this is done during board set up, so we start with a board that has no matches
    public void ReplaceMatchedItems()
    {
        foreach (Match3Item item in itemsToRemove)
        {
            item.isMatched = false;
        }
        RemoveAndRefill(itemsToRemove);
    }

    public IEnumerator ProcessTurnOnMatchedBoard(bool subtractMoves)
    {
        if (GameManager.instance.gameData.sound)
        {
            gameObject.GetComponent<AudioSource>().PlayOneShot(matchClip);
        }
        foreach (Match3Item item in itemsToRemove)
        {
            item.isMatched = false;
            if (item.altImage != null)
            {
                item.GetComponent<SpriteRenderer>().sprite = item.altImage;
            }
            Instantiate(matchParticlePrefab, new Vector3(item.transform.position.x, item.transform.position.y, 0), Quaternion.identity);
        }

        gameScene.ProcessTurn(itemsToRemove.Count, subtractMoves);
        yield return new WaitForSeconds(0.5f);

        RemoveAndRefill(itemsToRemove);

        yield return new WaitForSeconds(0.4f);

        if (CheckBoard())
        {
            StartCoroutine(ProcessTurnOnMatchedBoard(false));
        }
        else
        {
            gameScene.CheckGameOver();
            EndProcessingMove();
        }
    }

    private void StartProcessingMove()
    {
        if (isProcessingMove)
        {
            Debug.LogError("Attempted to start a new move while a move is already processing.");
            return;
        }

        isProcessingMove = true;
    }

    private void EndProcessingMove()
    {
        if (!isProcessingMove)
        {
            Debug.LogError("Attempted to end a move, but no move is currently processing.");
            return;
        }

        isProcessingMove = false;
    }

    #region Cascading Matches
    private void RemoveAndRefill(List<Match3Item> itemsToRemove)
    {
        foreach (Match3Item item in itemsToRemove)
        {
            gameBoard.SetValue(item.xIndex, item.yIndex, null);
            Destroy(item.gameObject);
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (gameBoard.IsUsable(x, y) && gameBoard.GetValue(x, y) == null)
                {
                    RefillItem(x, y);
                }
            }
        }
    }

    private void RefillItem(int x, int y)
    {
        int yOffset = 1;
        while (y + yOffset < height && gameBoard.GetValue(x, y + yOffset) == null)
        {
            yOffset++;
        }
        if (y + yOffset < height && gameBoard.GetValue(x, y + yOffset) != null)
        {
            // we've found an item
            Match3Item itemAbove = gameBoard.GetValue(x, y + yOffset);
            Vector3 targetPos = gameBoard.GetCellCenter(x, y);
            itemAbove.MoveToTarget(targetPos);
            itemAbove.SetIndices(x, y);
            gameBoard.SetValue(x, y, gameBoard.GetValue(x, y + yOffset));
            gameBoard.SetValue(x, y + yOffset, null);
        }
        if (y + yOffset == height)
        {
            SpawnItemAtTop(x);
        }
    }

    private void SpawnItemAtTop(int x)
    {
        int randomIndex = Random.Range(0, itemPrefabs.Length);
        GameObject newItem = Instantiate(itemPrefabs[randomIndex], gameBoard.GetCellCenter(x, height), Quaternion.identity);
        int index = FindIndexOfLowestNull(x);
        Vector3 targetPosition = gameBoard.GetCellCenter(x, index);
        newItem.transform.SetParent(itemParent.transform);
        var item = newItem.GetComponent<Match3Item>();
        item.SetIndices(x, index);
        gameBoard.SetValue(x, index, item);
        item.MoveToTarget(targetPosition);
    }

    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = height;
        for (int y = height - 1; y >= 0; y--)
        {
            if (gameBoard.GetValue(x, y) == null)
            {
                lowestNull = y;
            }
        }
        return lowestNull;
    }

    #endregion

    #region Matching Logic

    private MatchResult SuperMatch(MatchResult matchedItems)
    {
        if (matchedItems.direction == MatchDirection.Horizontal || matchedItems.direction == MatchDirection.LongHorizontal)
        {
            foreach (Match3Item item in matchedItems.connectedItems)
            {
                List<Match3Item> extraConnectedItems = new();
                CheckDirection(item, new Vector2Int(0, 1), extraConnectedItems);
                CheckDirection(item, new Vector2Int(0, -1), extraConnectedItems);

                if (extraConnectedItems.Count >= 2)
                {
                    extraConnectedItems.AddRange(matchedItems.connectedItems);
                    return new MatchResult { connectedItems = extraConnectedItems, direction = MatchDirection.Super };
                }
            }
        }
        else if (matchedItems.direction == MatchDirection.Vertical || matchedItems.direction == MatchDirection.LongVertical)
        {
            foreach (Match3Item item in matchedItems.connectedItems)
            {
                List<Match3Item> extraConnectedItems = new();
                CheckDirection(item, new Vector2Int(1, 0), extraConnectedItems);
                CheckDirection(item, new Vector2Int(-1, 0), extraConnectedItems);

                if (extraConnectedItems.Count >= 2)
                {
                    extraConnectedItems.AddRange(matchedItems.connectedItems);
                    return new MatchResult { connectedItems = extraConnectedItems, direction = MatchDirection.Super };
                }
            }
        }
        return new MatchResult { connectedItems = matchedItems.connectedItems, direction = matchedItems.direction };
    }

    MatchResult IsConnected(Match3Item item)
    {
        //initialize the list
        List<Match3Item> connectedItems = new() { item };

        //check horizontal
        CheckDirection(item, new Vector2Int(1, 0), connectedItems);
        CheckDirection(item, new Vector2Int(-1, 0), connectedItems);

        if (connectedItems.Count >= 3)
        {
            return new MatchResult { connectedItems = connectedItems, direction = connectedItems.Count > 3 ? MatchDirection.LongHorizontal : MatchDirection.Horizontal };
        }

        //reset the list
        connectedItems.Clear();
        connectedItems.Add(item);

        //check vertical
        CheckDirection(item, new Vector2Int(0, 1), connectedItems);
        CheckDirection(item, new Vector2Int(0, -1), connectedItems);
        if (connectedItems.Count >= 3)
        {
            return new MatchResult { connectedItems = connectedItems, direction = connectedItems.Count > 3 ? MatchDirection.LongVertical : MatchDirection.Vertical };
        }
        return new MatchResult { connectedItems = connectedItems, direction = MatchDirection.None };
    }

    void CheckDirection(Match3Item item, Vector2Int direction, List<Match3Item> connectedItems)
    {
        Match3ItemType itemType = item.itemType;
        int x = item.xIndex + direction.x;
        int y = item.yIndex + direction.y;

        while (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (gameBoard.IsUsable(x, y))
            {
                Match3Item neighborItem = gameBoard.GetValue(x, y);
                if (!neighborItem.isMatched && neighborItem.itemType == itemType)
                {
                    connectedItems.Add(neighborItem);
                    x += direction.x;
                    y += direction.y;
                    continue;
                }
            }
            break;
        }

    }

    #endregion

    #region Swapping Items

    private void SwapItem(Match3Item current, Match3Item target)
    {
        DoSwap(current, target);

        StartCoroutine(ProcessMatches(current, target));
    }

    private void DoSwap(Match3Item current, Match3Item target)
    {
        Match3Item temp = gameBoard.GetValue(current.xIndex, current.yIndex);

        gameBoard.SetValue(current.xIndex, current.yIndex, gameBoard.GetValue(target.xIndex, target.yIndex));
        gameBoard.SetValue(target.xIndex, target.yIndex, temp);

        int tempX = current.xIndex;
        int tempY = current.yIndex;

        current.SetIndices(target.xIndex, target.yIndex);
        target.SetIndices(tempX, tempY);

        current.MoveToTarget(gameBoard.GetValue(target.xIndex, target.yIndex).transform.position);
        target.MoveToTarget(gameBoard.GetValue(current.xIndex, current.yIndex).transform.position);
    }

    private bool IsAdjacent(Match3Item current, Match3Item target)
    {
        return Mathf.Abs(current.xIndex - target.xIndex) + Mathf.Abs(current.yIndex - target.yIndex) == 1;
    }

    // this happens only once per turn.  If the swap results in a match, we process the matches.  If not, we swap back.
    private IEnumerator ProcessMatches(Match3Item current, Match3Item target)
    {
        yield return new WaitForSeconds(0.2f);
        if (CheckBoard())
        {
            StartCoroutine(ProcessTurnOnMatchedBoard(true));
        }
        else
        {
            DoSwap(current, target);
            EndProcessingMove();
        }
    }

    #endregion
}

public class MatchResult
{
    public List<Match3Item> connectedItems;
    public MatchDirection direction;

}

public enum MatchDirection
{
    Vertical,
    Horizontal,
    LongVertical,
    LongHorizontal,
    Super,
    None
}