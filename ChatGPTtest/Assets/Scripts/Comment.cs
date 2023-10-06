using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Comment : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Text aiComment;

    void Update()
    {
        aiComment.text = Settings.comment;
    }
}
