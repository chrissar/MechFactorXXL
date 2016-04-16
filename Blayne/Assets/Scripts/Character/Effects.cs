using UnityEngine;
using System.Collections;

public class Effects : MonoBehaviour {
    public GameObject prefab;
    public Transform mMuzzle;
    public bool fire = false;
    Vector3 fwd;
    RaycastHit hit;

    public AudioSource gun;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        fwd = mMuzzle.TransformDirection(Vector3.forward);
        if (Input.GetButtonDown("Fire1") && Physics.Raycast(mMuzzle.position, fwd, out hit, 40))
        { //when we left click and our raycast hits something
            //Instantiate(prefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)); //then we'll instantiate a random bullet hole texture from our array and apply it where we click and adjust// the position and rotation of textures to match the object being hit
            gun.Play();
        }
    }
}
