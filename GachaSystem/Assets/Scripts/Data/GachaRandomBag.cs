using UnityEngine;
using System.Collections.Generic;

namespace Data
{
	[CreateAssetMenu(fileName = "Data", menuName = "Data/GachaRandomBag")]
	public class GachaRandomBag : ScriptableObject
	{
		public int dropID;	 // 랜덤백 그룹 ID
		public List<Common.Enum.GachaRewardGrade> gachaRewardGrade;	 // 가챠 결과물의 등급
		public List<Common.Enum.GachaRewardItemType> gachaRewardItemType;	 // 가챠 결과물의 타입 분류 값
		public List<int> gachaRewardID;	 // 가챠 결과물의 고유 ID

	}
}
