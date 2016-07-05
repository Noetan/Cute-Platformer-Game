using UnityEngine;
using System.Collections;
using Chronos;

namespace RadialMenu {
	
	public class RadialMenu : MonoBehaviour {
		
		public Camera cam; 								//GUI camera.
		public Transform ring; 							//Transform of ring mesh.
		public Transform selector; 						//Transform of selector mesh.
		
		public bool is2D = true;						//2D or 3D Radial Menu.
		public bool debugMenu = false;					//Debug RadialMenu to always be open to allow for visual adjustments in inspector.
		public bool gamePad = false;					//Toggles gamepad use.
		
		[Range(1,20)]
		public float sensitivty = 7f;					//Selector movement sensetivity. Default 7.
		public bool followCursor = false;				//Radial Menu will open at cursor position on MouseDown.
		public Vector2 menuPositionOffset;				//Offset of RadialMenu from center point.
		public float selectorTravel = 1f; 				//Maximum tavel of selector. Values of 0.9 - 1.2 are ideal.
		public float selectorInitialScale = 0.8f;		//End Scale size for selector.
		public float selectorHighlightScale = 1.2f;		//End Scale size for selector.
		[Range(1,100)]
		public int activeBuffer = 90; 					//Percentage distance of Max Radius that actions will activate. Default is 90%.
		[Range(1,100)]
		public float angleThreshold = 100f; 			//The Angle threshold between selector selector and action items. The lower the threshold, the larger the selection gap hotspot is between each item. This is more apparent with less items on a menu.
		public float itemStartDistance = -2f;			//Distance inwards that an action items starts before animation outwards. 0 for no animation.
		public float itemDistanceRadius = 1f;			//Action Icon distance offset from center of radial menu.
		[Range(0,360)]
		public float rotationOffset = 0f;				//Rotational offset of all menu items. Default 0: First icon is always at top of Menu.
		[Range(-180,180)]	
		public float stepOffset = 0f;					//Distance stepping offset of all menu items. Moves them closer together or further appart.
		public float screenEdgeBuffer;					//Buffer distance from screen edges that RadialMenu will not enter. Only used if Follow Cursor enabled.
		public bool seqentialItemAnimation = false;		//Enable/Disable sequential icon animation.
		public float itemHighlightScale = 1.5f;			//End scale size for item highlight.
		public float itemAnimationDelay = .1f;			//Delay for icon animation.
		public float itemAnimationTime = .3f;			//Length of time for icon animation.
		public iTween.EaseType easeType;				//Easing Type dropdown in inspector.
		public RadialActions[] actions;					//Array of action icons.
        public float timeScaleWhenOpen = 0.33f;         // Set world timescale when radial menu is open
		
		
		private Vector3 selectorCenter;
		private Vector3 selectorMove;
		[System.NonSerialized]
		public float stepAngle;
		private bool stateActive = false;
		// Selected message to broadcast if activated.
		private string activeMessage;
		private GameObject activeTarget;
		private Color selectorColor;
		private Color ringColor;
		private bool isAnimating = false;
		private Vector2 touchDelta;
		private Vector2 joyInput;
		private bool canJoy = false;
		
		
		private void Awake () {
			PopulateMenu();
			selectorCenter = selector.localPosition;
			selectorColor = selector.GetComponent<Renderer>().material.color;
			ringColor = ring.GetComponent<Renderer>().material.color;
			
			if(!debugMenu)
				transform.localScale = Vector3.zero;
			
			//if(gamePad)
				followCursor = true;
	
			try {	
				joyInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
				canJoy = true;
			} catch {
				Debug.LogWarning("RadialMenu warning: Missing input axis for Joy/Gamepad. Check if Joy X or Joy Y axis exists in Input Manager");
				canJoy = false;
				joyInput = Vector2.zero;
			}
			
		}
		
		
		// Update is called once per frame
		private void Update () {
			
			if(!debugMenu) {
				if(Input.GetButtonDown("Radial Menu")) {
					ActivateMenu();
				}
				
				if(Input.GetButton("Radial Menu")) {
					HandleSelectorMovement();
				}
				
				if(Input.GetButtonUp("Radial Menu")) {
					if(activeTarget != null)
						//Broadcast to mesage to target object.
						activeTarget.SendMessage(activeMessage);
					DisableMenu();
				}
			} else {
				transform.localScale = Vector3.one;
				//followCursor = false;
				DebugMenu();
				HandleSelectorMovement();
			}
			
			//Touch Input
			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {
				touchDelta = Input.GetTouch(0).deltaPosition;
			} else {
				touchDelta = Vector2.zero;
			}

			if(canJoy)
				joyInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis(/*"Joy Y"*/"Vertical"));
			
		}
		
