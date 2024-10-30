using MoreMountains.Feedbacks;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Garawell
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Button retryButton;
        [SerializeField] private Canvas endGameCanvas;
        [SerializeField] private CanvasGroup endGameCanvasGroup;
        [SerializeField] private MMF_Player comboFeedbacks;
        [SerializeField] private TMP_Text comboCountText, scoreText, endGameScoreText;

        public Action RetryClick;

        private IEnumerator AnimateCanvasGroup()
        {
            for (float time = 0; time < 1f; time += Time.deltaTime)
            {
                yield return null;
                endGameCanvasGroup.alpha = time;
            }

            endGameCanvasGroup.alpha = 1;
        }

        private void OnRetryClick()
        {
            retryButton.onClick.RemoveListener(OnRetryClick);
            RetryClick?.Invoke();
        }

        public void PlayComboFeedbacks(int comboCount)
        {
            comboCountText.text = comboCount.ToString();
            comboFeedbacks.PlayFeedbacks();
        }

        public void SetScore(int amount)
        {
            scoreText.text = amount.ToString();
        }

        public void ShowEndGamePanel(int score)
        {
            endGameCanvas.enabled = true;
            endGameScoreText.text = "SCORE: <color=green>" + score + "</color>";

            StartCoroutine(AnimateCanvasGroup());
            retryButton.onClick.AddListener(OnRetryClick);
        }
    }
}