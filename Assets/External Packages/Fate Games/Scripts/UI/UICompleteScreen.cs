using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace FateGames
{
    public class UICompleteScreen : UIElement
    {
        [HideInInspector] public bool Success = false;
        [SerializeField] private TextMeshProUGUI completeText = null;
        [SerializeField] private TextMeshProUGUI continueText = null;
        [SerializeField] private TextMeshProUGUI coinText = null;
        [SerializeField] private GameObject[] starObjects = null;

        public void SetScreen(bool success, int level)
        {
            if(success)
                coinText.text = "+" + ((MainLevelManager)LevelManager.Instance).Coin;
            else
            {
                coinText.text = "+0";
                foreach (GameObject star in starObjects) star.SetActive(false);
            }
            completeText.text = GameManager.Instance.LevelName + " " + (success ? " COMPLETED" : " FAILED");
            continueText.text = success ? "NEXT LEVEL" : "TRY AGAIN";
        }

        protected override void Animate()
        {
            return;
        }

        // Called by ContinueButton onClick
        public void Continue()
        {
            GameManager.Instance.LoadCurrentLevel();
        }
    }
}