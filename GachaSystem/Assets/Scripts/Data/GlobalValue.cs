using UnityEngine;

namespace Data
{
	[CreateAssetMenu(fileName = "Data", menuName = "Data/GlobalValue")]
	public class GlobalValue : ScriptableObject
	{
		public string globalValueID;	 // 전역변수로 사용할 대상
		public float value;	 // 고정 상수 값

	}
}
