using UnityEngine;

public enum DifficultyId
{
    // enum値は各EnemyProfileSOのDifficultyIdと一致させる。
    Easy,
    Normal,
    Hard,
    Master,
    Transcendent
}

[CreateAssetMenu(fileName = "EenmyDifficultyListSO", menuName = "EenmyDifficultyListSO")]
public class EenmyDifficultyListSO : ScriptableObject
{
    [SerializeField] Data[] datas;
    public Data[] Datas => datas;

    [System.Serializable]
    public class Data
    {
        public DifficultyId difficultyId;
        public string tag;
        public EnemyProfileSO profileSO;
    }

}