		private void HandleSelectorMovement() {
			
			Vector3 snap;

			float mouseX = Input.GetAxis("Mouse X") / (50/sensitivty);
			float mouseY = Input.GetAxis("Mouse Y") / (50/sensitivty);
			float joyX = joyInput.x * sensitivty/7;
			float joyY = joyInput.y * sensitivty/7;
			float touchX = touchDelta.x * sensitivty/7;
			float touchY = touchDelta.y  * sensitivty/7;

			//Combines all input types into one translation for simultaneous input adjustment. Usefull for input switching mid game.
			float x = Mathf.Clamp(mouseX + joyX + touchX, -1f, 1f);
			float y = Mathf.Clamp(mouseY + joyY + touchY, -1f, 1f);

			if(gamePad) {
				snap = Vector3.zero;
			} else {
				snap = selector.localPosition;
			}
			
			selectorMove  = snap + new Vector3(x, y, 0f);
			
			selector.localPosition = GetConstrainedPosition(selectorCenter, selectorMove);
			CheckPos(selector.localPosition);
		}
		
		
		private void PopulateMenu() {
			
			selector.localScale = new Vector3(selectorInitialScale, selectorInitialScale, selectorInitialScale);
			
			int i = 0;
			int itemLength = actions.Length;
			stepAngle = 360/itemLength;
			
			foreach (RadialActions item in actions) {
				
				if(is2D && item.mesh.childCount == 0) {
					Debug.LogError("Error: Action Item gameObjects for 2D RadialMenu needs child texture gameObjects. Please refer to the Docs on how to create a 2D RadialMenu");
					return;
				}
				
				if(!is2D && item.mesh.GetComponent<Renderer>() == null) {
					Debug.LogError("Error: Action Item gameObjects for 3D RadialMenu needs a material. Please refer to the Docs on how to create a 3D RadialMenu");
					return;
				}
				
				
				GameObject container = new GameObject();
				container.name = "ActionIcon"+i;
				container.transform.parent = transform;
				//Change Z value to put items in front or behind ring.
				container.transform.localPosition = new Vector3(0f, 0f, -1f);
				item.mesh.parent = container.transform;
				item.mesh.localPosition = new Vector3(0f, itemDistanceRadius, 0f);
				container.transform.localEulerAngles = new Vector3(0f, 0f, rotationOffset + (stepAngle + stepOffset)*i);
				
				i++;
			}
		}
		
		
		public void ActivateMenu() {

			isAnimating = true;
			selector.localPosition = Vector3.zero;
			iTween.ScaleTo( transform.gameObject, new Vector3(1,1,1), 0.5f);
			
			//Determins where the menu will open on screen.
			MenuPosition();
			
			int i = 0;
			
			foreach (RadialActions item in actions) {
				
				GameObject targetItem;
				
				targetItem = is2D ? item.mesh.GetChild(0).gameObject : item.mesh.gameObject;
				
				iTween.Stop(item.mesh.gameObject);
				if(targetItem != item.mesh.gameObject)
					iTween.Stop(targetItem);
				
				//Adjust roation offset
				item.mesh.parent.transform.localEulerAngles = new Vector3(0f, 0f, rotationOffset + (stepAngle+stepOffset)*i);
				
				float delay = (seqentialItemAnimation ? i : 0)/20f + itemAnimationDelay;
				Color color = targetItem.GetComponent<Renderer>().material.color;
				color.a = 0f;
				targetItem.GetComponent<Renderer>().material.color = color;
				
				item.mesh.localScale = new Vector3( 0f, 0f, 0f);
				item.mesh.localPosition = new Vector3( 0f, itemStartDistance, 0f);
				
				iTween.FadeTo(targetItem, iTween.Hash(
					"alpha", 0.5f, 
					"islocal", true, 
					"time", itemAnimationTime, 
					"delay", delay,
					"easeType", easeType,
					"oncomplete", "AnimComplete",
					"oncompletetarget", gameObject)
				              );
				
				iTween.ScaleTo(item.mesh.gameObject, iTween.Hash(
					"scale", new Vector3(1,1,1), 
					"islocal", true, 
					"time", itemAnimationTime, 
					"delay", delay,
					"easeType", easeType)
				               );
				
				
				if(itemStartDistance != 0 || itemDistanceRadius != 0) {
					
					iTween.MoveTo(item.mesh.gameObject, iTween.Hash(
						"position", new Vector3(0,itemDistanceRadius,0), 
						"islocal", true, 
						"time", itemAnimationTime, 
						"delay", delay,
						"easeType", easeType)
					              );
					
				}
				
				i++;
			}
			
		}
		
		
		private void CheckPos(Vector3 pos) {
			
			// Get selector angle
			Vector2 v2 = new Vector2(pos.x, pos.y);
			float selectorAngle = Angle(v2);
			
			int num = actions.Length;
			
			foreach (RadialActions item in actions) {
				
				// Get delta of selector and item angle
				float itemAngle = -item.mesh.parent.localEulerAngles.z;
				float delta = Mathf.Abs(Mathf.DeltaAngle(selectorAngle, itemAngle));
				Color color;
				
				if(delta < (((stepAngle + stepOffset)/2) * angleThreshold/100) && stateActive) {
					//ICON IS HIGHLIGHTED
					if(!isAnimating) {
						item.mesh.localScale = Vector3.Slerp(item.mesh.localScale, new Vector3(itemHighlightScale, itemHighlightScale, itemHighlightScale), Time.deltaTime * 10f);
						if(is2D){
							color = item.mesh.GetChild(0).GetComponent<Renderer>().material.color;
							color.a = 1f;
							item.mesh.GetChild(0).GetComponent<Renderer>().material.color = Color.Lerp(item.mesh.GetChild(0).GetComponent<Renderer>().material.color, color, Time.deltaTime * 20f);
						} else {
							color = item.mesh.GetComponent<Renderer>().material.color;
							color.a = 1f;
							item.mesh.GetComponent<Renderer>().material.color = Color.Lerp(item.mesh.GetComponent<Renderer>().material.color, color, Time.deltaTime * 20f);
						}
					}
					activeMessage = item.message;
					activeTarget = item.targetObject;
				} else {
					//REMOVE HIGHLIGHT
					if(!isAnimating) {
						item.mesh.localScale = Vector3.Slerp(item.mesh.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * 10f);
						if(is2D){
							color = item.mesh.GetChild(0).GetComponent<Renderer>().material.color;
							color.a = 0.5f;
							item.mesh.GetChild(0).GetComponent<Renderer>().material.color = Color.Lerp(item.mesh.GetChild(0).GetComponent<Renderer>().material.color, color, Time.deltaTime * 20f);
						} else {
							color = item.mesh.GetComponent<Renderer>().material.color;
							color.a = 0.5f;
							item.mesh.GetComponent<Renderer>().material.color = Color.Lerp(item.mesh.GetComponent<Renderer>().material.color, color, Time.deltaTime * 20f);
						}
					}
					num--;
				}
			}
			
			//Clears active item vars if selector isn't lined up with anything.
			if(num == 0) {
				activeMessage = null;
				activeTarget = null;
				selector.GetComponent<Renderer>().material.color = Color.Lerp(selector.GetComponent<Renderer>().material.color, selectorColor, Time.deltaTime * 20f);
				ring.GetComponent<Renderer>().material.color = Color.Lerp(ring.GetComponent<Renderer>().material.color, ringColor, Time.deltaTime * 20f);
				selector.localScale = Vector3.Slerp(selector.localScale, new Vector3(selectorInitialScale,selectorInitialScale,selectorInitialScale), Time.deltaTime * 20f);
			} else {
				Color newSelectorcolor = selectorColor;
				newSelectorcolor.a = 0.9f;
				selector.GetComponent<Renderer>().material.color = Color.Lerp(selector.GetComponent<Renderer>().material.color, newSelectorcolor, Time.deltaTime * 10f);
				ring.GetComponent<Renderer>().material.color = Color.Lerp(ring.GetComponent<Renderer>().material.color, newSelectorcolor, Time.deltaTime * 20f);
				selector.localScale = Vector3.Slerp(selector.localScale, new Vector3(selectorHighlightScale,selectorHighlightScale,selectorHighlightScale), Time.deltaTime * 20f);
			}
			
		}
		
