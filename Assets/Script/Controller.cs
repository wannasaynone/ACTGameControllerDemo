using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class Controller : MonoBehaviour {

    [Header("Controller Property")]
    [SerializeField]
    private float MovingUpSpeed = 0f;//加速度
    [SerializeField]
    private float MaxMovingSpeed = 0f;//最大速
    [SerializeField]
    private float JumpForce = 0f;//跳躍力
    [SerializeField]
    private float RunDetectInputTime = 0f;//跑步判斷按鍵間隔
    [SerializeField]
    private float RunSpeedScale = 0f;//跑速倍率
    [SerializeField]
    private float NormalAttackCd = 0f;//普攻攻擊頻率
    [SerializeField]
    private float MovingSpeedWhenGetHit = 0f;//被打時移動多少
    [SerializeField]
    private float RecoverInvincibleTime = 0f;//恢復後無敵幾秒
    [SerializeField]
    private bool isAI = false;//設定這個控制器是AI控制

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    private Rigidbody2D rigid;
    private Animator anim;
    private SpriteRenderer sprite;
    public Guid GUID { private set; get; }
    [SerializeField]
    private TextMesh TestHPText;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isAI) detectInput(); else AIController();
        animPlay();//先處理輸入才處理動畫
        timer();

        if (!isAI)
            TestHPText.text = "PLAYER:" + GameManager.insance.GetCharacterState(GUID).CurrentHP + "/" + GameManager.insance.GetCharacterState(GUID).MaxHP;
        else
            TestHPText.text = "AI:" + GameManager.insance.GetCharacterState(GUID).CurrentHP + "/" + GameManager.insance.GetCharacterState(GUID).MaxHP;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    public void SetGUID(Guid GUID)
    {
        if (this.GUID == Guid.Empty)
            this.GUID = GUID;
        else
            Debug.LogError("Is Trying To Reset GUID on Controller:" + gameObject.name);
    }

    public void SetIsHurt(bool isAttackFromFront)
    {
        if(!isInvincible)
        {
            GameManager.insance.CreateHitEffect(transform.position + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f)));
            if (isAttackFromFront)
            {
                _currentAttackState = attackState.HurtFromFront;
            }
            else
            {
                _currentAttackState = attackState.HurtFromBack;
            }
            StartCoroutine(IEShake());
        }
    }

    public void SetIsHeavyHurt(bool isAttackFromFront)
    {
        if(!isInvincible)
        {
            GameManager.insance.CreateHitEffect(transform.position + new Vector3(UnityEngine.Random.Range(-0.1f,0.1f), UnityEngine.Random.Range(-0.1f, 0.1f)));
            if (isAttackFromFront)
            {
                _currentAttackState = attackState.FlyFromFront;
                rigid.AddForce(new Vector2(-MovingSpeedWhenGetHit, JumpForce / 2));
            }
            else
            {
                _currentAttackState = attackState.FlyFromBack;
                rigid.AddForce(new Vector2(MovingSpeedWhenGetHit, JumpForce / 2));
            }
            StartCoroutine(IEShake());
            downRecoverTimer = 2f;
        }
    }

    public void SetIsDie(bool isAttackFromFront)
    {
        _currentAttackState = attackState.Die;
        if (isAttackFromFront)
        {
            anim.Play(animationClipName.Die_FromFront_DownOnGround.ToString(), 0);
        }
        else
        {
            anim.Play(animationClipName.Die_FromBackDownOnGround.ToString(), 0);
        }
    }

    public bool isFacingRight()
    {
        return transform.eulerAngles.y > 0;
    }

    public bool IsInvincible { get { return isInvincible; } }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    private enum animatorParameterName
    {
        JumpVelocity
    }

    private enum animationClipName
    {
        Idle,
        Move,
        Run,
        RunAndAttack,
        RunAndJumpAndAttack,
        Jump,
        JumpAndAttack,
        NormalAttack_0,
        NormalAttack_1,
        NormalAttack_2,
        NormalAttack_3,
        SkillAttack_0,//還沒做
        SkillAttack_1,//還沒做
        SkillAttack_2,//還沒做
        Hurt_FromFront,
        Hurt_FromBack,
        Fly_FromFront,
        Fly_FromBack,
        DownRecover,
        Die_FromBackDownOnGround,
        Die_FromFront_DownOnGround
    }

    private enum attackState
    {
        None,
        JumpAndAttack,
        RunAndAttack,
        RunAndJumpAndAttack,
        NormalAttack_0,
        NormalAttack_1,
        NormalAttack_2,
        NormalAttack_3,
        SkillAttack_0,//還沒做
        SkillAttack_1,//還沒做
        SkillAttack_2,//還沒做
        HurtFromFront,
        HurtFromBack,
        FlyFromFront,
        FlyFromBack,
        Die
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    //State
    private attackState _currentAttackState = attackState.None;
    private bool isRunning = false;
    private bool isInvincible = false;

    //Timer
    private float detectRunningTimer = 0f;
    private float normalAttackCdTimer = 0f;
    private float downRecoverTimer = 0f;
    private float invincibleTimer = 0f;

    private void animPlay()
    {
        anim.SetFloat(animatorParameterName.JumpVelocity.ToString(), rigid.velocity.y);

        if(_currentAttackState == attackState.None)//先判斷是否要攻擊
        {
            if (rigid.velocity.y > 0.05f || rigid.velocity.y < -0.05f)//跳躍動畫優先，如果有速度值=正在跳or正在掉
            {
                anim.Play(animationClipName.Jump.ToString(), 0);
            }
            else
            if (rigid.velocity.x > 0.05f || rigid.velocity.x < -0.05f)//如果沒有在跳就顯示走路
            {
                if (isRunning)
                    anim.Play(animationClipName.Run.ToString(), 0);
                else
                    anim.Play(animationClipName.Move.ToString(), 0);
            }
            else
            if (rigid.velocity.x < 0.05f && rigid.velocity.x > -0.05f
            && rigid.velocity.y < 0.05f && rigid.velocity.y > -0.05f)//如果速度都極小就判定為靜止
            {
                anim.Play(animationClipName.Idle.ToString(), 0);
            }
        }
        else
        {
            switch (_currentAttackState)
            {
                case attackState.NormalAttack_0:
                    anim.Play(animationClipName.NormalAttack_0.ToString(), 0);
                    break;
                case attackState.NormalAttack_1:
                    anim.Play(animationClipName.NormalAttack_1.ToString(), 0);
                    break;
                case attackState.NormalAttack_2:
                    anim.Play(animationClipName.NormalAttack_2.ToString(), 0);
                    break;
                case attackState.JumpAndAttack:
                    if (isJumping())
                        anim.Play(animationClipName.JumpAndAttack.ToString(), 0);
                    else
                        resetAttackState();//跳攻狀態落地的話重設攻擊狀態
                    break;
                case attackState.RunAndAttack:
                    anim.Play(animationClipName.RunAndAttack.ToString(), 0);
                    break;
                case attackState.RunAndJumpAndAttack:
                    if (isJumping())
                        anim.Play(animationClipName.RunAndJumpAndAttack.ToString(), 0);
                    else
                        resetAttackState();//跳攻狀態落地的話重設攻擊狀態
                    break;
                case attackState.HurtFromFront:
                    anim.Play(animationClipName.Hurt_FromFront.ToString(), 0);
                    break;
                case attackState.HurtFromBack:
                    anim.Play(animationClipName.Hurt_FromBack.ToString(), 0);
                    break;
                case attackState.FlyFromFront:
                    if(downRecoverTimer > 0)
                        anim.Play(animationClipName.Fly_FromFront.ToString(), 0);
                    else
                        anim.Play(animationClipName.DownRecover.ToString(), 0);
                    break;
                case attackState.FlyFromBack:
                    if (downRecoverTimer > 0)
                        anim.Play(animationClipName.Fly_FromBack.ToString(), 0);
                    else
                        anim.Play(animationClipName.DownRecover.ToString(), 0);
                    break;
            }
        }
    }

    private void timer()
    {
        if (detectRunningTimer > 0)
            detectRunningTimer -= Time.deltaTime;

        if (normalAttackCdTimer > 0)
            normalAttackCdTimer -= Time.deltaTime;

        if (aiConditionJudgeTimer > 0)
            aiConditionJudgeTimer -= Time.deltaTime;

        if (downRecoverTimer > 0)
        {
            downRecoverTimer -= Time.deltaTime;
            invincibleTimer = RecoverInvincibleTime;
        }

        if (invincibleTimer > 0)
        {
            isInvincible = true;
            invincibleTimer -= Time.deltaTime;
        }
        else
            isInvincible = false;

    }

    private void resetAttackState()//for animation event
    {
        _currentAttackState = attackState.None;
        if (normalAttackCdTimer <= 0)
            normalAttackCdTimer = NormalAttackCd;
    }

    private void move(bool isGoingRight)
    {
        float _movingUpSpeed = isGoingRight ? MovingUpSpeed : -MovingUpSpeed;
        _movingUpSpeed = _currentAttackState == attackState.None ? _movingUpSpeed : 0;
        _movingUpSpeed = isRunning ? _movingUpSpeed * RunSpeedScale : _movingUpSpeed;
        float _maxMovingSpeed = isRunning ? MaxMovingSpeed * RunSpeedScale : MaxMovingSpeed;

        //翻轉圖片
        if (_movingUpSpeed > 0f)
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        else if (_movingUpSpeed < 0f)
            transform.eulerAngles = new Vector3(0f, 180f, 0f);

        if (isGoingRight && rigid.velocity.x < _maxMovingSpeed || !isGoingRight && rigid.velocity.x > -_maxMovingSpeed)
            rigid.AddForce(new Vector2(_movingUpSpeed, 0f));
    }

    private void stopMove()
    {
        isRunning = false;
        rigid.velocity = new Vector2(rigid.velocity.x / RunSpeedScale, rigid.velocity.y / RunSpeedScale);
    }

    private void jump()
    {
        if (!isJumping() && _currentAttackState == attackState.None)
            rigid.AddForce(new Vector2(0f, JumpForce));
    }

    private bool isJumping()
    {
        return rigid.velocity.y > 0.01f || rigid.velocity.y < -0.01f;
    }

    private void attack()
    {
        if(normalAttackCdTimer <= 0)
        {
            changeAttackState();//先判斷要變更成什麼攻擊狀態
            switch (_currentAttackState)//輸出攻擊的內容(特效之類的)
            {
                case attackState.None://無
                    stopMove();
                    break;
                case attackState.NormalAttack_0://第一段普攻
                    stopMove();
                    break;
                case attackState.NormalAttack_1://第二段普攻
                    stopMove();
                    break;
                case attackState.NormalAttack_2://第三段普攻
                    stopMove();
                    break;
                case attackState.JumpAndAttack://跳攻
                    stopMove();
                    break;
                case attackState.RunAndJumpAndAttack://跑&跳&攻擊
                    break;
                case attackState.RunAndAttack://跑攻
                    break;
            }
        }
    }

    private void changeAttackState()
    {
        switch (_currentAttackState)
        {
            case attackState.None://無
                if (isJumping() && isRunning)
                    _currentAttackState = attackState.RunAndJumpAndAttack;
                else
                if (isJumping())
                    _currentAttackState = attackState.JumpAndAttack;
                else
                if (isRunning)
                    _currentAttackState = attackState.RunAndAttack;
                else
                    _currentAttackState = attackState.NormalAttack_0;
                break;
            case attackState.NormalAttack_0://第一段普攻
                _currentAttackState = attackState.NormalAttack_1;
                break;
            case attackState.NormalAttack_1://第二段普攻
                _currentAttackState = attackState.NormalAttack_2;
                break;
            case attackState.NormalAttack_2://第三段普攻
                break;
            case attackState.JumpAndAttack://跳攻
                break;
            case attackState.RunAndJumpAndAttack://跑&跳&攻擊
                break;
            case attackState.RunAndAttack://跑攻
                break;
        }
    }

    IEnumerator IEShake()
    {
        transform.localPosition += new Vector3(0.05f, 0);
        yield return new WaitForSeconds(0.01f);
        transform.localPosition -= new Vector3(0.05f, 0);
        yield return new WaitForSeconds(0.01f);
        transform.localPosition -= new Vector3(0.05f, 0);
        yield return new WaitForSeconds(0.01f);
        transform.localPosition += new Vector3(0.05f, 0);
    }

    private void showInvicible()//for animation event
    {
        StartCoroutine(IEIInvincible());
    }

    bool enteredIEIInvincible = false;
    IEnumerator IEIInvincible()
    {
        if(!enteredIEIInvincible)
        {
            enteredIEIInvincible = true;
            while (isInvincible)
            {
                if (sprite.enabled)
                    sprite.enabled = false;
                else
                    sprite.enabled = true;
                yield return new WaitForSeconds(0.1f);
            }
            if (!sprite.enabled)
                sprite.enabled = true;
            enteredIEIInvincible = false;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    private void detectInput()
    {
        if (InputDetecter.GetPlayerControllerInput(KeyCode.RightArrow, InputType.Down)
            || InputDetecter.GetPlayerControllerInput(KeyCode.LeftArrow, InputType.Down))
        {
            if (detectRunningTimer <= 0f)
                detectRunningTimer = RunDetectInputTime;
            else
                isRunning = true;
        }

        if (InputDetecter.GetPlayerControllerInput(KeyCode.RightArrow, InputType.Hold))
            move(true);

        if (InputDetecter.GetPlayerControllerInput(KeyCode.LeftArrow, InputType.Hold))
            move(false);

        if (InputDetecter.GetPlayerControllerInput(KeyCode.RightArrow, InputType.Up)
            || InputDetecter.GetPlayerControllerInput(KeyCode.LeftArrow, InputType.Up))
            stopMove();

        if (InputDetecter.GetPlayerControllerInput(KeyCode.Space, InputType.Down))
            jump();

        if (InputDetecter.GetPlayerControllerInput(KeyCode.Z, InputType.Down))
            attack();
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    private enum aiMotionOptions
    {
        Idle,
        Move_Right,
        Move_Left,
        Run_Right,
        Run_Left,
        RunAndAttack,
        RunAndJumpAndAttack,
        Jump,
        JumpAndAttack,
        Attack,
        SkillAttack_0,//還沒做
        SkillAttack_1,//還沒做
        SkillAttack_2//還沒做
    }

    //AI State
    private aiMotionOptions _currnetAIState = aiMotionOptions.Idle;
    private bool isAttacking = false;

    //AI Timer
    private float aiConditionJudgeTimer = 1f;

    private void AIController()//TEST中
    {
        AICondition();//切換AI狀態
        AIDo();//切換完AI狀態後執行
    }

    private CharacterState playerState = new CharacterState();
    private void AICondition()
    {
        if(aiConditionJudgeTimer <= 0)
        {
            playerState = GameManager.insance.GetPlayer();
            aiConditionJudgeTimer = 0.5f;
            if (playerState.CurrentHP < 0)
                _currnetAIState = aiMotionOptions.Idle;
            else
            if (playerState.isInvincible)
            {
                if(playerState.GameObject.transform.position.x > transform.position.x)
                    _currnetAIState = aiMotionOptions.Move_Left;
                else
                if (playerState.GameObject.transform.position.x < transform.position.x)
                    _currnetAIState = aiMotionOptions.Move_Right;
            }
            else
            if (playerState.GameObject.transform.position.x > transform.position.x && Mathf.Abs(playerState.GameObject.transform.position.x - transform.position.x) > 2f)
                _currnetAIState = aiMotionOptions.Run_Right;
            else
            if (playerState.GameObject.transform.position.x < transform.position.x && Mathf.Abs(playerState.GameObject.transform.position.x - transform.position.x) > 2f)
                _currnetAIState = aiMotionOptions.Run_Left;
            else
            if (playerState.GameObject.transform.position.x > transform.position.x && Mathf.Abs(playerState.GameObject.transform.position.x - transform.position.x) > 0.5f)
                _currnetAIState = aiMotionOptions.Move_Right;
            else
            if (playerState.GameObject.transform.position.x < transform.position.x && Mathf.Abs(playerState.GameObject.transform.position.x - transform.position.x) > 0.5f)
                _currnetAIState = aiMotionOptions.Move_Left;
            else
                _currnetAIState = aiMotionOptions.Attack;
        }
    }

    private void AIDo()
    {
        switch(_currnetAIState)
        {
            case aiMotionOptions.Idle:
                break;
            case aiMotionOptions.Move_Right:
                isRunning = false;
                move(true);
                break;
            case aiMotionOptions.Move_Left:
                isRunning = false;
                move(false);
                break;
            case aiMotionOptions.Run_Right:
                isRunning = true;
                move(true);
                break;
            case aiMotionOptions.Run_Left:
                isRunning = true;
                move(false);
                break;
            case aiMotionOptions.Attack:
                StartCoroutine(IEAINormalAttack());
                break;
        }
    }

    IEnumerator IEAINormalAttack()
    {
        if(!isAttacking)
        {
            isAttacking = true;
            attack();
            yield return new WaitForSeconds(0.2f);
            attack();
            yield return new WaitForSeconds(0.2f);
            attack();
            isAttacking = false;
        }

    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////


}
