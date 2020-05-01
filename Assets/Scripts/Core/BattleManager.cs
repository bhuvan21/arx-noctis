/* Where the management of the battle happens*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BattleManager : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;

    public GameObject canvas;

    // This was the easiest way to do this (M = minus)
    List<GameObject> attackButtons = new List<GameObject>();
    public GameObject buttonAttack0;
    public GameObject buttonAttackM1;
    public GameObject buttonAttackM2;
    public GameObject buttonAttackM3;
    public GameObject buttonAttackM4;
    public GameObject buttonAttackM5;
    public GameObject buttonAttackM6;
    public GameObject buttonAttackM7;

    public GameObject buttonAttack1;
    public GameObject buttonAttack2;
    public GameObject buttonAttack3;
    public GameObject buttonAttack4;
    public GameObject buttonAttack5;
    public GameObject buttonAttack6;
    public GameObject buttonAttack7;

    public AudioSource audio;

    int[] cooldowns = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    public GameObject playerHealthBar;
    public GameObject playerManaBar;
    public GameObject enemyHealthBar;
    public GameObject enemyManaBar;
    public GameObject enemyName;

    // the damage indicators
    GameObject playerDamage;
    GameObject enemyDamage;

    Attack playerAttack;
    Attack enemyAttack;

    Vector3 oldScale;

    string placeID;

    // reset all the cooldowns
    void CleanUp()
    {
        for (int i = 0; i < cooldowns.Length; i++)
        {
            cooldowns[i] = 0;
        }
    }

    // easy way to show and hide
    void hackyShow(GameObject obj)
    {
        Vector3 copy = obj.transform.position;
        copy = new Vector3(copy.x, copy.y, 0);
        obj.transform.position = copy;
    }

    void hackyHide(GameObject obj)
    {
        Vector3 copy = obj.transform.position;
        copy = new Vector3(copy.x, copy.y, -10000);
        obj.transform.position = copy;
    }

    public void StartBattle()
    {
        // get the display bars and show them
        playerHealthBar = GameObject.Find("Healthbar");
        playerManaBar = GameObject.Find("Manabar");
        enemyHealthBar = GameObject.Find("Healthbar2");
        enemyManaBar = GameObject.Find("Manabar2");
        enemyName = GameObject.Find("EnemyName");
        hackyShow(enemyHealthBar);
        hackyShow(enemyManaBar);
        hackyShow(enemyName);

        // show the enemy's name
        enemyName.GetComponent<Text>().text = "Level " + enemy.GetComponent<Enemy>().level.ToString() + " " + enemy.GetComponent<Enemy>().displayName;

        attackButtons.Add(buttonAttackM7);
        attackButtons.Add(buttonAttackM6);
        attackButtons.Add(buttonAttackM5);
        attackButtons.Add(buttonAttackM4);
        attackButtons.Add(buttonAttackM3);
        attackButtons.Add(buttonAttackM2);
        attackButtons.Add(buttonAttackM1);

        attackButtons.Add(buttonAttack1);
        attackButtons.Add(buttonAttack2);
        attackButtons.Add(buttonAttack3);
        attackButtons.Add(buttonAttack4);
        attackButtons.Add(buttonAttack5);
        attackButtons.Add(buttonAttack6);
        attackButtons.Add(buttonAttack7);

        UpdatePointBars();
        enemy.GetComponent<Enemy>().battleManager = this;
        enemy.GetComponent<Enemy>().inBattle = true;

        oldScale = player.transform.localScale;
        player.transform.localScale = new Vector3(1, 1, 1);
        BattleClass playerClass = player.GetComponent<CoreCharacterController>().currentClass;

        // update the attack buttons for the player's class
        for (int i = 0; i < attackButtons.Count; i++)
        {
            GameObject button = attackButtons[i];
            button.GetComponent<Image>().color = Color.gray;
            
            button.GetComponent<Button>().enabled = false;
            button.gameObject.transform.Find("Label").gameObject.SetActive(false);
            button.gameObject.transform.Find("Mana").gameObject.SetActive(false);
            button.gameObject.transform.Find("Cooldown").gameObject.SetActive(false);
        }

        // make the buttons actually work, and have them appear right for cooldowns etc
        for (int i = 0; i < playerClass.attacks.Length; i++)
        {
            if (playerClass.attacks[i].placement != 0)
            {
                GameObject button = attackButtons[playerClass.attacks[i].placement + 7];
                if (playerClass.attacks[i].placement > 0)
                {
                    button = attackButtons[playerClass.attacks[i].placement + 6];
                }
                GameObject text = button.gameObject.transform.Find("Label").gameObject;
                text.GetComponent<Text>().text = playerClass.attacks[i].name;
                text.SetActive(true);
                button.gameObject.transform.Find("Mana").gameObject.SetActive(true);
                button.gameObject.transform.Find("Mana").GetComponent<Text>().text = playerClass.attacks[i].mana.ToString();
                button.GetComponent<AttackButtonPreview>().attackName = playerClass.attacks[i].name;
                button.GetComponent<AttackButtonPreview>().attackDesc = playerClass.attacks[i].desc;
                if (playerClass.attacks[i].startingCooldown > 0)
                {
                    button.GetComponent<Image>().color = Color.green;
                    if (playerClass.attacks[i].placement > 0)
                    {
                        cooldowns[playerClass.attacks[i].placement + 6] = playerClass.attacks[i].startingCooldown;
                    }
                    else
                    {
                        cooldowns[playerClass.attacks[i].placement + 7] = playerClass.attacks[i].startingCooldown;
                    }
                    button.gameObject.transform.Find("Cooldown").gameObject.SetActive(true);
                    button.gameObject.transform.Find("Cooldown").GetComponent<Text>().text = playerClass.attacks[i].startingCooldown.ToString();
                }
                else
                {
                    button.GetComponent<Image>().color = Color.red;
                    button.GetComponent<Button>().enabled = true;
                }
            }
            else
            {
                buttonAttack0.GetComponent<AttackButtonPreview>().attackName = playerClass.attacks[i].name;
                buttonAttack0.GetComponent<AttackButtonPreview>().attackDesc = playerClass.attacks[i].desc;
            }
        }
    }

    // the small funcs that let battle flow
    public void PlayerMovedToAttack()
    {
        player.GetComponent<CoreCharacterController>().AnimateAttack(playerAttack.animName);
    }

    public void PlayerMovedFromAttack()
    {

        Attack[] attacks = enemy.GetComponent<Enemy>().currentClass.attacks;
        for (int i = 0; i < attacks.Length; i++)
        {
            Attack attack = attacks[i]; ;
            if (attack.placement == 0)
            {
                enemyAttack = attack;
            }
        }
        enemy.GetComponent<Enemy>().MoveToAttack();
    }

    public void EnemyMovedToAttack()
    {
        enemy.GetComponent<Enemy>().AnimateAttack(0);
    }

    // run at the end of each turn
    // cooldowns updated etc.
    public void EnemyMovedFromAttack()
    {
        for (int i = 0; i < cooldowns.Length; i++)
        {
            GameObject button = attackButtons[i];
            if (cooldowns[i] != 0)
            {
                cooldowns[i] -= 1;
                button.gameObject.transform.Find("Cooldown").GetComponent<Text>().text = cooldowns[i].ToString();
            }

            if (cooldowns[i] == 0)
            {
                button.gameObject.transform.Find("Cooldown").GetComponent<Text>().text = cooldowns[i].ToString();
                if (button.GetComponent<Image>().color != Color.grey)
                {
                    button.GetComponent<Image>().color = Color.red;
                }
                button.GetComponent<Button>().enabled = true;
                button.gameObject.transform.Find("Cooldown").gameObject.SetActive(false);
            }
        }

        if (playerAttack.cooldown > 0) {
            GameObject button;
            if (playerAttack.placement > 0)
            {
                cooldowns[playerAttack.placement + 6] = playerAttack.cooldown;
                button = attackButtons[playerAttack.placement + 6];
            }
            else
            {
                cooldowns[playerAttack.placement + 7] = playerAttack.cooldown;
                button = attackButtons[playerAttack.placement + 7];
            }
            button.gameObject.transform.Find("Cooldown").gameObject.SetActive(true);
            button.gameObject.transform.Find("Cooldown").GetComponent<Text>().text =playerAttack.cooldown.ToString();
            button.GetComponent<Image>().color = Color.green;
        }

        ShowButtons();
    }

    public void ButtonAttackPressed(int attackPlacement)
    {
        Attack[] attacks = player.GetComponent<CoreCharacterController>().currentClass.attacks;

        for (int i = 0; i < attacks.Length; i++)
        {
            Attack attack = attacks[i];
            if (attack.placement == attackPlacement)
            {
                if (player.GetComponent<InventoryManager>().currentMana >= attack.mana)
                {
                    HideButtons();
                    playerAttack = attack;
                    player.GetComponent<InventoryManager>().currentMana -= attack.mana;
                    UpdatePointBars();
                    player.GetComponent<CoreCharacterController>().MoveToAttack();
                }
            }
        }
    }

    // where the player damage happens
    public void PlayerDidHit()
    {
        // Use weapon to get random base damage
        int max = player.GetComponent<CoreCharacterController>().statMaximum;
        Weapon weapon = player.GetComponent<InventoryManager>().currentWeapon;

        DamageInstance dmg =  CalculateDamage(max, weapon, playerAttack, true, enemy.GetComponent<Enemy>().resistances);

        CreateDamageIndicator(dmg.value, true, dmg.critical);

        enemy.GetComponent<Enemy>().currentHealth -= dmg.value;
        if (enemy.GetComponent<Enemy>().currentHealth <=0)
        {
            enemy.GetComponent<Enemy>().currentHealth = 0;
            enemy.GetComponent<Enemy>().isDead = true;
            hackyHide(enemyHealthBar);
            hackyHide(enemyManaBar);
            hackyHide(enemyName);
        }
        UpdatePointBars();
    }

    // where the enemy damage happens (for now) TODO make player and enemy damage the same
    public void EnemyDidHit()
    {
        Weapon weapon = enemy.GetComponent<Enemy>().currentWeapon;
        DamageInstance dmg = CalculateDamage(player.GetComponent<CoreCharacterController>().statMaximum, weapon, enemyAttack, false, player.GetComponent<CoreCharacterController>().resistances);

        CreateDamageIndicator(dmg.value, false, dmg.critical);

        player.GetComponent<InventoryManager>().currentHealth -= dmg.value;
        UpdatePointBars();
    }

    public void CreateDamageIndicator(int dmg, bool isPlayer, bool critical)
    {
        GameObject spot;

        // Destroy old damage indicators
        if (isPlayer)
        {
            spot = GameObject.Find("PlayerDamageSpot");
        }
        else
        {
            spot = GameObject.Find("EnemyDamageSpot");
        }
        
        foreach (Transform child in spot.transform)
        {
            Destroy(child.gameObject);
        }

        GameObject myPrefab = Resources.Load("Prefabs/DamageIndicator", typeof(GameObject)) as GameObject;
        GameObject damage = Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        damage.transform.position = new Vector3(spot.transform.position.x + Random.Range(50, 50), spot.transform.position.y + Random.Range(50, 50), 0);
        damage.layer = 5;
        damage.gameObject.transform.SetParent(canvas.transform);
        damage.GetComponent<TextMeshProUGUI>().text = dmg.ToString();

        if (critical)
        {
            damage.GetComponent<TextMeshProUGUI>().fontSize = damage.GetComponent<TextMeshPro>().fontSize * 2.0f;
        }

        audio.Play();
        Destroy(damage, 0.25f);
    }


    public DamageInstance CalculateDamage (int statMax, Weapon weapon, Attack attack, bool isPlayer, List<Armour.Resistance> res)
    {
        DamageInstance dmg = new DamageInstance();


        int power;
        int luck;
        if (isPlayer)
        {
            power = player.GetComponent<InventoryManager>().getStat("power");
            luck = player.GetComponent<InventoryManager>().getStat("luck");

        }
        else
        {
            power = enemy.GetComponent<Enemy>().power;
            luck = enemy.GetComponent<Enemy>().luck;
        }

        int baseDamage = Random.Range(weapon.minimum, weapon.maximum + 1);
        float modified = baseDamage * attack.multiplier;

        // Apply power modifier
        modified = modified * (1.0f + (power / statMax * 2.0f));

        dmg.critical = false;
        dmg.element = weapon.element;

        // Check if critical, and apply crit modifier
        int criticalRoll = Random.Range(0, 100);
        if (criticalRoll <= (float)luck / 200.0f * 40.0f)
        {
            float criticalModifier = (((float)power / 200.0f) * 1.75f) + 1.75f;
            modified = modified * criticalModifier;
            dmg.critical = true;
        }

        // Apply enemy resistances
        foreach (Armour.Resistance resist in res)
        {
            if (resist.name == weapon.element && resist.value != 0)
            {
                modified = modified * (1.0f - ((float)resist.value / 100.0f));
            }
        }

        int final = Mathf.CeilToInt(modified);
        dmg.value = final;
        return dmg;
    }

    public void EnemyDead()
    {
        GameObject myPrefab = Resources.Load("Prefabs/BigScroll", typeof(GameObject)) as GameObject;
        GameObject scroll = Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        scroll.transform.Find("Name").GetComponent<TextMesh>().text = "Battle Over!";
        scroll.transform.Find("Name").GetComponent<Renderer>().sortingLayerName = "Popups";
        scroll.transform.Find("Name").GetComponent<Renderer>().sortingOrder = 1000;
        player.GetComponent<InventoryManager>().gold += CalculateGold();
        player.GetComponent<InventoryManager>().xp += CalculateXP();
        scroll.transform.Find("Description").GetComponent<TextMeshPro>().text = "Epic win! You defeated " + enemy.GetComponent<Enemy>().displayName + "!\nGold : " + CalculateGold().ToString() +  "\nXP : " + CalculateXP().ToString();
        scroll.transform.Find("Description").GetComponent<Renderer>().sortingLayerName = "Popups";
        scroll.transform.Find("Description").GetComponent<Renderer>().sortingOrder = 1000;
        scroll.transform.position = new Vector3(0, 0, 0);
        myPrefab = Resources.Load("Prefabs/MenuButton", typeof(GameObject)) as GameObject;
        GameObject button = Instantiate(myPrefab, Camera.main.WorldToScreenPoint(new Vector3(0, -2, 0)), Quaternion.identity);
        button.transform.Find("Text").GetComponent<Text>().text = "OK";
        button.transform.parent = GameObject.Find("Canvas").transform;
        button.transform.localScale = new Vector3(.5f, .5f, .5f);

        placeID = enemy.GetComponent<Enemy>().placeholderID;
        Destroy(enemy);

        button.GetComponent<Button>().onClick.AddListener(() => ExitBattle());
        button.GetComponent<Button>().onClick.AddListener(() => Destroy(scroll));
        button.GetComponent<Button>().onClick.AddListener(() => Destroy(button));
        
    }

    void ExitBattle()
    {
        player.GetComponent<CoreCharacterController>().inBattle = false;
        player.GetComponent<CoreCharacterController>().EndBattle(placeID);
        CleanUp();
        enemy = null;
    }

    // update the indicators
    void UpdatePointBars()
    {
        float current = player.GetComponent<InventoryManager>().currentHealth;
        float max = player.GetComponent<InventoryManager>().maxHealth;
        float perc = current / max;
        playerHealthBar.GetComponent<PointBar>().SetBar(perc);
        playerHealthBar.GetComponent<PointBar>().SetText(player.GetComponent<InventoryManager>().currentHealth.ToString());

        current = enemy.GetComponent<Enemy>().currentHealth;
        max = enemy.GetComponent<Enemy>().maxHealth;
        perc = current / max;
        enemyHealthBar.GetComponent<PointBar>().SetBar(perc);
        enemyHealthBar.GetComponent<PointBar>().SetText(enemy.GetComponent<Enemy>().currentHealth.ToString());

        current = enemy.GetComponent<Enemy>().currentMana;
        max = enemy.GetComponent<Enemy>().maxMana;
        perc = current / max;
        enemyManaBar.GetComponent<PointBar>().SetBar(perc);
        enemyManaBar.GetComponent<PointBar>().SetText(enemy.GetComponent<Enemy>().currentMana.ToString());

        current = player.GetComponent<InventoryManager>().currentMana;
        max = player.GetComponent<InventoryManager>().maxMana;
        perc = current / max;
        playerManaBar.GetComponent<PointBar>().SetBar(perc);
        playerManaBar.GetComponent<PointBar>().SetText(player.GetComponent<InventoryManager>().currentMana.ToString());
    }

    void HideButtons()
    {
        for (int i = 0; i < attackButtons.Count; i++)
        {
            GameObject button = attackButtons[i];
            button.SetActive(false);
        }
        buttonAttack0.SetActive(false);
    }

    void ShowButtons()
    {
        for (int i = 0; i < attackButtons.Count; i++)
        {
            GameObject button = attackButtons[i];
            button.SetActive(true);
        }
        buttonAttack0.SetActive(true);
    }

    int CalculateGold()
    {
        return enemy.GetComponent<Enemy>().maxHealth / 5;
    }

    int CalculateXP()
    {
        return enemy.GetComponent<Enemy>().level * 5;
    }

}

public class DamageInstance
{
    public bool critical;
    public int value;
    public string element;
}