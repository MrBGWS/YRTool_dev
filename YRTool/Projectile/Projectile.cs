using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YRTool
{
	public class Projectile : PoolableObject
	{
		public enum MovementVectors { Forward, Right, Up }

		[Header("Movement")]
		/// 如果为true，则炮弹将在初始化时朝其旋转方向旋转
		[Tooltip("if true, the projectile will rotate at initialization towards its rotation")]
		public bool FaceDirection = true;
		/// 如果为真，炮弹将朝着运动方向旋转
		[Tooltip("if true, the projectile will rotate towards movement")]
		public bool FaceMovement = false;
		/// 如果FaceMovement为true，则下面指定的射弹矢量将与移动矢量对齐，通常您希望使用3D中的“向前”和2D中的“向右”
		[Tooltip("if FaceMovement is true, the projectile's vector specified below will be aligned to the movement vector, usually you'll want to go with Forward in 3D, Right in 2D")]
		public MovementVectors MovementVector = MovementVectors.Forward;

		/// 对象的速度（相对于级别的速度）
		[Tooltip("the speed of the object (relative to the level's speed)")]
		public float Speed = 0;
		/// 物体随时间的加速度。启用时开始加速。
		[Tooltip("the acceleration of the object over time. Starts accelerating on enable.")]
		public float Acceleration = 0;
		/// 对象的当前方向
		[Tooltip("the current direction of the object")]
		public Vector3 Direction = Vector3.left;
		/// 如果设置为true，则spawner可以更改对象的方向。如果falese，将使用其设置在inspector中的值。
		[Tooltip("if set to true, the spawner can change the direction of the object. If not the one set in its inspector will be used.")]
		public bool DirectionCanBeChangedBySpawner = true;
		/// 投射物镜像时应用的翻转因子
		[Tooltip("the flip factor to apply if and when the projectile is mirrored")]
		public Vector3 FlipValue = new Vector3(-1, 1, 1);
		/// 如果投射物的模型（或精灵）朝右，则将其设置为true，否则设置为false
		[Tooltip("set this to true if your projectile's model (or sprite) is facing right, false otherwise")]
		public bool ProjectileIsFacingRight = true;

		[Header("Spawn")]
		//[MMInformation("Here you can define an initial delay (in seconds) during which this object won't take or cause damage. This delay starts when the object gets enabled. You can also define whether the projectiles should damage their owner (think rockets and the likes) or not", MoreMountains.Tools.MMInformationAttribute.InformationType.Info, false)]
		/// 投射物不能被摧毁的初始延迟
		[Tooltip("the initial delay during which the projectile can't be destroyed")]
		public float InitialInvulnerabilityDuration = 0f;
		/// 投射物伤害它的主人吗？
		[Tooltip("should the projectile damage its owner?")]
		public bool DamageOwner = false;

		/// 返回触摸区域的相关伤害
		public DamageOnTouch TargetDamageOnTouch { get { return _damageOnTouch; } }
		public Weapon SourceWeapon { get { return _weapon; } }

		protected Weapon _weapon;
		protected GameObject _owner;
		protected Vector3 _movement;
		protected float _initialSpeed;
		protected SpriteRenderer _spriteRenderer;
		protected DamageOnTouch _damageOnTouch;
		protected WaitForSeconds _initialInvulnerabilityDurationWFS;
		protected Collider _collider;
		protected Collider2D _collider2D;
		protected Rigidbody _rigidBody;
		protected Rigidbody2D _rigidBody2D;
		protected bool _facingRightInitially;
		protected bool _initialFlipX;
		protected Vector3 _initialLocalScale;
		protected bool _shouldMove = true;
		protected Health _health;
		protected bool _spawnerIsFacingRight;

		/// <summary>
		/// 唤醒时，我们存储对象的初始速度
		/// </summary>
		protected virtual void Awake()
		{
			_facingRightInitially = ProjectileIsFacingRight;
			_initialSpeed = Speed;
			_health = GetComponent<Health>();
			_collider = GetComponent<Collider>();
			_collider2D = GetComponent<Collider2D>();
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_damageOnTouch = GetComponent<DamageOnTouch>();
			_rigidBody = GetComponent<Rigidbody>();
			_rigidBody2D = GetComponent<Rigidbody2D>();
			_initialInvulnerabilityDurationWFS = new WaitForSeconds(InitialInvulnerabilityDuration);
			if (_spriteRenderer != null) { _initialFlipX = _spriteRenderer.flipX; }
			_initialLocalScale = transform.localScale;
		}

		/// <summary>
		/// 处理炮弹初始无敌时间
		/// </summary>
		/// <returns>The invulnerability.</returns>
		protected virtual IEnumerator InitialInvulnerability()
		{
			if (_damageOnTouch == null) { yield break; }
			if (_weapon == null) { yield break; }

			_damageOnTouch.ClearIgnoreList();
			_damageOnTouch.IgnoreGameObject(_weapon.Owner.gameObject);
			yield return _initialInvulnerabilityDurationWFS;
			if (DamageOwner)
			{
				_damageOnTouch.StopIgnoringObject(_weapon.Owner.gameObject);
			}
		}

		/// <summary>
		/// 生成投射物
		/// </summary>
		protected virtual void Initialization()
		{
			Speed = _initialSpeed;
			ProjectileIsFacingRight = _facingRightInitially;
			if (_spriteRenderer != null) { _spriteRenderer.flipX = _initialFlipX; }
			transform.localScale = _initialLocalScale;
			_shouldMove = true;
			_damageOnTouch?.InitializeFeedbacks();

			if (_collider != null)
			{
				_collider.enabled = true;
			}
			if (_collider2D != null)
			{
				_collider2D.enabled = true;
			}
		}

		/// <summary>
		/// 在update中，我们根据级别的速度和对象的速度移动对象，并应用加速度
		/// </summary>
		protected virtual void FixedUpdate()
		{
			base.Update();
			if (_shouldMove)
			{
				Movement();
			}
		}

		/// <summary>
		/// 处理炮弹每一帧的运动
		/// </summary>
		public virtual void Movement()
		{
			_movement = Direction * (Speed / 10) * Time.deltaTime;
			//transform.Translate(_movement,Space.World);
			if (_rigidBody != null)
			{
				_rigidBody.MovePosition(this.transform.position + _movement);
			}
			if (_rigidBody2D != null)
			{
				_rigidBody2D.MovePosition(this.transform.position + _movement);
			}
			// We apply the acceleration to increase the speed
			Speed += Acceleration * Time.deltaTime;
		}

		/// <summary>
		/// 设置投射物的方向。
		/// </summary>
		/// <param name="newDirection">New direction.</param>
		/// <param name="newRotation">New rotation.</param>
		/// <param name="spawnerIsFacingRight">If set to <c>true</c> spawner is facing right.</param>
		public virtual void SetDirection(Vector3 newDirection, Quaternion newRotation, bool spawnerIsFacingRight = true)
		{
			_spawnerIsFacingRight = spawnerIsFacingRight;

			if (DirectionCanBeChangedBySpawner)
			{
				Direction = newDirection;
			}
			if (ProjectileIsFacingRight != spawnerIsFacingRight)
			{
				Flip();
			}
			if (FaceDirection)
			{
				transform.rotation = newRotation;
			}

			if (_damageOnTouch != null)
			{
				_damageOnTouch.SetKnockbackScriptDirection(newDirection);
			}

			if (FaceMovement)
			{
				switch (MovementVector)
				{
					case MovementVectors.Forward:
						transform.forward = newDirection;
						break;
					case MovementVectors.Right:
						transform.right = newDirection;
						break;
					case MovementVectors.Up:
						transform.up = newDirection;
						break;
				}
			}
		}

		/// <summary>
		/// 翻转抛射物
		/// </summary>
		protected virtual void Flip()
		{
			if (_spriteRenderer != null)
			{
				_spriteRenderer.flipX = !_spriteRenderer.flipX;
			}
			else
			{
				this.transform.localScale = Vector3.Scale(this.transform.localScale, FlipValue);
			}
		}

		/// <summary>
		/// 翻转抛射物
		/// </summary>
		protected virtual void Flip(bool state)
		{
			if (_spriteRenderer != null)
			{
				_spriteRenderer.flipX = state;
			}
			else
			{
				this.transform.localScale = Vector3.Scale(this.transform.localScale, FlipValue);
			}
		}

		/// <summary>
		/// 设置投射物的父武器。
		/// </summary>
		/// <param name="newWeapon">New weapon.</param>
		public virtual void SetWeapon(Weapon newWeapon)
		{
			_weapon = newWeapon;
		}

		/// <summary>
		/// 将投射物的DamageOnTouch造成的伤害设置为指定值
		/// </summary>
		/// <param name="newDamage"></param>
		public virtual void SetDamage(float minDamage, float maxDamage)
		{
			if (_damageOnTouch != null)
			{
				_damageOnTouch.MinDamageCaused = minDamage;
				_damageOnTouch.MaxDamageCaused = maxDamage;
			}
		}

		/// <summary>
		/// 设定炮弹的拥有者。
		/// </summary>
		/// <param name="newOwner">New owner.</param>
		public virtual void SetOwner(GameObject newOwner)
		{
			_owner = newOwner;
			DamageOnTouch damageOnTouch = this.gameObject.GetComponent<DamageOnTouch>();
			if (damageOnTouch != null)
			{
				damageOnTouch.Owner = newOwner;
				damageOnTouch.Owner = newOwner;
				if (!DamageOwner)
				{
					damageOnTouch.ClearIgnoreList();
					damageOnTouch.IgnoreGameObject(newOwner);
				}
			}
		}

		/// <summary>
		/// 返回射弹的当前所有者
		/// </summary>
		/// <returns></returns>
		public virtual GameObject GetOwner()
		{
			return _owner;
		}

		/// <summary>
		/// 销毁时，禁用碰撞器并阻止移动
		/// </summary>
		public virtual void StopAt()
		{
			if (_collider != null)
			{
				_collider.enabled = false;
			}
			if (_collider2D != null)
			{
				_collider2D.enabled = false;
			}

			_shouldMove = false;
		}

		/// <summary>
		/// 销毁时
		/// </summary>
		protected virtual void OnDeath()
		{
			StopAt();
		}

		/// <summary>
		/// 启用时，我们触发短暂的无敌时间
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			Initialization();
			if (InitialInvulnerabilityDuration > 0)
			{
				StartCoroutine(InitialInvulnerability());
			}

			if (_health != null)
			{
				_health.OnDeath += OnDeath;
			}
		}

		/// <summary>
		/// 在禁用时，我们将OnDeath方法插入生命组件
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();
			if (_health != null)
			{
				_health.OnDeath -= OnDeath;
			}
		}
	}
}
