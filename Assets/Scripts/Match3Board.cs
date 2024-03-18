using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spinach;

public class Match3Board : MonoBehaviour
{
    public int width = 6;
    public int height = 8;
    public Match3Part itemPrefab;
    public Grid<Match3Part> gameBoard;
    public GameObject gameBoardGO;

    public GameObject background;
    public List<GameObject> itemsToDestroy = new();
    public GameObject itemContainer;

    public GameScene gameScene;

    [SerializeField]
    private Match3Part selectedItem;
    private Match3Part swapItem;

    // [SerializeField]
    // private bool isProcessingMove;
    [SerializeField]
    List<Match3Part> itemsToRemove = new();

    public ParticleSystem matchParticlePrefab;
    public AudioClip matchClip;

    [SerializeField]
    Material hilight;

    [SerializeField] private GoalTracker goalTracker;

    public ArrayLayout arrayLayout; // this is the layout of the board, true means unusable
    public static Match3Board Instance;

    // public bool ignoreInput = false;

    private void Awake()
    {
        Instance = this;
    }

    private Vector2 touchStartPos, touchEndPos = default;

    void Start()
    {
        arrayLayout = new ArrayLayout(height, width);
        InitializeBoard();
        gameScene.StateMachine.ChangeState(GameState.Idle);
    }

