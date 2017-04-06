// csSkillAttack.cs
// 		Get rid of the halo object after 1 second to clean up.

using UnityEngine;
using System.Collections;

public class csSkillAttack : MonoBehaviour 
{
	void  Start()
	{
		// Remove this particle game object 1 sec after initialized.
		Destroy(this.gameObject, 1);
	}
}
