/* Does all the enemy stuff - battling basicly*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Enemy : MonoBehaviour
{
    public string internalName = "slime.basic.1";
    public string displayName = "Basic Slime";
    public int level;

    public Animator animator;

    public BattleManager battleManager;
    public bool inBattle = false;
    bool movingToAttack = false;
    bool animatingAttack = false;
    bool movingFromAttack = false;
    int selectedAttack = 0;

    public string placeholderID;

    Vector2 battleMovingFrom;
    Vector2 battleMovingTo;
    float battleMovePercentage;
    public float battleMoveSpeed = 90f;

    public BattleClass currentClass;

    public int power;
    public int immunity;
    public int endurance;
    public int wisdom;
    public int luck;
    public int recovery;

    public Weapon currentWeapon;

    public int maxHealth = 50;
    public int currentHealth = 50;

    public int maxMana = 50;
    public int currentMana = 50;

    public bool isDead;

    public List<Armour.Stat> stats = new List<Armour.Stat>
    { 
        new Armour.Stat("power", 0),
        new Armour.Stat("immunity", 0),
        new Armour.Stat("endurance", 0),
        new Armour.Stat("wisdom", 0),
        new Armour.Stat("luck", 0),
        new Armour.Stat("recovery", 0)
    };

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

    void Start()
    {
        animator = GetComponent<Animator>();

        for (int i = 0; i < resistances.Count; i++)
        {
            Armour.Resistance res = resistances[i];
            Armour.Resistance newres = new Armour.Resistance();
            newres.name = res.name;
            for (int j = 0; j < currentWeapon.resistances.Count; j++)
            {
                Armour.Resistance resadd = currentWeapon.resistances[j];
                if (resadd.name == res.name)
                {
                    newres.value = resadd.value + res.value;
                    
                }
            }
            resistances[i] = newres;
        }


        for (int i = 0; i < stats.Count; i++)
        {
            Armour.Stat stat = stats[i];
            Armour.Stat newstat = new Armour.Stat();
            newstat.name = stat.name;
            for (int j = 0; j < currentWeapon.stats.Count; j++)
            {
                Armour.Stat statadd = currentWeapon.stats[j];
                if (statadd.name == stat.name)
                {
                    newstat.value = statadd.value + stat.value;
                }
            }
            stats[i] = newstat;
        }
    }

    void Update()
    {
        if (isDead)
        {
            animator.SetBool("dead", true);
        }
    }

    public void Dead()
    {
        battleManager.EnemyDead();
    }

    public void MoveToAttack()
    {
        battleMovingTo = GameObject.Find("PlayerSpot").transform.position;
        battleMovingFrom = GameObject.Find("EnemySpot").transform.position;
        battleMovingTo = new Vector2(battleMovingTo.x + 2.5f, battleMovingTo.y);
        battleMovePercentage = 0;
        Vector3 originalScale = transform.localScale;
        transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        movingToAttack = true;
        animator.SetBool("running", true);
    }

    public void AnimateAttack(int attackCode)
    {
        animatingAttack = true;
        selectedAttack = attackCode;
        animator.SetBool("attack" + selectedAttack.ToString(), true);
    }

    public void EndAnimateAttack()
    {
        animatingAttack = false;
        animator.SetBool("attack" + selectedAttack.ToString(), false);
        movingFromAttack = true;
        battleMovingFrom = GameObject.Find("PlayerSpot").transform.position;
        battleMovingTo = GameObject.Find("EnemySpot").transform.position;
        battleMovingFrom = new Vector2(battleMovingFrom.x + 2.5f, battleMovingFrom.y);
        battleMovePercentage = 0;
        Vector3 originalScale = transform.localScale;
        transform.localScale = new Vector3(Mathf.Abs(originalScale.x) * -1, originalScale.y, originalScale.z);
        animator.SetBool("running", true);
    }

    public void DoHit()
    {
        battleManager.EnemyDidHit();
    }

    private void FixedUpdate()
    {
        if (inBattle)
        {
            // BATTLE LOGIC
            if (movingToAttack)
            {
                if (battleMovePercentage >= 1)
                {
                    movingToAttack = false;
                    animator.SetBool("running", false);
                    battleManager.EnemyMovedToAttack();
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
                    battleManager.EnemyMovedFromAttack();
                    Vector3 originalScale = transform.localScale;
                    transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
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
    }
}