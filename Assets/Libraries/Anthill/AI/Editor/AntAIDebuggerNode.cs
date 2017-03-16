using System.Collections.Generic;
using UnityEngine;

namespace Anthill.AI
{
	public class AntAIDebuggerNode
	{
		public Rect rect;
		public string title;
		public string value;
		public bool isDragged;
		public bool isSelected;
		public Vector2 outputOffset;
		public Vector2 inputOffset;
		public List<KeyValuePair<AntAIDebuggerNode, Color>> links;

		public GUIStyle currentStyle;
		public GUIStyle defaultNodeStyle;
		public GUIStyle selectedNodeStyle;

		public AntAIDebuggerNode(float aX, float aY, float aWidth, float aHeight, 
			GUIStyle aDefaultStyle, GUIStyle aSelectedStyle)
		{
			rect = new Rect(aX, aY, aWidth, aHeight);
			currentStyle = aDefaultStyle;
			defaultNodeStyle = aDefaultStyle;
			selectedNodeStyle = aSelectedStyle;
			links = new List<KeyValuePair<AntAIDebuggerNode, Color>>();
		}

		public void SetOutput(float aX, float aY)
		{
			outputOffset = new Vector2(aX, aY);
		}

		public void SetInput(float aX, float aY)
		{
			inputOffset = new Vector2(aX, aY);
		}

		public void LinkTo(AntAIDebuggerNode aNode, Color aColor)
		{
			links.Add(new KeyValuePair<AntAIDebuggerNode, Color>(aNode, aColor));
		}

		public void Drag(Vector2 aDelta)
		{
			rect.position += aDelta;
		}

		public void Draw()
		{
			GUI.Box(rect, title, currentStyle);
		}

		public bool ProcessEvents(Event aEvent)
		{
			switch (aEvent.type)
			{
				case EventType.MouseDown :
					if (aEvent.button == 0)
					{
						if (rect.Contains(aEvent.mousePosition))
						{
							isDragged = true;
							GUI.changed = true;
							isSelected = true;
							currentStyle = selectedNodeStyle;
						}
						else
						{
							GUI.changed = true;
							isSelected = false;
							currentStyle = defaultNodeStyle;
						}
					}

					if (aEvent.button == 1 && isSelected && 
						rect.Contains(aEvent.mousePosition))
					{
						//ProcessContextMenu();
						aEvent.Use();
					}
				break;

				case EventType.MouseUp :
					isDragged = false;
				break;

				case EventType.MouseDrag :
					if (aEvent.button == 0 && isDragged)
					{
						Drag(aEvent.delta);
						aEvent.Use();
						return true;
					}
				break;
			}
			return false;
		}

		public Vector2 Output
		{
			get { return new Vector2(rect.x + outputOffset.x, rect.y + outputOffset.y); }
		}

		public Vector2 Input
		{
			get { return new Vector2(rect.x + inputOffset.x, rect.y + inputOffset.y); }
		}
	}
}