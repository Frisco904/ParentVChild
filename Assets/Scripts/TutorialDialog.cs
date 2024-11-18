using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialDialog : MonoBehaviour
{
        public TextMeshProUGUI textComponent;
        
        public string[] lines;
        public float textSpeed;
        public int index;

        void Start()
        {
            textComponent.text = string.Empty;
            StartDialog();
        }

        void Update() 
        {
            if (Input.anyKeyDown)
            {
                if (textComponent.text == lines[index]) {
                    NextLine();
                    return;
                }

                StopAllCoroutines();
                textComponent.text = lines[index];
            }

        }

        void StartDialog()
        {
            index = 0;
            StartCoroutine(TypeLine());

        }

        private IEnumerator TypeLine()
        {
            foreach (char c in lines[index].ToCharArray())
            {
                textComponent.text += c;
                yield return new WaitForSeconds(textSpeed);
            }
        }

        private void NextLine()
        {
            if (index < lines.Length - 1)
            {
                index++;
                textComponent.text = string.Empty;;
                StartCoroutine(TypeLine());
            } 
            else {
                //Check if AutoLevelStart has already started the level before calling the Start level function.
                if (!LevelManager.main.GetStartLevel()) { LevelManager.main.StartLevel(); }
                gameObject.SetActive(false);
            }

        }

}