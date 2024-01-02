using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PotionBoard : MonoBehaviour
{
    //define the size of the board
    public int width = 6;
    public int height = 8;
    //get a reference to our potion prefabs
    public GameObject[] potionPrefabs;
    //get a reference to the collection nodes potionBoard + GO
    //public Node[,] potionBoard;
    public Spinach.Grid<Node> potionBoard;
    public GameObject potionBoardGO;
    public List<GameObject> potionsToDestroy = new();
    public GameObject potionParent;

    public GameScene gameScene;

    [SerializeField]
    private Potion selectedPotion;
    [SerializeField]
    private bool isProcessingMove;
    [SerializeField]
    List<Potion> potionsToRemove = new();

    public ParticleSystem matchParticlePrefab;
    public AudioClip matchClip;

    //layoutArray
    public ArrayLayout arrayLayout;
    //public static of potionBoard
    public static PotionBoard Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        arrayLayout = new ArrayLayout(height, width);
        InitializeBoard();
    }

    void Update()
    {
        if (!isProcessingMove)
        {
            if (Input.touchCount > 0 || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
            {
                if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
                {
                    if (isProcessingMove) return;
                    Ray ray = Camera.main.ScreenPointToRay(Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                    if (hit.collider != null && hit.collider.GetComponent<Potion>() != null)
                    {
                        Potion potion = hit.collider.gameObject.GetComponent<Potion>();
                        selectedPotion = potion;
                    }
                }

                if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp(0))
                {
                    if (isProcessingMove) return;
                    Ray ray = Camera.main.ScreenPointToRay(Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                    if (hit.collider != null && hit.collider.GetComponent<Potion>() != null)
                    {
                        Potion potion = hit.collider.gameObject.GetComponent<Potion>();
                        if (potion != selectedPotion)
                        {
                            SwapPotion(potion, selectedPotion);
                        }
                    }
                    selectedPotion = null;
                }
            }
        }
    }

    void InitializeBoard()
    {
        if (potionBoard == null)
        {
            potionBoard = Spinach.Grid<Node>.FitAndCenteredGrid(width, height);
        }
        DestroyPotions();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = potionBoard.GetCellCenter(x, y);
                if (arrayLayout.rows[y].row[x]) // true means no potion
                {
                    potionBoard.SetValue(x, y, new Node(false, null));
                }
                else
                {
                    SpawnPotionAtTop(x);
                    // int randomIndex = Random.Range(0, potionPrefabs.Length);
                    // GameObject potion = Instantiate(potionPrefabs[randomIndex], position, Quaternion.identity);
                    // potion.transform.SetParent(potionParent.transform);
                    // potion.GetComponent<Potion>().SetIndices(x, y);
                    // potionBoard.SetValue(x, y, new Node(true, potion));
                    //potionsToDestroy.Add(potion);
                }
            }
        }
        while (CheckBoard())
        {
            Debug.Log("Board has matches, adjusting...");
            ReplaceMatchedPotions();
        }

        Debug.Log("Board has no matches.  Ready to play.");

    }

    private void DestroyPotions()
    {
        foreach (Node node in potionBoard.Each())
        {
            if (node?.potion != null)
            {
                Destroy(node.potion);
            }
        }
    }

    public bool CheckBoard()
    {
        Debug.Log("Checking Board");
        bool hasMatched = false;

        potionsToRemove.Clear();

        foreach (Node nodePotion in potionBoard.Each())
        {
            if (nodePotion.potion != null)
            {
                nodePotion.potion.GetComponent<Potion>().isMatched = false;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (potionBoard.GetValue(x, y).isUsable)
                {
                    Potion potion = potionBoard.GetValue(x, y).potion.GetComponent<Potion>();
                    if (!potion.isMatched)
                    {
                        MatchResult matchedPotions = IsConnected(potion);
                        if (matchedPotions.connectedPotions.Count >= 3)
                        {
                            MatchResult superMatchedPotions = SuperMatch(matchedPotions);

                            potionsToRemove.AddRange(superMatchedPotions.connectedPotions);
                            foreach (Potion pot in superMatchedPotions.connectedPotions)
                            {
                                pot.isMatched = true;
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
    public void ReplaceMatchedPotions()
    {
        foreach (Potion potion in potionsToRemove)
        {
            potion.isMatched = false;
        }
        RemoveAndRefill(potionsToRemove);
    }

    public IEnumerator ProcessTurnOnMatchedBoard(bool subtractMoves)
    {
        if (GameManager.instance.gameData.sound)
        {
            gameObject.GetComponent<AudioSource>().PlayOneShot(matchClip);
        }
        foreach (Potion potion in potionsToRemove)
        {
            potion.isMatched = false;
            if (potion.altImage != null)
            {
                potion.GetComponent<SpriteRenderer>().sprite = potion.altImage;
            }
            Instantiate(matchParticlePrefab, new Vector3(potion.transform.position.x, potion.transform.position.y, 0), Quaternion.identity);
        }

        gameScene.ProcessTurn(potionsToRemove.Count, subtractMoves);
        yield return new WaitForSeconds(0.5f);

        RemoveAndRefill(potionsToRemove);

        yield return new WaitForSeconds(0.4f);

        if (CheckBoard())
        {
            StartCoroutine(ProcessTurnOnMatchedBoard(false));
        }
        else
        {
            gameScene.CheckGameOver();
        }
    }

    #region Cascading Potions
    private void RemoveAndRefill(List<Potion> potionsToRemove)
    {
        foreach (Potion potion in potionsToRemove)
        {
            potionBoard.GetValue(potion.xIndex, potion.yIndex).potion = null;
            Destroy(potion.gameObject);
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (potionBoard.GetValue(x, y).isUsable && potionBoard.GetValue(x, y).potion == null)
                {
                    RefillPotion(x, y);
                }
            }
        }
    }

    private void RefillPotion(int x, int y)
    {
        int yOffset = 1;
        while (y + yOffset < height && potionBoard.GetValue(x, y + yOffset).potion == null)
        {
            yOffset++;
        }
        if (y + yOffset < height && potionBoard.GetValue(x, y + yOffset).potion != null)
        {
            // we've found a potion
            Potion potionAbove = potionBoard.GetValue(x, y + yOffset).potion.GetComponent<Potion>();
            Vector3 targetPos = potionBoard.GetCellCenter(x, y);// new Vector3(x - spacingX, y - spacingY, potionAbove.transform.position.z);
            potionAbove.MoveToTarget(targetPos);
            potionAbove.SetIndices(x, y);
            potionBoard.SetValue(x, y, potionBoard.GetValue(x, y + yOffset));
            potionBoard.SetValue(x, y + yOffset, new Node(true, null));
        }
        if (y + yOffset == height)
        {
            SpawnPotionAtTop(x);
        }
    }

    private void SpawnPotionAtTop(int x)
    {
        int randomIndex = Random.Range(0, potionPrefabs.Length);
        GameObject newPotion = Instantiate(potionPrefabs[randomIndex], potionBoard.GetCellCenter(x, height), Quaternion.identity);
        int index = FindIndexOfLowestNull(x);
        Vector3 targetPosition = potionBoard.GetCellCenter(x, index);
        newPotion.transform.SetParent(potionParent.transform);
        newPotion.GetComponent<Potion>().SetIndices(x, index);
        potionBoard.SetValue(x, index, new Node(true, newPotion));
        newPotion.GetComponent<Potion>().MoveToTarget(targetPosition);
    }

    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = height;
        for (int y = height - 1; y >= 0; y--)
        {
            if (potionBoard.GetValue(x, y)?.potion == null)
            {
                lowestNull = y;
            }
        }
        return lowestNull;
    }

    #endregion

    #region Matching Logic

    private MatchResult SuperMatch(MatchResult matchedPotions)
    {
        if (matchedPotions.direction == MatchDirection.Horizontal || matchedPotions.direction == MatchDirection.LongHorizontal)
        {
            foreach (Potion pot in matchedPotions.connectedPotions)
            {
                List<Potion> extraConnectedPotions = new();
                CheckDirection(pot, new Vector2Int(0, 1), extraConnectedPotions);
                CheckDirection(pot, new Vector2Int(0, -1), extraConnectedPotions);

                if (extraConnectedPotions.Count >= 2)
                {
                    extraConnectedPotions.AddRange(matchedPotions.connectedPotions);
                    return new MatchResult { connectedPotions = extraConnectedPotions, direction = MatchDirection.Super };
                }
            }
        }
        else if (matchedPotions.direction == MatchDirection.Vertical || matchedPotions.direction == MatchDirection.LongVertical)
        {
            foreach (Potion pot in matchedPotions.connectedPotions)
            {
                List<Potion> extraConnectedPotions = new();
                CheckDirection(pot, new Vector2Int(1, 0), extraConnectedPotions);
                CheckDirection(pot, new Vector2Int(-1, 0), extraConnectedPotions);

                if (extraConnectedPotions.Count >= 2)
                {
                    extraConnectedPotions.AddRange(matchedPotions.connectedPotions);
                    return new MatchResult { connectedPotions = extraConnectedPotions, direction = MatchDirection.Super };
                }
            }
        }
        return new MatchResult { connectedPotions = matchedPotions.connectedPotions, direction = matchedPotions.direction };
    }

    MatchResult IsConnected(Potion potion)
    {
        //initialize the list
        List<Potion> connectedPotions = new();
        connectedPotions.Add(potion);

        //check horizontal
        CheckDirection(potion, new Vector2Int(1, 0), connectedPotions);
        CheckDirection(potion, new Vector2Int(-1, 0), connectedPotions);

        if (connectedPotions.Count >= 3)
        {
            return new MatchResult { connectedPotions = connectedPotions, direction = connectedPotions.Count > 3 ? MatchDirection.LongHorizontal : MatchDirection.Horizontal };
        }

        //reset the list
        connectedPotions.Clear();
        connectedPotions.Add(potion);

        //check vertical
        CheckDirection(potion, new Vector2Int(0, 1), connectedPotions);
        CheckDirection(potion, new Vector2Int(0, -1), connectedPotions);
        if (connectedPotions.Count >= 3)
        {
            return new MatchResult { connectedPotions = connectedPotions, direction = connectedPotions.Count > 3 ? MatchDirection.LongVertical : MatchDirection.Vertical };
        }
        return new MatchResult { connectedPotions = connectedPotions, direction = MatchDirection.None };
    }

    void CheckDirection(Potion pot, Vector2Int direction, List<Potion> connectedPotions)
    {
        PotionType potionType = pot.potionType;
        int x = pot.xIndex + direction.x;
        int y = pot.yIndex + direction.y;

        while (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (potionBoard.GetValue(x, y).isUsable)
            {
                Potion neighborPotion = potionBoard.GetValue(x, y).potion.GetComponent<Potion>();
                if (!neighborPotion.isMatched && neighborPotion.potionType == potionType)
                {
                    connectedPotions.Add(neighborPotion);
                    x += direction.x;
                    y += direction.y;
                    continue;
                }
            }
            break;
        }

    }

    #endregion

    #region Swapping Potions

    private void SwapPotion(Potion current, Potion target)
    {
        if (!IsAdjacent(current, target))
        {
            return;
        }

        DoSwap(current, target);

        isProcessingMove = true;
        StartCoroutine(ProcessMatches(current, target));
    }

    private void DoSwap(Potion current, Potion target)
    {
        GameObject temp = potionBoard.GetValue(current.xIndex, current.yIndex).potion;

        potionBoard.GetValue(current.xIndex, current.yIndex).potion = potionBoard.GetValue(target.xIndex, target.yIndex).potion;
        potionBoard.GetValue(target.xIndex, target.yIndex).potion = temp;

        int tempX = current.xIndex;
        int tempY = current.yIndex;

        current.SetIndices(target.xIndex, target.yIndex);
        target.SetIndices(tempX, tempY);

        current.MoveToTarget(potionBoard.GetValue(target.xIndex, target.yIndex).potion.transform.position);
        target.MoveToTarget(potionBoard.GetValue(current.xIndex, current.yIndex).potion.transform.position);
    }

    private bool IsAdjacent(Potion current, Potion target)
    {
        return Mathf.Abs(current.xIndex - target.xIndex) + Mathf.Abs(current.yIndex - target.yIndex) == 1;
    }

    // this happens only once per turn.  If the swap results in a match, we process the matches.  If not, we swap back.
    private IEnumerator ProcessMatches(Potion current, Potion target)
    {
        yield return new WaitForSeconds(0.2f);
        if (CheckBoard())
        {
            StartCoroutine(ProcessTurnOnMatchedBoard(true));
        }
        else
        {
            DoSwap(current, target);
        }

        isProcessingMove = false;
    }

    #endregion
}

public class MatchResult
{
    public List<Potion> connectedPotions;
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