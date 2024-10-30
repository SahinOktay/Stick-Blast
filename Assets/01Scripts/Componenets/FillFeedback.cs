using UnityEngine;
using MoreMountains.Feedbacks;
using TMPro;
using System;
using PoolSystem;

namespace Garawell
{
    public class FillFeedback : MonoBehaviour, IPoolable
    {
        [SerializeField] private MMF_Player feedbackPlayer;
        [SerializeField] private TMP_Text scoreText;

        public Action<FillFeedback> FeedbackComplete;

        public void PlayScoreFeedback(int score)
        {
            scoreText.text = "+" + score;
            feedbackPlayer.PlayFeedbacks();
            feedbackPlayer.Events.OnComplete.AddListener(OnFeedbackComplete);
        }

        private void OnFeedbackComplete()
        {
            FeedbackComplete?.Invoke(this);
        }

        public void Reset()
        {

        }
    }
}