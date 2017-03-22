using UnityEngine;

namespace Anthill.AI
{
	public class AntAIBlackboardProp
	{
		public enum ValueType
		{
			String,
			Float,
			Int,
			Bool,
			Vec2,
			Vec3
		}

		public delegate void ValueUpdateDelegate(AntAIBlackboardProp aProperty);
		public event ValueUpdateDelegate EventChanging;
		public event ValueUpdateDelegate EventChanged; 

		public string _strValue;
		public float _floatValue;
		public int _intValue;
		public bool _boolValue;
		public Vector2 _vec2Value;
		public Vector3 _vec3Value;
		public ValueType _type;

		public AntAIBlackboardProp()
		{
			_type = ValueType.String;
		}

		#region Public Methods

		public override string ToString()
		{
			string result = "";
			switch (_type)
			{
				case ValueType.String :
					result = _strValue;
				break;

				case ValueType.Float :
					result = _floatValue.ToString("00");
				break;

				case ValueType.Int :
					result = _intValue.ToString();
				break;

				case ValueType.Bool :
					result = _boolValue.ToString();
				break;

				case ValueType.Vec2 :
					result = string.Format("{0}, {1}", 
						_vec2Value.x.ToString(), 
						_vec2Value.y.ToString());
				break;

				case ValueType.Vec3 :
					result = string.Format("{0}, {1}, {2}", 
						_vec3Value.x.ToString("00"), 
						_vec3Value.y.ToString("00"), 
						_vec3Value.z.ToString("00"));
				break;
			}
			return result;
		}

		#endregion
		#region Getters/Setters

		public ValueType Type
		{
			get { return _type; }
		}

		public string Value
		{
			get { return _strValue; }
			set
			{ 
				if (EventChanging != null)
				{
					EventChanging(this);
				}

				_type = ValueType.String;
				_strValue = value;
				if (EventChanged != null)
				{
					EventChanged(this);
				}
			}
		}

		public float AsFloat
		{
			get { return _floatValue; }
			set
			{
				if (EventChanging != null)
				{
					EventChanging(this);
				}

				_type = ValueType.Float;
				_floatValue = value;
				if (EventChanged != null)
				{
					EventChanged(this);
				}
			}
		}

		public int AsInt
		{
			get { return _intValue; }
			set 
			{
				if (EventChanging != null)
				{
					EventChanging(this);
				}

				_type = ValueType.Int;
				_intValue = value;
				if (EventChanged != null)
				{
					EventChanged(this);
				}
			}
		}

		public bool AsBool
		{
			get { return _boolValue; }
			set
			{
				if (EventChanging != null)
				{
					EventChanging(this);
				}

				_type = ValueType.Bool;
				_boolValue = value;
				if (EventChanged != null)
				{
					EventChanged(this);
				}
			}
		}

		public Vector2 AsVector2
		{
			get { return _vec2Value; }
			set
			{
				if (EventChanging != null)
				{
					EventChanging(this);
				}

				_type = ValueType.Vec2;
				_vec2Value = value;
				if (EventChanged != null)
				{
					EventChanged(this);
				}
			}
		}

		public Vector3 AsVector3
		{
			get { return _vec3Value; }
			set
			{
				if (EventChanging != null)
				{
					EventChanging(this);
				}

				_type = ValueType.Vec3;
				_vec3Value = value;
				if (EventChanged != null)
				{
					EventChanged(this);
				}
			}
		}

		#endregion
	}
}