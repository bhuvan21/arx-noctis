/*Manages the player's click and move controls, and attacking*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CoreCharacterController : MonoBehaviour
{
    public bool canMove = true;
    bool unstickMovement = true;

    Vector2  mousePos;
    bool mouseDown;
    bool mouseDownDispatched = true;
    Vector2 movingFrom;
    Vector2 movingTo;
    float movePercentage;
    float moveSpeed = 10.5f;

    public Vector2 prebattlePos;
    public string prebattleSceneName;

    Vector2 battleMovingFrom;
    Vector2 battleMovingTo;
    float battleMovePercentage;
    float battleMoveSpeed = 92.5f;

    public Animator animator;
    public BoxCollider2D collider;
    public GameObject spawner;

    public BattleManager battleManager;
    public bool inBattle = false;
    bool movingToAttack = false;
    bool movingFromAttack = false;
    bool exitingBattle;
    string selectedAttack;

    BattleClass ruffinClass;
    public BattleClass currentClass;

    public int statMaximum = 200;
    public string displayName = "Bhuvan";
    public List<Armour.Resistance> resistances = new List<Armour.Resistance>
    {
        new Armour.Resistance("fire", 0),
        new Armour.Resistance("water", 0),
        new Armour.Resistance("darkness", 0),
        new Armour.Resistance("light", 0),
        new Armour.Resistance("wind", 0),
        new Armour.Resistance("stone", 0),
        new Armour.Resistance("ice", 0),
        new Armour.Resistance("nature", 0),
        new Armour.Resistance("metal", 0),
    };

    private static CoreCharacterController playerInstance;

    string enemyName;
    string enemyID;

    public Quest currentQuest;

    AsyncOperation asyncLoadLevel;

    Vector3 oldScale;

    private void Start()
    {   
        currentClass = (BattleClass)Resources.Load("Ruffian");
    }

    // prevents duplicates of the player
    void Awake()
    {
        DontDestroyOnLoad(this);
        if (playerInstance == null)
        {
            playerInstance = this;
        }
        else
        {
            Object.Destroy(gameObject);
        }
    }

    // get user input and a little other stuff
    void Update()
    {
        if (inBattle == false)
        {
            if (mouseDownDispatched == true)
            {
                mouseDown = Input.GetMouseButtonDown(0);
            }

            if (mouseDown)
            {
                mouseDownDispatched = false;
            }
            mousePos = Input.mousePosition;
            if (movePercentage > 0 && movePercentage < 1)
            {
                animator.SetBool("running", true);
            }
            else
            {
                animator.SetBool("running", false);
            }
        }
    }

    public void stopMoving()
    {
        movingTo = transform.position;
    }

    // small battle funcs - keeps it flowing
    public void MoveToAttack()
    {
        battleMovingFrom = GameObject.Find("PlayerSpot").transform.position;
        battleMovingTo = GameObject.Find("EnemySpot").transform.position;
        battleMovingTo = new Vector2(battleMovingTo.x - 2.5f, battleMovingTo.y);
        battleMovePercentage = 0;
        Vector3 originalScale = transform.localScale;
        transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        movingToAttack = true;
        animator.SetBool("running", true);
    }

    public void AnimateAttack(string animName)
    {
        selectedAttack = animName;
        animator.SetBool(selectedAttack, true);
    }

    public void EndAnimateAttack()
    {
        animator.SetBool(selectedAttack, false);
        movingFromAttack = true;

        battleMovingTo = GameObject.Find("PlayerSpot").transform.position;
        battleMovingFrom = GameObject.Find("EnemySpot").transform.position;
        battleMovingFrom = new Vector2(battleMovingFrom.x - 2.5f, battleMovingFrom.y);
        battleMovePercentage = 0;
        Vector3 originalScale = transform.localScale;
        transform.localScale = new Vector3(Mathf.Abs(originalScale.x)*-1, originalScale.y, originalScale.z);
        animator.SetBool("running", true);
    }

    public void DoHit()
    {
        battleManager.PlayerDidHit();
    }

    // battle logic and movement
    void FixedUpdate()
    {
        mouseDownDispatched = true;
        // BATTLE LOGIC
        if (inBattle)
        {
            if (movingToAttack)
            {
                if (battleMovePercentage >= 1)
                {
                    movingToAttack = false;
                    animator.SetBool("running", false);
                    battleManager.PlayerMovedToAttack();
                }
                else
                {
                    float increment = 1 / Vector2.Distance(battleMovingFrom, battleMovingTo);
                    if (increment > 1)
                    {
                        increment = 1;
                    }
                    increment = increment / (100 - battleMoveSpeed);
                    battleMovePercentage += increment;
                    Vector2 newPos = Vector2.Lerp(battleMovingFrom, battleMovingTo, battleMovePercentage);
                    transform.position = newPos;
                }
            }
            else if (movingFromAttack)
            {
                if (battleMovePercentage >= 1)
                {
                    movingFromAttack = false;
                    animator.SetBool("running", false);
                    Vector3 originalScale = transform.localScale;
                    transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
                    battleManager.PlayerMovedFromAttack();
                }
                else
                {
                    float increment = 1 / Vector2.Distance(battleMovingFrom, battleMovingTo);
                    if (increment > 1)
                    {
                        increment = 1;
                    }
                    increment = increment / (100 - battleMoveSpeed);
                    battleMovePercentage += increment;
                    Vector2 newPos = Vector2.Lerp(battleMovingFrom, battleMovingTo, battleMovePercentage);
                    transform.position = newPos;
                }
            }
        }

        // i have no idea what this does but it's movement related
        if (mouseDown && canMove && !EventSystem.current.IsPointerOverGameObject())
        {
            unstickMovement = true;
            movePercentage = 0;
            movingFrom = transform.position;
            movingTo = Camera.main.ScreenToWorldPoint(mousePos);
            Vector3 originalScale = transform.localScale;
            Vector3 newScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            if (movingTo.x < movingFrom.x)
            {
                transform.localScale = new Vector3(-1 * newScale.x, newScale.y, newScale.z);
            }
            else
            {
                transform.localScale = newScale;
            }
        }
        else if (movePercentage < 1.0 && canMove)
        {
            float increment = 1 / Vector2.Distance(movingFrom, movingTo);
            if (increment > 1)
            {
                increment = 1;
            }
            increment = increment / moveSpeed;
            movePercentage += increment;
            Vector2 newPos = Vector2.Lerp(movingFrom, movingTo, movePercentage);
            this.gameObject.GetComponent<Rigidbody2D>().MovePosition(newPos);
        }
        else if (movePercentage >= 1)
        {
            movingFrom = transform.position;
        }
    }

    // handles the player hitting nps/location swaps
    void OnTriggerEnter2D(Collider2D col)
    {
        if (exitingBattle && col.gameObject.GetComponent<Enemy>() != null)
        {
            Destroy(col.gameObject);
            exitingBattle = false;
            return;
        }
        if (col.gameObject.GetComponent<InteractionManager>() != null)
        {
            if (col.gameObject.GetComponent<InteractionManager>().interactionType == "endq")
            {
                GameObject myPrefab = Resources.Load("Prefabs/BigScroll", typeof(GameObject)) as GameObject;
                GameObject scroll = Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                scroll.transform.Find("Name").GetComponent<TextMesh>().text = currentQuest.questName;
                scroll.transform.Find("Name").GetComponent<Renderer>().sortingLayerName = "Popups";
                scroll.transform.Find("Name").GetComponent<Renderer>().sortingOrder = 1000;
                scroll.transform.Find("Description").GetComponent<TextMeshPro>().text = currentQuest.questDesc;
                scroll.transform.Find("Description").GetComponent<Renderer>().sortingLayerName = "Popups";
                scroll.transform.Find("Description").GetComponent<Renderer>().sortingOrder = 1000;
                scroll.transform.position = new Vector3(0, 0, 0);
                myPrefab = Resources.Load("Prefabs/MenuButton", typeof(GameObject)) as GameObject;
                GameObject button = Instantiate(myPrefab, Camera.main.WorldToScreenPoint(new Vector3(0, -3, 0)), Quaternion.identity);
                button.transform.Find("Text").GetComponent<Text>().text = "Close";
                button.transform.parent = GameObject.Find("Canvas").transform;
                button.transform.localScale = new Vector3(1, 1, 1);
                button.GetComponent<Button>().onClick.AddListener(() => EndQuest(col));
                button.GetComponent<Button>().onClick.AddListener(() => Destroy(scroll));
                button.GetComponent<Button>().onClick.AddListener(() => Destroy(button));
            }
            else
            {
                Scene current = SceneManager.GetActiveScene();
                SceneManager.LoadScene(col.gameObject.GetComponent<InteractionManager>().sceneName);
                transform.position = col.gameObject.GetComponent<InteractionManager>().goTo;
                transform.localScale = col.gameObject.GetComponent<InteractionManager>().scaleTo;
                if (col.gameObject.GetComponent<InteractionManager>().interactionType == "npc")
                {
                    canMove = false;
                }
            }
        }
        else if (col.gameObject.GetComponent<Enemy>() != null && inBattle == false)
        {
            enemyName = col.gameObject.GetComponent<Enemy>().displayName;
            enemyID = col.gameObject.GetComponent<Enemy>().placeholderID;

            IEnumerator coroutine = LoadBattle();
            StartCoroutine(coroutine);
        }
        movePercentage = 1;
        movingFrom = transform.position;
    }

    // small things
    private void OnCollisionEnter2D(Collision2D collision)
    {
        movePercentage = 1;
        movingFrom = transform.position;
        unstickMovement = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (unstickMovement == false)
        {
            movePercentage = 1;
            movingFrom = transform.position;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        movePercentage = 1;
        movingFrom = transform.position;
    }

    public void EndBattle(string placeID)
    {
        IEnumerator coroutine = UnloadBattle(placeID);
        StartCoroutine(coroutine);
    }

    public void EndQuest(Collider2D col)
    {
        SceneManager.LoadScene(col.gameObject.GetComponent<InteractionManager>().sceneName);
        transform.position = col.gameObject.GetComponent<InteractionManager>().goTo;
    }

    IEnumerator UnloadBattle(string place)
    {
        exitingBattle = true;
        transform.position = prebattlePos;
        asyncLoadLevel = SceneManager.LoadSceneAsync(prebattleSceneName, LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }
        canMove = true;
        inBattle = false;
        movingFromAttack = false;
        transform.localScale = oldScale;
        spawner.GetComponent<EnemySpawner>().DestroyEnemy(place);
    }

    IEnumerator LoadBattle()
    {
        oldScale = transform.localScale;
        prebattlePos = transform.position;
        prebattleSceneName = SceneManager.GetActiveScene().name;
        asyncLoadLevel = SceneManager.LoadSceneAsync("battleground", LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }
        transform.position = GameObject.Find("PlayerSpot").transform.position;
        GameObject myPrefab = Resources.Load("Prefabs/" + enemyName.Replace(" ", ""), typeof(GameObject)) as GameObject;
        GameObject enemy = Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        enemy.transform.position = GameObject.Find("EnemySpot").transform.position;
        enemy.transform.parent = GameObject.Find("EnemySpot").transform;
        enemy.GetComponent<Enemy>().placeholderID = enemyID;
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        battleManager.player = this.gameObject;
        battleManager.enemy = enemy;
        battleManager.StartBattle();
        canMove = false;
        inBattle = true;
    }

    private void OnLevelWasLoaded(int level)
    {
        if (this.gameObject.GetComponent<StatDisplayManager>() != null)
        {
            this.gameObject.GetComponent<StatDisplayManager>().startUpdate();
        } 
    }

}
