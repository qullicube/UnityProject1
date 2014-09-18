using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class SpriteAnimator : MonoBehaviour
{
		Animator spriteAnimator;
		SpriteRenderer spriteRenderer;
		
		//down-left = 0, up-left = 1, etc.
		public void Play (string animationName, int orientation)
		{
				orientation = orientation % 4;
				
				var bit0 = orientation % 2;
				var bit1 = orientation / 2;
				
				var flipFront = bit0 ^ bit1;
				var flipSide = bit1;
		
				spriteAnimator.Play (animationName + flipFront.ToString ());
				spriteRenderer.material.SetFloat ("_Flip", flipSide == 0 ? 1.0f : -1.0f);
		}
		
		void Start ()
		{
				spriteAnimator = GetComponent<Animator> ();
				spriteRenderer = GetComponent<SpriteRenderer> ();
		}
}

/*

bit1 - 0 0 1 1
bit0 - 0 1 0 1

frontal = 0 1 1 0
side = bit1

*/