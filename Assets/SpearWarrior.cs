using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpearWarriorStatus{
    Idle = 0,
    Walking = 1,
    ShieldRaised = 2
}

public class SpearWarrior : MonoBehaviour, CanTakeDamage
{
    #region Variables
    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private SpearWarriorStatus currentStatus;
    [SerializeField] private int healthPoint;
    private Animator animator;

    private bool canMove = true;
    private bool canAttack = true;
    private bool canTurn = true;

    private bool isPlayerInRange = false;
    private Transform playerTransform;
    #endregion

    private void Start() {
        animator = GetComponent<Animator>();
        GetComponentInChildren<DetectionRange>().AddOnPlayerDetected(SetPlayerInRange);
        GetComponentInChildren<DetectionRange>().AddOnPlayerNotDetected(SetPlayerNotInRange);
        animator.SetBool("Patroling", true);
        currentStatus = SpearWarriorStatus.Walking;
    }

    void Update()
    {
        if (isPlayerInRange == false){
            // Set a patrol cycle between x= -8 & x= 10
            if (transform.position.x >= 10 && walkSpeed > 0){
                if (currentStatus != SpearWarriorStatus.Idle)
                    StartCoroutine("IdleAndTurn");
            }

            if (transform.position.x <= -8 && walkSpeed < 0){
                if (currentStatus != SpearWarriorStatus.Idle)
                    StartCoroutine("IdleAndTurn");
            }

            // Right mouse button input to test the Dash Attack
            if (Input.GetMouseButtonDown(1)){
                animator.SetTrigger("DashAttack");
                Debug.Log(Vector2.Distance(transform.position,playerTransform.position));
            }

            // Left mouse button input to start and stop the character
            if (Input.GetMouseButtonDown(0)){
                if (currentStatus == SpearWarriorStatus.Walking){
                    Stop();
                animator.SetBool("Patroling",false);
                currentStatus = SpearWarriorStatus.Idle;
                } 
                else if (currentStatus == SpearWarriorStatus.Idle){
                animator.SetBool("Patroling",true);
                currentStatus = SpearWarriorStatus.Walking;
                }
            }

            if (canMove && currentStatus == SpearWarriorStatus.Walking){
                GetComponent<Rigidbody2D>().velocity = new Vector2(walkSpeed,0);
            }
        }
        else{
            Fighting();
        }
    }

    #region FightingBehaviour
    private void Fighting(){
        // Shield bump anim on space button to test hit
        // Require the Shield Raised state
        if (Input.GetKeyDown(KeyCode.Space)){
            ShieldHit();
        }
        // Turn the character to face the player
        if (canAttack == true && transform.position.x+1 < playerTransform.position.x && transform.localScale.x == -1){
            Turn();
        }
        else if (canAttack == true && transform.position.x-1 > playerTransform.position.x && transform.localScale.x == 1){
            Turn();
        }
        else{
            // If the player is close enough we attack
            if (Vector2.Distance(transform.position,playerTransform.position) < 4){
                if (canAttack)
                    StartCoroutine("AttackAndShield");
            }
            // If the player is too far we chase him
            else{
                if (currentStatus == SpearWarriorStatus.Idle)
                    currentStatus = SpearWarriorStatus.Walking;
                if (currentStatus == SpearWarriorStatus.Walking)
                    GetComponent<Rigidbody2D>().velocity = new Vector2(walkSpeed,0);
            }
        }
    }

    // We attack and then put the shield up right after for 1 to 2.5 sec
    IEnumerator AttackAndShield(){
        canAttack = false;
        animator.SetTrigger("DashAttack");
        yield return new WaitForSeconds(1);
        animator.SetTrigger("RaiseShield");
        currentStatus = SpearWarriorStatus.ShieldRaised;
        yield return new WaitForSeconds(Random.Range(1,2.5f));
        animator.SetTrigger("PutShieldBack");
        currentStatus = SpearWarriorStatus.Idle;
    }

    public void SetCanAttack(){
        canAttack = true;
    }

    public void TakeHit(int value){
        healthPoint -= value;
    }

    private void SetPlayerInRange(GameObject player){
        isPlayerInRange = true;
        playerTransform = player.transform;
    }

    private void SetPlayerNotInRange(){
        isPlayerInRange = false;
        playerTransform = null;
    }
    #endregion

    IEnumerator IdleAndTurn(){
        currentStatus = SpearWarriorStatus.Idle;
        Stop();
        canMove = false;
        animator.SetBool("Patroling",false);
        yield return new WaitForSeconds(2);
        Turn();
        // another wait for smoother animation
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("Patroling",true);
        canMove = true;
        currentStatus = SpearWarriorStatus.Walking;
    }

    // Function called in the animation "Turn"
    public void ReturnSpriteHorizontally(){
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y,transform.localScale.z);
        // Add somes issues with pivot point so i repaired it this way
        // Get child with the spriteRenderer
        Transform childTransform = transform.GetChild(0);
        childTransform.localPosition = new Vector3(childTransform.localPosition.x != 0 ? 0 : 1.15f,childTransform.localPosition.y,childTransform.localPosition.z);
        // Get child with the spear collider
        Transform spearTransform = transform.GetChild(2);
        spearTransform.localPosition = new Vector3(spearTransform.localPosition.x != 0 ? 0 : -1.15f,spearTransform.localPosition.y,spearTransform.localPosition.z);
        // Get child with the shield collider
        Transform shieldTransform = transform.GetChild(3);
        shieldTransform.localPosition = new Vector3(shieldTransform.localPosition.x != 0 ? 0 : -1.15f,shieldTransform.localPosition.y,shieldTransform.localPosition.z);
    }

    public void Turn(){
        if (canTurn){
            StartCoroutine("WaitBeforeNextTurn");
            Stop();
            canMove = false;
            walkSpeed *= -1;
            animator.SetTrigger("Turn");
        }
    }

    // Set a little waiting to avoid a infinity turn loop
    IEnumerator WaitBeforeNextTurn(){
        canTurn = false;
        yield return new WaitForSeconds(1f);
        canTurn = true;
    }

    public void SetCanMoveTrue(){
        canMove = true;
    }

    public void SetCanMoveFalse(){
        canMove = false;
    }

    public void Stop(){
        GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
    }

    public void Dash(){
            GetComponent<Rigidbody2D>().velocity = new Vector2(walkSpeed * dashSpeed,0);
    }

    public void ShieldHit(){
        animator.SetTrigger("ShieldBump");
    }
}
