using TMPro;
using UnityEngine;

public class TaskUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI detailText;
    public TextMeshProUGUI progressText;
    public GameObject completedIcon;

    public void Setup(TaskSO def, TaskState st)
    {
        titleText.text = def.title;
        detailText.text = def.description;
        progressText.text = $"{st.progress}/{def.target}";
        completedIcon.SetActive(st.completed);
    }

    public void Refresh(TaskSO def, TaskState st)
    {
        Setup(def, st);
    }
}