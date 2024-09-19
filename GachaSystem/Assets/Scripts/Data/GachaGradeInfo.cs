using UnityEngine;

namespace Data
{
	[CreateAssetMenu(fileName = "Data", menuName = "Data/GachaGradeInfo")]
	public class GachaGradeInfo : ScriptableObject
	{
		public int gachaID;	 // 뽑기 메뉴 별 고유 ID
		public int normalGachaRate;	 // 일반 등급 아이템의 드랍 확률
		public int rareGachaRate;	 // 고급 등급 아이템의 드랍 확률
		public int epicGachaRate;	 // 에픽 아이템의 드랍 확률
		public int gachaRandombagID;	 // 뽑기 랜덤백 테이블의 그룹 ID

	}
}
