using UnityEngine;
using System.Collections;

public class ExtinguishFire : MonoBehaviour {

	public AudioClip fireSound;

	public AudioClip extinguisherSound;

	public long maxHits = 100;

	public float initialFireSize = 3;

	public float fireReducer = 0.999f;

	private long hits = 0;

	private ParticleSystem[] systems;
	
	private float timer;

	private float audioCurrentPosition;

	private AudioSource audio;

	// Use this for initialization
	void Start () {
		systems = GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem system in systems) {
			timer = Mathf.Max(system.startLifetime, timer);
			system.startSize *= initialFireSize;
			system.startSpeed *= initialFireSize;
			system.startLifetime *= Mathf.Lerp (initialFireSize, 1, 0.5f);
			system.Clear ();
			system.Play ();
		}

		audio = GetComponent<AudioSource>();
		audio.clip = fireSound;
		audio.Play();
		audioCurrentPosition = 0;
	}
	
	// Update is called once per frame
	void Update () {
		audioCurrentPosition += Time.deltaTime;
		if(audioCurrentPosition >= audio.clip.length) {
			audio.Play();
			audioCurrentPosition = 0;
		}

		GameObject extinguisher = GameObject.FindGameObjectWithTag ("Extinguisher");
		if (extinguisher != null) {
			Ray ray = new Ray (extinguisher.transform.position, extinguisher.transform.forward);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
				if (hit.collider.gameObject.Equals (gameObject)) {
					Debug.Log (gameObject.name + ": " + hits++);
					foreach (ParticleSystem system in systems) {
						audio.volume *= fireReducer;
						system.startSize *= fireReducer;
						system.startSpeed *= fireReducer;
						system.startLifetime *= Mathf.Lerp(fireReducer, 1, 0.5f);
						if (hits >= maxHits) {
							system.enableEmission = false;
							BroadcastMessage("Extinguish", SendMessageOptions.DontRequireReceiver);
						}
					}
				}
			}
		}
		if (hits >= maxHits) {
			timer -= Time.deltaTime;
			if(timer <= 0) {
				Destroy(gameObject);
			}
		}
	}
}
