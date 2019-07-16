using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;

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

    GameObject playerDamage;
    GameObject enemyDamage;

    Attack playerAttack;
    Attack enemyAttack;

    Vector3 oldScale;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void CleanUp()
    {
        for (int i = 0; i < cooldowns.Length; i++)
        {
            cooldowns[i] = 0;
        }
    }

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
        playerHealthBar = GameObject.Find("Healthbar");
        playerManaBar = GameObject.Find("Manabar");
        enemyHealthBar = GameObject.Find("Healthbar2");
        enemyManaBar = GameObject.Find("Manabar2");
        enemyName = GameObject.Find("EnemyName");
        hackyShow(enemyHealthBar);
        hackyShow(enemyManaBar);
        hackyShow(enemyName);
        enemyName.GetComponent<Text>().text = enemy.GetComponent<Enemy>().displayName;

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
        BattleClass playerClass = player.GetComponent<CharacterMovementController>().currentClass;
        for (int i = 0; i < attackButtons.Count; i++)
        {
            GameObject button = attackButtons[i];
            button.GetComponent<Image>().color = Color.gray;
            
            button.GetComponent<Button>().enabled = false;
            button.gameObject.transform.Find("Label").gameObject.SetActive(false);
            button.gameObject.transform.Find("Mana").gameObject.SetActive(false);
            button.gameObject.transform.Find("Cooldown").gameObject.SetActive(false);
        }
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
                //button.GetComponent<Button>().AddListener(() => ButtonHighlighted(button, playerClass.attacks[i]));
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

    public void PlayerMovedToAttack()
    {
        player.GetComponent<CharacterMovementController>().AnimateAttack(playerAttack.animName);
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

    public void ButtonAttack0Pressed()
    {
        Attack[] attacks = player.GetComponent<CharacterMovementController>().currentClass.attacks;
        HideButtons();
        for (int i = 0; i < attacks.Length; i++)
        {
            Attack attack = attacks[i]; ;
            if (attack.placement == 0)
            {
                playerAttack = attack;
            }
        }
        player.GetComponent<CharacterMovementController>().MoveToAttack();
    }

    public void ButtonAttackM1Pressed()
    {
        Attack[] attacks = player.GetComponent<CharacterMovementController>().currentClass.attacks;
        HideButtons();
        for (int i = 0; i < attacks.Length; i++)
        {
            Attack attack = attacks[i]; ;
            if (attack.placement == -1)
            {
                playerAttack = attack;
            }
        }
        player.GetComponent<CharacterMovementController>().MoveToAttack();
    }

    public void ButtonAttackPressed(int attackPlacement)
    {
        Attack[] attacks = player.GetComponent<CharacterMovementController>().currentClass.attacks;

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
                    player.GetComponent<CharacterMovementController>().MoveToAttack();
                }
            }
        }
    }



    public void PlayerDidHit()
    {
        // Use weapon to get random base damage
        int max = player.GetComponent<CharacterMovementController>().statMaximum;
        Weapon weapon = player.GetComponent<InventoryManager>().currentWeapon;
        int baseDamage = Random.Range(weapon.minimum, weapon.maximum + 1);
        float modified = baseDamage * playerAttack.multiplier;

        // Apply power modifier
        modified = modified * (1.0f + (player.GetComponent<InventoryManager>().getStat("power") / max * 2.0f));

        // Delete previous damage indicators
        GameObject enemySpot = GameObject.Find("EnemyDamageSpot");
        foreach (Transform child in enemySpot.transform)
        {
            Destroy(child.gameObject);
        }

        // Create damage indicator
        GameObject myPrefab = Resources.Load("Prefabs/Damage", typeof(GameObject)) as GameObject;
        GameObject damage = Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        damage.transform.position = enemySpot.transform.position;
        damage.transform.parent = enemySpot.transform;
        damage.GetComponent<TextMesh>().characterSize = 0.5f;

        // Check if critical, and apply crit modifier
        int criticalRoll = Random.Range(0, 100);
        if (criticalRoll <= (float)player.GetComponent<InventoryManager>().getStat("luck") / 200.0f*40.0f)
        {
            float criticalModifier = (((float)player.GetComponent<InventoryManager>().getStat("power") / 200.0f) * 1.75f) + 1.75f;
            modified = modified * criticalModifier;
            damage.GetComponent<TextMesh>().characterSize = 1.0f;
        }

        // Apply enemy resistances
        foreach (Armour.Resistance resist in enemy.GetComponent<Enemy>().resistances)
        {
            if (resist.name == player.GetComponent<InventoryManager>().currentWeapon.element && resist.value != 0)
            {
                modified = modified * (1.0f - ((float)resist.value / 100.0f));
            }
        }

        int final = Mathf.CeilToInt(modified);
        damage.GetComponent<TextMesh>().text = final.ToString();

        audio.Play();

        Destroy(damage, 0.5f);

        enemy.GetComponent<Enemy>().currentHealth -= final;
        if (enemy.GetComponent<Enemy>().currentHealth <=0)
        {
            enemy.GetComponent<Enemy>().currentHealth = 0;
            enemy.GetComponent<Enemy>().isDead = true;
            hackyHide(enemyHealthBar);
            hackyHide(enemyManaBar);
            hackyHide(enemyName);
            //player.transform.localScale = oldScale;
        }
        UpdatePointBars();
    }

    public void EnemyDidHit()
    {
        Weapon weapon = enemy.GetComponent<Enemy>().currentWeapon;
        int baseDamage = Random.Range(weapon.minimum, weapon.maximum + 1);
        float modified = baseDamage * enemyAttack.multiplier;
        foreach (Armour.Resistance resist in player.GetComponent<CharacterMovementController>().resistances)
        {
            if (resist.name == enemy.GetComponent<Enemy>().currentWeapon.element && resist.value != 0)
            {
                modified = modified * (1.0f - ((float)resist.value / 100.0f));
            }
        }

        int final = Mathf.CeilToInt(modified);

        GameObject myPrefab = Resources.Load("Prefabs/Damage", typeof(GameObject)) as GameObject;
        GameObject damage = Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        damage.transform.position = GameObject.Find("PlayerDamageSpot").transform.position;
        damage.GetComponent<TextMesh>().text = final.ToString();
        damage.GetComponent<TextMesh>().characterSize = 0.5f;
        Destroy(damage, 0.5f);

        player.GetComponent<InventoryManager>().currentHealth -= final;
        UpdatePointBars();
    }

    public void EnemyDead()
    {
        string placeID = enemy.GetComponent<Enemy>().placeholderID;
        Destroy(enemy);
        player.GetComponent<CharacterMovementController>().inBattle = false;
        player.GetComponent<CharacterMovementController>().EndBattle(placeID);
        CleanUp();
        enemy = null;
    }

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


}