		private void MenuPosition() {
			
			/*if(followCursor) {
				
				float screenRatio = cam.aspect;
				float buffer = screenEdgeBuffer/100f;
				Vector3 mouseTrans = cam.ScreenToViewportPoint(Input.mousePosition);
				
				if(mouseTrans.x < buffer / screenRatio)
					mouseTrans.x = buffer / screenRatio;
				
				if(mouseTrans.x > 1-buffer / screenRatio)
					mouseTrans.x = 1-buffer / screenRatio;
				
				if(mouseTrans.y < buffer )
					mouseTrans.y = buffer ;
				
				if(mouseTrans.y > 1-buffer )
					mouseTrans.y = 1-buffer ;
				
				Vector3 newPos = cam.ViewportToWorldPoint(new Vector3(mouseTrans.x, mouseTrans.y, transform.localPosition.z));
				transform.position = newPos;
				
				
			} else {*/
				
				transform.position = cam.ViewportToWorldPoint(new Vector3(0.5f + menuPositionOffset.x/100f, 0.5f + menuPositionOffset.y/100f, transform.localPosition.z));

			//}
			
		}
		
		
		
		private void DisableMenu() {
			stateActive = false;
			iTween.Stop(transform.gameObject);
			iTween.ScaleTo( transform.gameObject, new Vector3(0,0,0), 0.5f);
			iTween.FadeTo( transform.gameObject, 0f, 0.25f);
		}
		
		
		private static float Angle(Vector2 p_vector2) {
			if (p_vector2.x < 0) {
				return 360 - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1);
			} else {
				return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
			}
		}
		
		
		private Vector3 GetConstrainedPosition (Vector3 midPoint, Vector3 endPoint) {
			
			float perc = activeBuffer/100f;
			
			if(Vector3.Distance(midPoint, endPoint) > selectorTravel * perc) {
				stateActive = true;
			} else {
				stateActive = false;
				activeMessage = null;
				activeTarget = null;
			}
			
			//Check for max length
			if (Vector3.Distance(midPoint, endPoint) > selectorTravel) {
				//get the direction of the line between mid point and end point
				Vector3 dirVector = endPoint - midPoint;
				dirVector.Normalize();
				return (dirVector * selectorTravel) + midPoint;    
			}
			//return the original position
			return endPoint;	
		}
		
		
		private void AnimComplete() {
			isAnimating = false;
		}
		
		#region --- Debug
		
		private void DebugMenu() {
			
			int i = 0;
			
			foreach (RadialActions item in actions) {
				item.mesh.localPosition = new Vector3(0f, itemDistanceRadius, 0f);
				item.mesh.parent.transform.localEulerAngles = new Vector3(0f, 0f, rotationOffset + (stepAngle+stepOffset)*i);
				i++;
			}
			
			MenuPosition();
			
		}
		#endregion --- Debug
	}
}
