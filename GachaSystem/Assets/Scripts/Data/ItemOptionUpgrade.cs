using UnityEngine;

namespace Data
{
	[CreateAssetMenu(fileName = "Data", menuName = "Data/ItemOptionUpgrade")]
	public class ItemOptionUpgrade : ScriptableObject
	{
		public int upgradeBelowLimit;	 // 강화 단계 이하 조건
		public int upgradeCost;	 // 단계 별 요구 비용
		public int normalUpgradeValue;	 // 일반 등급 강화 능력치 증가량
		public int rareUpgradeValue;	 // 고급 등급 강화 능력치 증가량
		public int epicUpgradeValue;	 // 희귀 등급 강화 능력치 증가량

	}
}
