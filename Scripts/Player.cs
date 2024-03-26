using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Export] public AnimatedSprite2D animSprite;
    
    [Export] public float moveSpeed = 300;
    [Export] public float gravity = 30;
    [Export] public float jumpStrength = 600;

    public Vector2 initialSpriteScale;
    public override void _Ready()
    {
        initialSpriteScale = animSprite.Scale;
    }
    public override void _PhysicsProcess(double delta)
    {
        Vector2 newVel = Velocity;
        float horizontalInput = (
            Input.GetActionStrength(GameConstants.INPUT_MOVE_RIGHT) 
            - Input.GetActionStrength(GameConstants.INPUT_MOVE_LEFT)
            );
        
        newVel.X = horizontalInput * moveSpeed;
        newVel.Y += gravity;

        bool isFalling = Velocity.Y > 0.0 && !IsOnFloor();
        bool isJumping = Input.IsActionJustPressed(GameConstants.INPUT_JUMP) && IsOnFloor();
        bool isJumpCancelled = Input.IsActionJustReleased(GameConstants.INPUT_JUMP) && Velocity.Y < 0;
        bool isIdle = IsOnFloor() && Mathf.IsZeroApprox(Velocity.X);
        bool isWalking = IsOnFloor() && !Mathf.IsZeroApprox(Velocity.X); 
        
        if (isJumping)
        {
            newVel.Y = -jumpStrength;
        }
        
        Velocity = newVel;

        
        MoveAndSlide();

        if (isJumping)
        {
            animSprite.Play(GameConstants.ANIM_JUMP_START);
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
}
