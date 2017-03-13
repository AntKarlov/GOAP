using System;
using UnityEngine;

namespace Gpgoap
{
	[Serializable]
	public class WorldState {

		[SerializeField]
		private bool[] values;  // Values for atoms.
		[SerializeField]
		private bool[] mask;
//		private int mask;  // Mask for atoms that do not matter.

		public WorldState() {
			Clear();
		}

		public void Clear() {
			values = new bool[32];
			mask = new bool[32];
		}


//		ws->values = value ? ( ws->values | ( 1LL << idx ) ) : ( ws->values & ~( 1LL << idx ) );
//		ws->dontcare &= ~( 1LL << idx );
		public bool Set(int index, bool value) {
//			Debug.Log("Set " + index + ":" + value);
			if (index != -1) {
				values[index] = value;
				mask[index] = true;
//				values = value ? ( values | ( 1 << index ) ) : ( values & ~( 1 << index ) );
//				mask &= ~(1 << index);
				return true;
			}
			return false;
		}

		public int Heuristic(WorldState other) {
//			int care = (other.mask ^ -1);
//			int diff = ((values & care) ^ ( other.values & care ));
			int dist = 0;
			for ( int i = 0, j = mask.Length; i < j; i++ )
				if (other.mask[i])
					if (values[i] != other.values[i])
						dist++;
//				if ( ( diff & ( 1 << i ) ) != 0 ) dist++;
			return dist;
		}


		public bool Match(WorldState other) {
			//			int care = (other.mask ^ -1);
			//			Debug.Log(values+" match "+other.values + " :: "+(values & care)+" care "+(other.values& care));
			for (int i = 0, j = mask.Length; i < j; i++)
				if ((other.mask[i] && mask[i]) && (values[i] != other.values[i]))
					return false;
			return true;// ((values & care) == (other.values & care));
		}

		//					if ( ( post.dontcare & ( 1 << i ) ) == 0)
		public bool Masked(int index) {
			return mask[index]; //(mask & (1 << index)) == 0;
		}

		public bool Value(int index) {
			return values[index]; //(values & (1 << index)) != 0;
		}

		public WorldState Copy() {
			WorldState copy = new WorldState();
			for (int i = 0, j = mask.Length; i < j; i++) {
				copy.mask[i] = mask[i];
				copy.values[i] = values[i];
			}
			return copy;
		}

		public void Act(WorldState post) {

			for (int i = 0, j = mask.Length; i < j; i++) {
				//				values[i] = !post.mask[i] && values[i] || post.values[i] && post.mask[i];
				//				mask[i] = mask[i] && post.mask[i];
				mask[i] = mask[i] || post.mask[i];
				if (post.mask[i])
					values[i] = post.values[i];
//				values[i] = mask[i] && (post.values )//(values[i] && post.values[i]);
			}

//			int unaffected = post.mask;
//			int affected = ( unaffected ^ -1);
//			values = ( values & unaffected ) | ( post.values & affected );
//			mask &= post.mask;
		}

//		public bool Equals(WorldState other) {
//			return false; //this.values == other.values && this.mask == other.mask;
//		}

		public bool Equals(WorldState other) {
			for (int i = 0, j = values.Length; i < j; i++)
				if (values[i] != other.values[i])
					return false;
			return true; //values?this.values == other.values:this.mask == other.mask;
		}

		public bool[] Description(bool a = false) {
//			bool[] result = new bool[ActionPlanner.ATOMS];
////			Debug.Log("goap_worldstate_description not implemented yet");
////			int added = 0;
//			for ( int i = 0; i < ActionPlanner.ATOMS; ++i ) {
//				if (Masked(i))//( ws.dontcare & ( 1 << i ) ) == 0)
//				{
//					result[i] = ((values & ( 1 << i ) ) != 0);
////					bool set =
////					result += set?"T":"_";
//					//					added = snprintf(buf, sz, "%s,", set?upval:val);
//					//					buf += added; sz -= added;
//					//					buf += string.Format("%s,", set?upval:val);
//				}
//			}
//			return result;//"Description";
			if (a)
				return values;
			bool[] result = new bool[mask.Length];
			for (int i = 0, j = mask.Length; i < j; i++) {
				result[i] = mask[i] && values[i];
			}
			return result;
		}

	}
}