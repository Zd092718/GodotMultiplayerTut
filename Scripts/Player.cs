using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [ExportGroup("References")]
    [Export] public AnimatedSprite2D animSprite;
    [Export] public PackedScene playerCamera;
    [Export] public Vector2 cameraTransform;
    
    [ExportGroup("Movement")]
    [Export] public float moveSpeed = 300;
    [Export] public float gravity = 30;
    [Export] public float jumpStrength = 600;
    [Export] public int maxJumps = 1;

    public Vector2 initialSpriteScale;
    private int jumpCount = 0;
    private Camera2D cameraInstance;
    private Vector2 newCamGP;
    private Vector2 newVel;
    
    public override void _Ready()
    {
        initialSpriteScale = animSprite.Scale;
        animSprite.AnimationFinished += HandleAnimationFinished;
        SetupCamera();
    }

    public override void _Process(double delta)
    {
        UpdateCameraPos();
    }

    public override void _PhysicsProcess(double delta)
    {
        newVel = Velocity;
        float horizontalInput = (
            Input.GetActionStrength(GameConstants.INPUT_MOVE_RIGHT) 
            - Input.GetActionStrength(GameConstants.INPUT_MOVE_LEFT)
            );
        
        newVel.X = horizontalInput * moveSpeed;
        newVel.Y += gravity;

        HandleMovementState();

        Velocity = newVel;

        
        MoveAndSlide();


        FaceMovementDirection(horizontalInput);
        
    }
    private void HandleMovementState()
    {

        bool isFalling = Velocity.Y > 0.0 && !IsOnFloor();
        bool isJumping = Input.IsActionJustPressed(GameConstants.INPUT_JUMP) && IsOnFloor();
        bool isDoubleJumping = Input.IsActionJustPressed(GameConstants.INPUT_JUMP) && isFalling;
        bool isJumpCancelled = Input.IsActionJustReleased(GameConstants.INPUT_JUMP) && Velocity.Y < 0;
        bool isIdle = IsOnFloor() && Mathf.IsZeroApprox(Velocity.X);
        bool isWalking = IsOnFloor() && !Mathf.IsZeroApprox(Velocity.X);
        
        if (isJumping)
        {
            animSprite.Play(GameConstants.ANIM_JUMP_START);
        } else if (isDoubleJumping)
        {
            animSprite.Play(GameConstants.ANIM_DOUBLE_JUMP_START);
        } else if (isWalking)
        {
            animSprite.Play(GameConstants.ANIM_WALK);
        } else if (isFalling)
        {
            animSprite.Play(GameConstants.ANIM_FALL);
        } else if (isIdle)
        {
            animSprite.Play(GameConstants.ANIM_IDLE);
        }

        
        if (isJumping)
        {
            jumpCount++;
            newVel.Y = -jumpStrength;
        } else if (isDoubleJumping)
        {
            jumpCount++;
            if (jumpCount <= maxJumps)
            {
                newVel.Y = -jumpStrength;
            }
        }
        else if(isJumpCancelled)
        {
            newVel.Y = 0;
        } else if (IsOnFloor())
        {
            jumpCount = 0;
        }
    }
    private void FaceMovementDirection(float horizontalInput)
    {

        if (!Mathf.IsZeroApprox(horizontalInput))
        {
            if (horizontalInput < 0)
            {
                animSprite.Scale = new Vector2(-initialSpriteScale.X, initialSpriteScale.Y);
            }
            else
            {
                animSprite.Scale = initialSpriteScale;
            }
        }
    }

    private void UpdateCameraPos()
    {

        newCamGP = cameraInstance.GlobalPosition;
        newCamGP.X = GlobalPosition.X;
        cameraInstance.GlobalPosition = newCamGP;
    }

    private void SetupCamera()
    {
        cameraInstance = playerCamera.Instantiate<Camera2D>();
        cameraInstance.GlobalPosition = cameraTransform;
        GetTree().CurrentScene.CallDeferred("add_child", cameraInstance);
    }
    
    private void HandleAnimationFinished()
    {
        animSprite.Play(GameConstants.INPUT_JUMP);
    }
}
