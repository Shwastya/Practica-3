using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerBehaviour : MonoBehaviour
{
	// Definir Hashes de:
	// Parametros (Speed, Attack, Damage, Dead)
	// Estados (Base Layer.Idle, Attack Layer.Idle, Attack Layer.Attack)
	// TODO
	static int speedHash = Animator.StringToHash("Speed");
	static int attackHash = Animator.StringToHash("Attack");
	static int damageHash = Animator.StringToHash("Damage");
	static int deadHash = Animator.StringToHash("Dead");

	static int baseIdleState = Animator.StringToHash("Base Layer.Idle");
	static int attackIdleState = Animator.StringToHash("Attack Layer.Idle");
	static int attackState = Animator.StringToHash("Attack Layer.Attack");
	
	[SerializeField] private float walkSpeed	= 1f;       // Parametro que define la velocidad de "caminar"
	[SerializeField] private float runSpeed		= 2f;       // Parametro que define la velocidad de "correr"
	[SerializeField] private float rotateSpeed	= 160;      // Parametro que define la velocidad de "girar"	

	// Indica si el player esta pausado (congelado). Que no responde al Input
	public bool paused = false;

	[SerializeField] private AudioClip FxAttack = null;
	[SerializeField] private AudioClip FxDamage = null;
	[SerializeField] private AudioClip FxDead   = null;

	private AudioSource _sourceAttack = null;
	private AudioSource _sourceDamage = null;
	private AudioSource _sourceDeath  = null;


	// Variables auxiliares
	private float _angularSpeed			= 0;		// Velocidad de giro actual
	private float _speed				= 0;		// Velocidad de traslacion actual
	private float _originalColliderZ	= 0;		// Valor original de la posición 'z' del collider

	// Variables internas:
	private int _lives = 3;							// Vidas restantes
	private float _h = 0, _v = 0;					// Axis del Input (en settings)
	
	private Animator _an = null;
	private AnimatorStateInfo _infoState = default;
	private BoxCollider _box = null;
	private Rigidbody _rb = null;

	private bool run = false;

	private float _attackRate = 0.75f;       // Periodo de ataque
	private float _nextAttackTime = 0;

	
	void Start()
	{
		// Obtener los componentes Animator, Rigidbody y el valor original center.z del BoxCollider
		// TODO		
		_rb = GetComponent<Rigidbody>();
		_box = GetComponent<BoxCollider>();
		_originalColliderZ = _box.center.z;
		_an = GetComponent<Animator>();

		_sourceAttack = gameObject.AddComponent<AudioSource>();
		_sourceDamage = gameObject.AddComponent<AudioSource>();
		_sourceDeath  = gameObject.AddComponent<AudioSource>();

	}
	
	// Aqui moveremos y giraremos la araña en funcion del Input
	private void FixedUpdate()
	{
		// Si estoy en pausa no hacer nada (no moverme ni atacar)
		if (paused) return;

		// Calculo de velocidad lineal (_speed) y angular (_angularSpeed) en función del Input
		// Si camino/corro hacia delante delante: _speed = walkSpeed / _speed = runSpeed	 
		// Si no me muevo: _speed = 0
		// TODO		
		if (Input.GetKey(KeyCode.LeftShift) || (UIButton.GetInput(ButtonType.RUN))) run = true;
		else run = false;
	
		_speed = (run) ? _speed = _v * 2f : _speed = _v * 1f;
		if (_v < 0) _speed = 1f;

		// Si giro izquierda: _angularSpeed = -rotateSpeed;
		// Si giro derecha: _angularSpeed = rotateSpeed;
		// Si no giro : _angularSpeed = 0;
		// TODO
		_angularSpeed = Mathf.RoundToInt(_h) * rotateSpeed;

		// Actualizamos el parámetro "Speed" en función de _speed. Para activar la animación de caminar/correr
		// TODO
		_an.SetFloat(speedHash, _speed);

		// Movemos y rotamos el rigidbody (MovePosition y MoveRotation) en función de "_speed" y "_angularSpeed"
		// TODO				
		_rb.MovePosition(transform.position + transform.forward * (run ? _v * ((_v < 0)
						? walkSpeed : runSpeed) : _v * walkSpeed) * Time.fixedDeltaTime);
		_rb.MoveRotation(transform.rotation * Quaternion.Euler(new Vector3(0, _angularSpeed, 0) * Time.fixedDeltaTime));

		// Mover el collider en función del parámetro "Distance" (necesario cuando atacamos)
		// TODO
		_box.center = new Vector3(_box.center.x, _box.center.y, _originalColliderZ + (_an.GetFloat("Distance") * 6));
	}

	// En este bucle solamente comprobaremos si el Input nos indica "atacar" y activaremos el trigger "Attack"
	private void Update()
	{
		// Si estoy en pausa no hacer nada (no moverme ni atacar)
		// TODO
		if (paused) return;

		// movimiento por teclado
		_h = Input.GetAxis("Horizontal");
		_v = Input.GetAxis("Vertical");	

		// movimiento con crossbutton (a lo cutre)
		if (CrossButton.GetInput(InputType.UP) ||
			(UIButton.GetInput(ButtonType.RUN)))   _v =  1;	// AUTORUN push button	
		if (CrossButton.GetInput(InputType.DOWN))  _v = -1;
		if (CrossButton.GetInput(InputType.RIGHT)) _h =  1; 
		if (CrossButton.GetInput(InputType.LEFT))  _h = -1;

		if (CrossButton.GetInput(InputType.UP_LEFT))    { _v =  1; _h = -1; }
		if (CrossButton.GetInput(InputType.UP_RIGHT))   { _v =  1; _h =  1; }
		if (CrossButton.GetInput(InputType.DOWN_LEFT))  { _v = -1; _h = -1; }
		if (CrossButton.GetInput(InputType.DOWN_RIGHT)) { _v = -1; _h =  1; }

		// Si detecto Input tecla/boton ataque ==> Activo disparados 'Attack'
		// UIbutton
		if (Input.GetKeyDown(KeyCode.Space) || UIButton.GetInput(ButtonType.ATTACK))
		{
			if (Time.time >= _nextAttackTime)
			{
				if (_an.GetCurrentAnimatorStateInfo(1).fullPathHash != damageHash)
				{
					_an.SetTrigger(attackHash);
					_nextAttackTime = Time.time + 1f / _attackRate;
					
				}				
			}
		}
		if (_an.GetFloat("Distance") > 0.02) play(_sourceAttack, FxAttack);
	}
	private void play(AudioSource source, AudioClip clip) 
	{
		if (!source.isPlaying && GameManager.instance.soundEnabled)
		{
			source.clip = clip;
			source.Play();
			source.volume = GameManager.instance.volumen;
		}		
	}
	private void playDelay(AudioSource source, AudioClip clip, float delay)
	{		
		if (!source.isPlaying && GameManager.instance.soundEnabled)
		{
			source.clip = clip;
			source.PlayDelayed(delay);
			source.volume = GameManager.instance.volumen;
		}
	}
	// Función para resetear el Player
	public void reset()
	{
		//Reiniciar el numero de vidas
		// TODO
		_lives = 3;
		UIManager.instance.resetHearts();
		// Pausamos a Player
		// TODO
		paused = true;

		// Forzar estado Idle en las dos capas (Base Layer y Attack Layer): función Play() de Animator
		// TODO
		if (_an != null)
		{
			// Reseteo todos los triggers (Attack y Dead)
			// TODO	
			resetAnimation();			
		}
		// Posicionar el jugador en el (0,0,0) y rotación nula (Quaternion.identity)
		// TODO
		if (_rb != null)
		{
			_rb.MovePosition(new Vector3(0, 0, 0));
			_rb.MoveRotation(Quaternion.identity);
		}		
	}
	public void resetAnimation()
	{
		_an.ResetTrigger("Attack");
		_an.ResetTrigger("Dead");
		if (_an.GetCurrentAnimatorStateInfo(0).fullPathHash != baseIdleState)
			_an.Play("Base Layer.Idle");
		if (_an.GetCurrentAnimatorStateInfo(1).fullPathHash != attackIdleState)
			_an.Play("Attack Layer.Idle");
	}
	public void speedBrake() 
	{
		_an.SetFloat("Speed", 0);
	}
	public int getLives()
	{
		return _lives;
	}
	// Funcion recibir daño
	public void recieveDamage()
	{
		// Restar una vida
		_lives--;
		UIManager.instance.updateHearts(_lives);
		
		// Si no me quedan vidas notificar al GameManager (notifyPlayerDead) y disparar trigger "Dead"
		// TODO
		if (_lives < 1)
		{
			play(_sourceDeath, FxDead);
			GameManager.instance.notifyPlayerDead();
			_an.SetTrigger(deadHash);
		}
		// Si aun me quedan vidas dispara el trigger TakeDamage
		// TODO
		else 
		{
			_an.SetTrigger(damageHash);
			play(_sourceDamage, FxDamage);
		}		
	}	
	private void OnCollisionEnter(Collision collision)
	{
		// Obtener estado actual de la capa Attack Layer
		// TODO
		_infoState = _an.GetCurrentAnimatorStateInfo(1);

		// Si el estado es 'Attack' matamos al enemigo (mirar etiqueta)
		// TODO
		if (_infoState.fullPathHash == attackState)
		{			
			if (collision.collider.gameObject.tag == "Enemy")
			{
				if (Physics.Raycast(_rb.transform.position, _rb.transform.TransformDirection(Vector3.forward), 10))
					collision.collider.GetComponent<SkeletonBehaviour>().kill();
			}			
		}
	}
}
