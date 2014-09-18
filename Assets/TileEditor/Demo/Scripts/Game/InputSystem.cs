using UnityEngine;
using System.Collections;

//Handle global input of the game
[RequireComponent(typeof(GameSystem))]
public class InputSystem : MonoBehaviour
{
	#region Game Component
	
		GameSystem gameSystem;

	#endregion

	#region Start
	
		void Start ()
		{
				gameSystem = GetComponent<GameSystem> ();
		}
		
	#endregion
		
	#region Update
	
		void Update ()
		{
				OnInput ();
		}
	
	#endregion

		//Input of the system
		void OnInput ()
		{
				switch (gameSystem.state) {
				case GameState.PAUSE: 
						{	
								//Unpause game
								if (Input.GetButtonDown ("Start")) {
										gameSystem.State = GameState.BATTLE;
								}
						}
						break;
				case GameState.BATTLE:
						{
								//Pause game
								if (Input.GetButtonDown ("Start")) {
										gameSystem.State = GameState.PAUSE;
										return;
								}
					
								//Input states
								var rotateLeft = Input.GetButtonDown ("L1");
								var rotateRight = Input.GetButtonDown ("R1");
								var tilt = rotateLeft && rotateRight;
						
								//Rotate-Tilt
								if (tilt) {
										gameSystem.Camera_Tilt ();
								} else if (rotateLeft) {
										gameSystem.Camera_RotateLeft ();
								} else if (rotateRight) {
										gameSystem.Camera_RotateRight ();
								}
						
								//Move Tile Selector
								if (!OnMoveSelector ()) {
										//Action
										OnAction ();
								}
						}
						break;
				case GameState.FIELDMAP:
						{
								//Input states
								var rotateLeft = Input.GetButtonDown ("L1");
								var rotateRight = Input.GetButtonDown ("R1");
								var tilt = rotateLeft && rotateRight;

								//Rotate-Tilt
								if (tilt) {
										gameSystem.Camera_Tilt ();
								} else if (rotateLeft) {
										gameSystem.Camera_RotateLeft ();
								} else if (rotateRight) {
										gameSystem.Camera_RotateRight ();
								}

								//Walk
								var x = Input.GetAxisRaw ("Horizontal");
								var z = Input.GetAxisRaw ("Vertical");
								var jmp = Input.GetButtonDown ("X");

								gameSystem.FieldMap_MainPlayerMove (x, z);

								if (jmp) {
										gameSystem.FieldMap_MainPlayerJump (1.0f);
								}
						}
						break;
				default:
						break;
				}
		}
	
		float time;
		const float MIN_TIME = 0.1f; //TODO: find right fit (?)
		const float MAX_TIME = 0.3f;
	
		bool OnMoveSelector ()
		{
				//Return range -1 to 1
				var x = Input.GetAxis ("Horizontal");
				var z = Input.GetAxis ("Vertical");


				var processed = false;
				//if x && y is large, then move fast
				var multiplier = 1 - Mathf.Max (x * x, z * z);
				var threshold = Mathf.Lerp (MIN_TIME, MAX_TIME, multiplier);

				//Return only -1 0 1
				x = Input.GetAxisRaw ("Horizontal");
				z = Input.GetAxisRaw ("Vertical");
			
				//Move only after interval is reached
				if (time >= threshold && ((Mathf.Abs (x) > 0.01f || Mathf.Abs (z) > 0.01f))) {
						gameSystem.Selector_MoveBy ((int)x, (int)z);
						processed = true;
						time = 0.0f;
				}
		
				//Update time
				time += Time.deltaTime;
		
				return processed;
		}
	
		bool OnAction ()
		{
				var processed = false; //not sure yet if used
		
				if (Input.GetButtonDown ("X")) {
						//Select unit or DoMove
						if (gameSystem.selectedPlayer == null) {
								gameSystem.Selector_SelectPlayer ();
						} else {
								gameSystem.SelectedPlayer_MoveToSelectedMovable ();
						}
				} else if (Input.GetButtonDown ("^")) {
						gameSystem.Selector_DeselectPlayer ();
				}
		
				return processed;
		}
}
