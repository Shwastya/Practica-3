using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBehaviour : MonoBehaviour
{
	// Definir Hashes de:
	// Parametros (Attack, Dead, Distance)
	// Estados (Attack, Idle)
	// TODO
	static int deadHash = Animator.StringToHash("Dead");
	static int chaseHash = Animator.StringToHash("Chase");
	static int reachHash = Animator.StringToHash("Reach");
	static int attackHash = Animator.StringToHash("Attack");

	static int idleState = Animator.StringToHash("Base Layer.Idle");
	static int attackState = Animator.StringToHash("Base Layer.Attack");

	[SerializeField] private AudioClip FxAttack = null;
	[SerializeField] private AudioClip FxDead = null;

	private AudioSource _sourceAttack = null;
	private AudioSource _sourceDeath = null;

	// Variables auxiliares 
	private PlayerBehaviour _player		= null;     //Puntero a Player (establecido por método 'setPlayer')
	private Vector3 originalPosition	= default;	// como el enemigo se mueve, hay que resetearlo
	private bool _dead					= false;	// Indica si ya he sido eliminado
	private float _originalColliderZ	= 0;        // Valora original de la posición 'z' del collider	

	private float _distanceAlert = 600f;
	private float _distanceAttack = 50f;
	private float _walkSpeed = 0.3f;
	
	private float _attackRate = 0.3f;       // Periodo de ataque
	private float _nextAttackTime = 0;


	private bool _alerted = false;

	// Variables privades
	private Animator _an = null;
	private BoxCollider _box;
	private Rigidbody _rb = null;
	
	public void setPlayer(PlayerBehaviour player)
	{
		_player = player;
	}
	void Start ()
	{
		// Obtener los componentes Animator y el valor original center.z del BoxCollider
		// TODO
		_rb = GetComponent<Rigidbody>();
		_box = GetComponent<BoxCollider>();
		_originalColliderZ = _box.center.z;
		_an = GetComponent<Animator>();

		setPlayer(GameManager.instance.player);
		originalPosition = transform.position;

		_sourceAttack = gameObject.AddComponent<AudioSource>();
		_sourceDeath = gameObject.AddComponent<AudioSource>();
	}
	void FixedUpdate ()
	{
		// Si estoy muerto ==> No hago nada
		// TODO
		if (_player.paused &&  !_dead) enemySetToIdle(); 

		if (_dead || _player.paused ) return;

		// Si Player esta a menos de 1m de mi y no estoy muerto:
		// - Le miro
		// - Si ha pasado 1s o más desde el ultimo ataque ==> attack()
		// TODO
		float dist = (_player.transform.position - transform.position).sqrMagnitude;

		if (!_alerted && dist < _distanceAlert) 
		{			
			_nextAttackTime = Time.time + 1f / _attackRate;
			_alerted = true;
		} 
		else if (_alerted && !UIManager.instance.mainMenu.activeSelf && !_dead)
		{
			transform.LookAt(_player.transform.position);

			if (dist <= _distanceAttack)
			{
				_an.SetTrigger(reachHash);
				_an.ResetTrigger(chaseHash);

				if (Time.time >= _nextAttackTime) attack();
			}
			else if (dist > _distanceAttack - 2.0f) chase();
		}	
		// Desplazar el collider en 'z' un multiplo del parametro Distance
		// TODO
		_box.center = new Vector3(_box.center.x, _box.center.y, _originalColliderZ + (_an.GetFloat("Distance") * 0.3f));
	}	
	public void chase()
	{
		if (_an.GetCurrentAnimatorStateInfo(0).fullPathHash != attackState)
		{
			_an.ResetTrigger(reachHash);
			_an.SetTrigger(chaseHash);
			Vector3 dir = _player.transform.position - transform.position;
			_rb.MovePosition(transform.position + (dir * _walkSpeed * Time.fixedDeltaTime));
		}		
	}
	public void attack()
	{
		// Activo el trigger "Attack"
		// TODO	
		_nextAttackTime = Time.time + 1f / _attackRate;
		_an.SetTrigger(attackHash);
		playDelay(_sourceAttack, FxAttack, 0.7f);
		_sourceAttack.volume = GameManager.instance.volumen;
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
	public void kill()
	{
		// Guardo que estoy muerto, disparo trigger "Dead" y desactivo el collider
		// TODO
		_dead = true;
		_an.SetTrigger(deadHash);
		play(_sourceDeath, FxDead);
		_box.enabled = false;	
		_alerted = false;

		// Notifico al GameManager que he sido eliminado
		// TODO
		GameManager.instance.notifyEnemyKilled(this);
	}
	// Funcion para resetear el collider (activado por defecto), la variable donde almaceno si he muerto y forzar el estado "Idle" en Animator
	public void reset()
	{
		// TODO
		if (_box != null)
		{
			_rb.MovePosition(originalPosition);
			if (_box.enabled == false) _box.enabled = true;			
		}

		if (_dead) _dead = false;		

		if (_alerted)
		{
			_an.ResetTrigger(chaseHash);
			_alerted = false;
		}
		enemySetToIdle();	
	}
	public void enemySetToIdle()
	{
		if (_an != null) _an.Play(idleState);
	}
	private void OnCollisionEnter(Collision collision)
	{
		// Obtener el estado actual
		// Si el estado es 'Attack' y el parametro Distance es > 0 atacamos a Player (comprobar etiqueta).
		// La Distancia >0 es para acotar el ataque sólo al momento que mueve la espada (no toda la animación).
		// TODO
		if (_an.GetCurrentAnimatorStateInfo(0).fullPathHash == attackState)
		{
			if (collision.collider.gameObject.tag == "Player" && _an.GetFloat("Distance") > 0)
				_player.recieveDamage();
		}
	}
}
