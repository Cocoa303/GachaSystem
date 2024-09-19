using UnityEngine;

namespace Data
{
	[CreateAssetMenu(fileName = "Data", menuName = "Data/ItemList")]
	public class ItemList : ScriptableObject
	{
		public int itemID;	 // 아이템 고유 ID
		public ItemGrade itemGrade;	 // 아이템 등급
		public ItemOptionType itemOptionType;	 // 아이템 옵션에 사용 될 옵션 타입
		public int defaultValue;	 // 1레벨 기본 능력치값
		public string iconPath;	 // 아이콘 경로
		public enum ItemGrade
		{
			Normal,
			Rare,
			Epic
		}
		public enum ItemOptionType
		{
			AttackIncrease,
			DefenseIncrease,
			HpIncrease
		}

	}
}
