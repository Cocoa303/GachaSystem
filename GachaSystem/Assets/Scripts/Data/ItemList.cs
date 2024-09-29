using UnityEngine;
namespace Data
{
	[CreateAssetMenu(fileName = "Data", menuName = "Data/ItemList")]
	public class ItemList : ScriptableObject
	{
		public int itemID;	 // 아이템 고유 ID
		public Common.Enum.ItemGrade itemGrade;	 // 아이템 등급
		public Common.Enum.ItemOptionType itemOptionType;	 // 아이템 옵션에 사용 될 옵션 타입
		public int defaultValue;	 // 1레벨 기본 능력치값
		public string atlasName;	 // 아틀라스 이름
		public string iconName;	 // 아이콘 이름

	}
}