    void Update()
    {
        if (gameScene.StateMachine.CurrentState == GameState.Idle)
        {
            if (Input.touchCount > 0 || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
            // touch event detected
            {
                if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
                // swipe start detected
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                    if (hit.collider != null && hit.collider.GetComponent<Match3Part>() != null)
                    {
                        Match3Part item = hit.collider.gameObject.GetComponent<Match3Part>();
                        selectedItem = item;
                        selectedItem.gameObject.GetComponent<SpriteRenderer>().material = hilight;
                        touchStartPos = Input.touchCount > 0 ? (Vector2)Input.GetTouch(0).position : (Vector2)Input.mousePosition;
                    }
                }
                if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp(0))
                // swipe end detected
                {
                    if (touchStartPos == default || selectedItem == default) return;
                    touchEndPos = Input.touchCount > 0 ? (Vector2)Input.GetTouch(0).position : (Vector2)Input.mousePosition;
                    Vector2 swipeDirection = (touchEndPos - touchStartPos).normalized;

                    var minSwipeDistance = 0.5f;
                    if ((Camera.main.ScreenToWorldPoint(touchEndPos) - Camera.main.ScreenToWorldPoint(touchStartPos)).magnitude < minSwipeDistance)
                    {
                        touchStartPos = touchEndPos = default;
                        selectedItem.gameObject.GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
                        selectedItem = null;
                        return;
                    }

                    touchStartPos = touchEndPos = default;

                    swapItem = null;
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
                        selectedItem.gameObject.GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
                        gameScene.StateMachine.ChangeState(GameState.Checking);
                    }
                }
            }
        }
    }

    /// <summary>
    /// This is called by the state machine to reset input selections
    /// </summary>
    public void WaitForInput()
    {
        selectedItem = null;
        swapItem = null;
        touchStartPos = touchEndPos = default;
    }

    void InitializeBoard()
    {
        Match3Item[,] items = new BoardDefinition(width, height).InitializeRandomBoard(false);

        if (gameBoard == null)
        {
            gameBoard = Spinach.Grid<Match3Part>.FitAndCenteredGrid(width, height);
            gameBoard.SetUsable(arrayLayout.ConvertToUsableArray());
            background.transform.position = new Vector3(0, 1.5f, 9.5f);
            background.GetComponent<SpriteRenderer>().size = new Vector2((width + 1f), (height + 4.5f));
            background.transform.localScale = new Vector3(gameBoard.GetCellSize(), gameBoard.GetCellSize(), 1);
        }
        DestroyItems();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (gameBoard.IsUsable(x, y))
                {
                    SpawnItemAtTop(x, items[x, y]);
                }
            }
        }
    }

    private void DestroyItems()
    {
        foreach (Match3Part item in gameBoard.Each())
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

        foreach (Match3Part item in gameBoard.Each())
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
                    Match3Part item = gameBoard.GetValue(x, y);
                    if (!item.isMatched)
                    {
                        MatchResult matchedItems = IsConnected(item);
                        if (matchedItems.connectedItems.Count >= 3)
                        {
                            MatchResult superMatchedItems = SuperMatch(matchedItems);

                            itemsToRemove.AddRange(superMatchedItems.connectedItems);
                            foreach (Match3Part superItem in superMatchedItems.connectedItems)
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

    public IEnumerator ProcessTurnOnMatchedBoard()
    {
        List<Coroutine> itemAnimations = new();
        foreach (Match3Part item in itemsToRemove)
        {
            // set the destination based on the item type.  If there is a current goal >0 for the item type,
            // move it to the location of the goal display.  Otherwise, move it to the general score display
            Vector3 destination = new Vector3(2, 6, item.transform.position.z - 1);
            if (goalTracker.HasUnmetGoal(item.item))
            {
                var goaldest = goalTracker.GetGoalLocation(item.item).Value;
                if (goaldest != null) destination = goaldest;
            }
            itemAnimations.Add(StartCoroutine(item.YoureFired((Vector3)destination)));
        }
        if (GameManager.instance.gameData.Sound)
        {
            GameManager.instance.PlaySoundEffect(matchClip);
        }
        StartCoroutine(Spinach.Utils.WaitAndExecute(.5f, () => RemoveAndRefill(itemsToRemove)));

        foreach (Coroutine itemAnimation in itemAnimations)
        {
            yield return itemAnimation;
        }
        foreach (Match3Part item in itemsToRemove)
        {
            item.isMatched = false;
        }
        gameScene.ProcessTurn(itemsToRemove);

        if (CheckBoard())
        {
            StartCoroutine(ProcessTurnOnMatchedBoard());
        }
        else
        {
            if (gameScene.CheckWin())
            {
                gameScene.StateMachine.ChangeState(GameState.Win);
            }
            else if (gameScene.CheckFail())
            {
                gameScene.StateMachine.ChangeState(GameState.Fail);
            }
            else
            {
                gameScene.StateMachine.ChangeState(GameState.Idle);
            }
        }
    }

    #region Cascading Matches
    private void RemoveAndRefill(List<Match3Part> itemsToRemove)
    {
        foreach (Match3Part item in itemsToRemove)
        {
            gameBoard.SetValue(item.xIndex, item.yIndex, null);
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
            Match3Part itemAbove = gameBoard.GetValue(x, y + yOffset);
            Vector3 targetPos = gameBoard.GetCellCenter(x, y);
            StartCoroutine(itemAbove.MoveToTarget(targetPos));
            itemAbove.SetIndices(x, y);
            gameBoard.SetValue(x, y, gameBoard.GetValue(x, y + yOffset));
            gameBoard.SetValue(x, y + yOffset, null);
        }
        if (y + yOffset == height)
        {
            SpawnItemAtTop(x);
        }
    }

    private void SpawnItemAtTop(int x, Match3Item item = null)
    {
        Match3Part newItem = Instantiate(itemPrefab, gameBoard.GetCellCenter(x, height), Quaternion.identity);
        if (item != null)
        {
            newItem.Init(item);
        }
        else
        {
            newItem.Init();
        }

        newItem.transform.localScale = new Vector3(gameBoard.GetCellSize(), gameBoard.GetCellSize(), 1);

        int index = FindIndexOfLowestNull(x);
        gameBoard.SetValue(x, index, newItem);
        newItem.transform.SetParent(itemContainer.transform, false);
        Vector3 targetPosition = gameBoard.GetCellCenter(x, index);
        newItem.SetIndices(x, index);
        StartCoroutine(newItem.MoveToTarget(targetPosition));
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
            foreach (Match3Part item in matchedItems.connectedItems)
            {
                List<Match3Part> extraConnectedItems = new();
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
            foreach (Match3Part item in matchedItems.connectedItems)
            {
                List<Match3Part> extraConnectedItems = new();
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

    MatchResult IsConnected(Match3Part item)
    {
        //initialize the list
        List<Match3Part> connectedItems = new() { item };

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

    void CheckDirection(Match3Part part, Vector2Int direction, List<Match3Part> connectedItems)
    {
        int x = part.xIndex + direction.x;
        int y = part.yIndex + direction.y;

        while (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (gameBoard.IsUsable(x, y))
            {
                Match3Part neighborItem = gameBoard.GetValue(x, y);
                if (!neighborItem.isMatched && neighborItem.item.Equals(part.item))
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

    private IEnumerator DoSwap(Match3Part current, Match3Part target)
    {
        Match3Part temp = gameBoard.GetValue(current.xIndex, current.yIndex);

        gameBoard.SetValue(current.xIndex, current.yIndex, gameBoard.GetValue(target.xIndex, target.yIndex));
        gameBoard.SetValue(target.xIndex, target.yIndex, temp);

        int tempX = current.xIndex;
        int tempY = current.yIndex;

        current.SetIndices(target.xIndex, target.yIndex);
        target.SetIndices(tempX, tempY);

        StartCoroutine(current.MoveToTarget(gameBoard.GetValue(target.xIndex, target.yIndex).transform.position));
        StartCoroutine(target.MoveToTarget(gameBoard.GetValue(current.xIndex, current.yIndex).transform.position));

        while (current.isMoving || target.isMoving)
        {
            yield return null;
        }
    }

    private bool IsAdjacent(Match3Part current, Match3Part target)
    {
        return Mathf.Abs(current.xIndex - target.xIndex) + Mathf.Abs(current.yIndex - target.yIndex) == 1;
    }

    // this happens only once per turn.  If the swap results in a match, we process the matches.  If not, we swap back.
    public IEnumerator ValidateSwap()
    {
        yield return StartCoroutine(DoSwap(selectedItem, swapItem));
        if (CheckBoard())
        {
            gameScene.StateMachine.ChangeState(GameState.Matching);
        }
        else
        {
            //undo swap
            yield return StartCoroutine(DoSwap(swapItem, selectedItem));
            gameScene.StateMachine.ChangeState(GameState.Idle);
        }
    }

    #endregion
}

public class MatchResult
{
    public List<Match3Part> connectedItems;
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