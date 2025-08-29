using UnityEngine;

[CreateAssetMenu(fileName = "EenmyDifficultyListSO", menuName = "EenmyDifficultyListSO")]
public class EenmyDifficultyListSO : ScriptableObject
{
    [SerializeField] Data[] datas;
    public Data[] Datas => datas;

    [System.Serializable]
    public class Data
    {
        public string tag;
        public EnemyProfileSO profileSO;
    }

}