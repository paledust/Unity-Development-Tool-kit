using System;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace HUD
{
    public struct TextDisplayData
    {
        public string content;
        public float riseDist;
        public float textSize;
        public Color textColor;
        public TextDisplayData(string content, Color textColor, float riseDist = 30, float textSize = 24)
        {
            this.content = content;
            this.riseDist = riseDist;
            this.textSize = textSize;
            this.textColor = textColor;
        }
    }

    public class HUD_Text : MonoBehaviour, IHUD
    {
        [SerializeField] private TextMeshProUGUI textNum;

        public void Init(TextDisplayData displayData, float animeDurationMulti, Action OnRecycle)
        {
            textNum.text = displayData.content;
            textNum.color = displayData.textColor;
            textNum.fontSize = displayData.textSize;

            // 最终位置 = 当前位置 + 反方向偏移 + 随机偏移
            Vector3 finalPosition = transform.position + Vector3.up * displayData.riseDist;

            transform.localScale = Vector2.zero;
            //跳字动画
            transform.DOScale(Vector2.one * 1.5f, 0.5f * animeDurationMulti).SetEase(Ease.OutBack);
            transform.DOMove(finalPosition, 1.5f * animeDurationMulti).SetEase(Ease.OutCirc);
            textNum.DOFade(0, 0.5f * animeDurationMulti).SetDelay(1f * animeDurationMulti).OnComplete(() =>
            {
                OnRecycle?.Invoke();
            });
        }
        public void Init(TextDisplayData displayData, Action OnRecycle) => Init(displayData, 1, OnRecycle);

        void OnDestroy()
        {
            textNum.DOKill();
            transform.DOKill();
        }
    }
}