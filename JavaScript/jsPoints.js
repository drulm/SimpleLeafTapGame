#pragma strict

private var moveSpeed : 			float 	= 3;				// The move speed

function Start () {

}

function Update () {
	var moveStep: float = moveSpeed * Time.deltaTime;			// The move delta time
	
	transform.Translate(Vector3.up * moveStep);

	GetComponent.<Renderer>().material.color.a -= Time.deltaTime;
	if (GetComponent.<Renderer>().material.color.a <= 0.0) {
		Destroy(this.gameObject);
	}
}
