using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteHitFlash;

public class CombatTestDummy : MonoBehaviour, IDamageable
{

    [SerializeField] private GameObject hitParticles;

    private Animator anim;
    private SpriteRenderer sprite;
    private Controller2D controller;
    private Player player;
    private HitFlash hitFlash;
    private LineRenderer lineRenderer;

    private Vector2 velocity;
    private Vector2 hitlagPosition;

    private float maxHealth = 100;
    private float currentHealth;
    private float weight = 100;

    //these should come from attack
    private Vector2 kbAngle;
    private float kbAngleDeg;
    private float kbScaling;
    private float kbBase;

    private float knockback;
    private float damageTaken;

    private int timer;
    private int hitCooldownTime;
    private int hitstunTime;
    private int hitlag;

    private bool inHitstun;
    private bool canBeHit;

    private List<Vector3> linePoints = new List<Vector3>();

    private void Awake() {
        player = GameObject.Find("Player").GetComponent<Player>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        controller = GetComponent<Controller2D>();
        hitFlash = GetComponent<HitFlash>();
        lineRenderer = GetComponent<LineRenderer>();

        canBeHit = true;

        currentHealth = maxHealth;
    }

    private void Update() {

        controller.Move(velocity * Time.deltaTime, Vector2.zero);

        if (controller.collisions.below) {
            velocity.y = 0;
        }
        if (controller.collisions.left || controller.collisions.right) {    //hit wall bounce off
            velocity.x *= -0.6f;
            Instantiate(hitParticles, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        }
    }

    private void FixedUpdate() {

        if (inHitstun) {
            timer++;
        }
        else if (timer > hitstunTime) {
            inHitstun = false;
        }

        if (timer < hitlag) {
            anim.speed = 0;
            transform.position = hitlagPosition;
        }
        else if (timer == hitlag && hitlag != 0) {
            anim.speed = 1; //hitlag over = unfreeze
            ApplyKnockBack(ref velocity);

            if (hitstunTime > 35) {
                CreateDustTrail();
            }

            //hitFlash.Flash(hitStunColor, hitstunTime);
        }

        velocity.y -= 1.5f;   //gravity

        if (timer > hitCooldownTime) {
            canBeHit = true;
        }

        if (!controller.collisions.below) {
            if (velocity.x > 0) {   //airdrift
                velocity.x -= .3f;
                if (velocity.x < 0) {
                    velocity.x = 0;
            }
            }
            else {
                velocity.x += .3f;
                if (velocity.x > 0) {
                    velocity.x = 0;
                }
            }
        }
        else {
            reduceByTraction(ref velocity.x, false);
        } 
    }


    public void Damage(float amount, float baseKnockBack, float knockbackScaling, Vector2 knockbackAngle)
    {
        if (!canBeHit) {
            return;
        }

        canBeHit = false;

        currentHealth -= amount;
        damageTaken = amount;

        kbAngle = knockbackAngle.normalized;
        kbAngleDeg = Vector2.Angle(Vector2.right, kbAngle);
        //Debug.Log(kbAngleDeg);
        kbScaling = knockbackScaling;
        kbBase = baseKnockBack;
        hitlagPosition = transform.position;

        if (currentHealth <= 0) {
            currentHealth = 0;
        }

        timer = 0;

        CalculateKnockBack();

        hitFlash.Flash(Color.white, hitlag);

        Debug.Log("<color=red>Damage Taken: </color>" + amount + "    <color=green>Current health: </color>" + currentHealth + "<color=magenta>\nKnockback: </color> " + knockback + "    <color=cyan>Hitstun: </color>" + hitstunTime);

        InHitstun();
        

        Instantiate(hitParticles, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        anim.SetTrigger("damage");
        
    }

    private void InHitstun() {
        inHitstun = true;
    }

    #region Particles

    public ParticleSystem hitstunDust;

    private void CreateDustTrail() {
        hitstunDust.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        
        var main = hitstunDust.main;
        main.duration = (hitstunTime * 0.01667f);

        hitstunDust.Play();
    }

    #endregion

    #region Knockback and Trajectory Calculations

    private void CalculateKnockBack() {
        float missingHealthPercent = maxHealth - (currentHealth / maxHealth) * 100;
        hitlag = (int)(damageTaken * 0.333f) + 4;
        hitCooldownTime = hitlag + 4;

        if (player.Core.Movement.FacingDirection == 1) {
            kbAngle.x = Mathf.Abs(kbAngle.x);
            kbAngleDeg = Mathf.Abs(kbAngleDeg);
        }
        else {
            kbAngle.x *= -1;
            kbAngleDeg = Mathf.Abs(kbAngleDeg - 180);
        }
        
        knockback = (((((missingHealthPercent * 0.1f + (missingHealthPercent * damageTaken * 0.05f)) * (200 / (weight + 100)) * 1.4f) + 18) * kbScaling * 0.01f) + kbBase);
        hitstunTime = (int)(knockback * 0.4f);

        //DrawTrajectory(knockback * kbAngle * .33f * .12f, Color.red);
    }

    private void ApplyKnockBack(ref Vector2 velocity) {
        Vector2 kbAngleDI = GetKBAngleWithDI();

        DrawTrajectory(knockback * kbAngleDI * .33f * .12f, Color.green);

        velocity = knockback * kbAngleDI * .33f;
    }

    private Vector2 GetKBAngleWithDI() {
        float maxDI = 10;   //max +- change in orginal kbAngle
        float kbAngleRad = (kbAngleDeg + Random.Range(-maxDI, maxDI)) * Mathf.Deg2Rad;

        return new Vector2(Mathf.Cos(kbAngleRad), Mathf.Sin(kbAngleRad));
    }

    private void DrawTrajectory(Vector2 velocity, Color color) {

        float flightDuration = -1 + (2 * velocity.y) / -1.5f; //1.5f is gravity
        float stepTime = flightDuration / 25; //25 line segments

        linePoints.Clear();
        linePoints.Add(hitlagPosition);

        for (int i = 1; i < 25; i++)
        {
            float stepTimePassed = stepTime * i;

            Vector2 movementVector = new Vector2(
                velocity.x * stepTimePassed,
                velocity.y * stepTimePassed - (0.5f * -1.5f * stepTimePassed * stepTimePassed)
            );

            if (velocity.x > 0) {   //airdrift
                movementVector.x = velocity.x * stepTimePassed - (.5f * -0.3f * stepTimePassed * stepTimePassed);
            }
            else {
                movementVector.x = velocity.x * stepTimePassed + (.5f * -0.3f * stepTimePassed * stepTimePassed);
            }

            Vector3 newPointOnLine = -movementVector + hitlagPosition;

            RaycastHit2D hit = Physics2D.Raycast(linePoints[i-1], newPointOnLine - linePoints[i-1], (newPointOnLine - linePoints[i-1]).magnitude);

            if (i > 4 && hit) {
                linePoints.Add(hit.point);
                break;
            }

            linePoints.Add(newPointOnLine);
        }

        lineRenderer.endColor = color;
        lineRenderer.positionCount = linePoints.Count;
        lineRenderer.SetPositions(linePoints.ToArray());
    }

    #endregion

    private void reduceByTraction(ref float velocityX, bool applyDouble) {
        if (velocityX > 0) {
            if (applyDouble && velocityX > 5) {
                velocityX -= 1 * 2;
            }
            else {
                velocityX -= 10.1f;
            }
            if (velocityX < 0) {
                velocityX = 0;
            }
        } else {
            if (applyDouble && velocityX < 5) {
                velocityX += 1 * 2;
            }
            else {
                velocityX += 10.1f;
            }
            if (velocityX > 0) {
                velocityX = 0;
            }
        }
    }
}
