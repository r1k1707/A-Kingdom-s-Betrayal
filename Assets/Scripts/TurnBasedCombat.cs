using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TurnBasedCombat : MonoBehaviour
//Note to self: Next time you make a turn-based system, separate the characters' functions T_T
{
    private PartyMember pendingHealer = null; // who can use the healing spell
    private bool selectingHealTarget = false; // need to select who to heal
    private bool enemyusedAOE = false;

    private void Update()
    {
        if (selectingHealTarget && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                foreach (var member in partyMembers)
                {
                    if (hit.collider.gameObject == member.characterObject)
                    {
                        pendingHealer.Heal(this, member);
                        selectingHealTarget = false;
                        pendingHealer = null;
                        return;
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class PartyMember
    {
        public string name;
        public GameObject characterObject;
        public int health;
        public int maxHealth;
        public TextMeshProUGUI healthText;
        public int speed;
        public Button attackButton;
        public Button guardButton;
        public Button healButton;
        public Button fireballButton;

        public bool isAlive => health > 0;

        public void Heal(TurnBasedCombat battleSystem, PartyMember target)
        {
            if (!isAlive) return;

            int healAmount = 5;
            target.health += healAmount;
            if (target.health > target.maxHealth)
                target.health = target.maxHealth;

            Debug.Log($"{name} healed {target.name}!");
            battleSystem.UpdateUI();
            battleSystem.NextTurn();
        }

        public void CastFireball(TurnBasedCombat battleSystem)
        {
            if (battleSystem.enemyHealth <= 0)
            {
                Debug.Log("Enemy has fallen!");
                battleSystem.Victory();
                return;
            }

            if (!isAlive) return;
            battleSystem.StartCoroutine(battleSystem.DoAttack(characterObject, battleSystem.Enemy, () =>
            {
                int dmg = 12;
                battleSystem.enemyHealth -= dmg;
                if (battleSystem.enemyHealth < 0) battleSystem.enemyHealth = 0;

                Debug.Log($"Fireball hits the enemy for {dmg} damage! Enemy HP = {battleSystem.enemyHealth}");

                if (battleSystem.enemyHealth <= 0)
                {
                    Animator enemyAnim = battleSystem.Enemy.GetComponent<Animator>();
                    if (enemyAnim != null)
                        enemyAnim.SetTrigger("Death");

                    battleSystem.Victory();
                    return;
                }
                battleSystem.UpdateUI();
                battleSystem.NextTurn();
            }));

        }

        public void TakeTurn(TurnBasedCombat battleSystem)
        {
            if (battleSystem.enemyHealth <= 0)
            {
                Debug.Log("Enemy has fallen!");
                battleSystem.Victory();
                return;
            }
            if (!isAlive) return;

            battleSystem.StartCoroutine(battleSystem.DoAttack(characterObject, battleSystem.Enemy, () =>
            {
                battleSystem.enemyHealth -= 5;
                if (battleSystem.enemyHealth < 0) battleSystem.enemyHealth = 0;
                {
                    Debug.Log($"{name} attacked! Enemy HP = {battleSystem.enemyHealth}");
                }

                if (battleSystem.enemyHealth <= 0)
                {
                    Animator enemyAnim = battleSystem.Enemy.GetComponent<Animator>();
                    if (enemyAnim != null)
                        enemyAnim.SetBool("isDead", true);

                    battleSystem.Victory();
                    return;
                }

                battleSystem.NextTurn();
                battleSystem.UpdateUI();
            }));

        }

        public void SetButtonsActive(bool active)
        {
            attackButton.gameObject.SetActive(active);
            guardButton.gameObject.SetActive(active);
            healButton.gameObject.SetActive(active);
            fireballButton.gameObject.SetActive(active);
        }
    }
    public bool isAlive => enemyHealth > 0;

    [Header("Party & Enemy Setup")]
    [SerializeField] private List<PartyMember> partyMembers = new List<PartyMember>();
    [SerializeField] private GameObject Enemy;
    [SerializeField] private int enemyHealth = 200;
    [SerializeField] private int maxEnemyHealth = 200;
    [SerializeField] private int enemySpeed = 5;
    [SerializeField] private TextMeshProUGUI enemyHealthText;

    [Header("End Screens")]
    [SerializeField] private GameObject victoryScreen;

    private Dictionary<GameObject, Vector3> startPositions = new Dictionary<GameObject, Vector3>();
    private Vector3 enemyStartPosition;
    private Queue<object> turnQueue = new Queue<object>();

    private void Start()
    {
        foreach (var member in partyMembers)
        {
            startPositions[member.characterObject] = member.characterObject.transform.position;

            // Hide all buttons at start
            member.SetButtonsActive(false);
        }

        enemyStartPosition = Enemy.transform.position;
        UpdateUI();
        TurnOrder();
        NextTurn();
    }

    public void NextTurn()
    {
        // Hide all member's buttons
        foreach (var member in partyMembers)
        {
            member.SetButtonsActive(false);
        }

        if (turnQueue.Count == 0)
        {
            TurnOrder();
        }

        object current = turnQueue.Dequeue();

        // PARTY MEMBER TURN
        if (current is PartyMember pm && pm.isAlive)
        {
            Debug.Log($"{pm.name}'s turn!");
            pm.SetButtonsActive(true);

            pm.attackButton.onClick.RemoveAllListeners();
            pm.guardButton.onClick.RemoveAllListeners();
            pm.healButton.onClick.RemoveAllListeners();
            pm.fireballButton?.onClick.RemoveAllListeners();

            // Normal attack
            pm.attackButton.onClick.AddListener(() =>
            {
                pm.TakeTurn(this);
                pm.SetButtonsActive(false);
            });



            // Heal for only Support
            if (pm.name == "Support")
            {
                pm.healButton.onClick.AddListener(() =>
                {
                    Debug.Log("Select an ally to heal");
                    pendingHealer = pm;
                    selectingHealTarget = true;
                    Animator anim = pm.characterObject.GetComponent<Animator>();
                    if (anim != null)
                    {
                        anim.SetTrigger("Heal");
                    }

                    pm.SetButtonsActive(false);
                });
            }
            else
            {
                pm.healButton.gameObject.SetActive(false);
            }

            // Fireball for Mage only
            if (pm.name == "Mage")
            {
                pm.fireballButton.gameObject.SetActive(true);
                pm.fireballButton.onClick.AddListener(() =>
                {
                    pm.CastFireball(this);
                    pm.SetButtonsActive(false);
                });
            }
            else
            {
                if (pm.fireballButton != null)
                    pm.fireballButton.gameObject.SetActive(false);
            }

            // Guard
            pm.guardButton.onClick.AddListener(() =>
            {
                Debug.Log($"{pm.name} is guarding!");
                pm.SetButtonsActive(false);
                NextTurn();
            });
        }
        // ENEMY TURN
        else if (current is GameObject go && go == Enemy && enemyHealth > 0)
        {
            Debug.Log("Enemy's turn!");
            EnemyAttack();
        }
        else
        {
            NextTurn();
        }
    }
    private IEnumerator EnemyAOEAttack(List<PartyMember> targets)
    {
        foreach (PartyMember target in targets)
        {
            yield return DoAttack(Enemy, target.characterObject, () =>
            {
                target.health -= 5;
                if (target.health < 0) target.health = 0;
                Debug.Log($"Enemy hits {target.name} with AOE! {target.name} HP = {target.health}");
            });
        }

        UpdateUI();
        NextTurn();
    }

    public void EnemyAttack()
    {
        List<PartyMember> aliveMembers = partyMembers.FindAll(m => m.isAlive);

        if (aliveMembers.TrueForAll(m => !m.isAlive))
        {
            Debug.Log("All party members are down!");
            return;
        }

        // First attack is AOE
        if (!enemyusedAOE)
        {
            Debug.Log("Enemy uses AOE attack!");
            enemyusedAOE = true;
            StartCoroutine(EnemyAOEAttack(aliveMembers));
        }
        else
        {
            // Normal single-target attack
            PartyMember target = aliveMembers[Random.Range(0, aliveMembers.Count)];

            StartCoroutine(DoAttack(Enemy, target.characterObject, () =>
            {
                target.health -= 3;
                if (target.health < 0) target.health = 0;

                Debug.Log($"Enemy attacked {target.name}! {target.name} HP = {target.health}");
                UpdateUI();

                if (aliveMembers.TrueForAll(m => !m.isAlive))
                {
                    Debug.Log("All party members are down!");
                    return;
                }

                NextTurn();
            }));
        }
    }

    IEnumerator DoAttack(GameObject attacker, GameObject target, System.Action onComplete)
    {
        Animator anim = attacker.GetComponent<Animator>();
        // Trigger animation
        if (anim != null)
            anim.SetTrigger("Attack");

        // For the enemy's Hit Animation to activate instead of just push back
        Animator targetAnim = target.GetComponent<Animator>();
        if (targetAnim != null)
            targetAnim.SetTrigger("Hit");

        Vector3 attackerStart = attacker == Enemy ? enemyStartPosition : startPositions[attacker];
        Vector3 targetStart = target == Enemy ? enemyStartPosition : startPositions[target];

        Vector3 attackPos = attackerStart + (targetStart - attackerStart).normalized * 0.5f;
        Vector3 hitPushPos = targetStart + (targetStart - attackerStart).normalized * 0.3f;

        // Move forward a little
        yield return MoveOverTime(attacker, attackerStart, attackPos, 0.15f);
        yield return MoveOverTime(target, targetStart, hitPushPos, 0.05f); //push back the target being hit
        yield return MoveOverTime(target, hitPushPos, targetStart, 0.1f); //return the target back to its original position
        yield return new WaitForSeconds(0.3f); //need a delay to time the attack with the move forward animation

        // Move attacker back
        yield return MoveOverTime(attacker, attackPos, attackerStart, 0.1f);

        onComplete.Invoke();
    }


    IEnumerator MoveOverTime(GameObject obj, Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            obj.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.position = endPos;
    }

    public void UpdateUI()
    {
        foreach (PartyMember member in partyMembers)
        {
            member.healthText.text = $"HP: {member.health}/{member.maxHealth}";
        }
        enemyHealthText.text = $"HP: {enemyHealth}/{maxEnemyHealth}";
    }

    public void TurnOrder()
    {
        List<object> players = new List<object>();
        players.AddRange(partyMembers);
        players.Add(Enemy);

        players.Sort((first, second) =>
        {
            int speedFirst = first is PartyMember ? ((PartyMember)first).speed : enemySpeed;
            int speedSecond = second is PartyMember ? ((PartyMember)second).speed : enemySpeed;
            return speedSecond.CompareTo(speedFirst);
        });

        turnQueue.Clear();
        foreach (var unit in players)
        {
            turnQueue.Enqueue(unit);
        }
    }
    public void Victory()
    {
        if (victoryScreen != null && !victoryScreen.activeSelf)
        {
            Debug.Log("Victory!");
            StopAllCoroutines();

            foreach (var member in partyMembers)
                member.SetButtonsActive(false);

            victoryScreen.SetActive(true);

            // Prevent further turns like "STOP FIGHTING, THIS ISN'T YOU"
            turnQueue.Clear();
            enabled = false;
        }
    }
}
