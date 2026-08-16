using System;
using System.Collections;
using UnityEngine;
using EasingFunc;

namespace CoroutineUtil
{
    public static class CommonCoroutine{
        public static IEnumerator delayAction(Action action, float delay){
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
        public static IEnumerator coroutineFadeSprite(SpriteRenderer m_sprite, float targetAlpha, float duration, float delay = 0){
            yield return new WaitForSeconds(delay);
            if(targetAlpha == 1) m_sprite.enabled = true;

            Color initColor = m_sprite.color;
            Color targetColor = initColor;
            targetColor.a = targetAlpha;

            yield return new WaitForLoop(duration, (t)=>{
                m_sprite.color = Color.Lerp(initColor, targetColor, Easing.SmoothInOut(t));
            });

            if(targetAlpha == 0) m_sprite.enabled = false;
        }
        public static IEnumerator coroutineFadeSpriteColor(SpriteRenderer m_sprite, Color targetColor, float duration){
            Color initColor = m_sprite.color;

            yield return new WaitForLoop(duration, (t)=>{
                m_sprite.color = Color.Lerp(initColor, targetColor, Easing.SmoothInOut(t));
            });
        }
        public static IEnumerator coroutineFadeUI(UnityEngine.UI.MaskableGraphic graphic, float targetAlpha, float duration){
            Color initColor, targetColor;
            initColor = graphic.color;
            targetColor = initColor;
            targetColor.a = targetAlpha;

            yield return new WaitForLoop(duration, (t)=>{
                graphic.color = Color.Lerp(initColor, targetColor, t);
            });
        }
        public static IEnumerator CoroutineDestroyDelay(float delay, GameObject gameObject){
            yield return new WaitForSeconds(delay);
            GameObject.Destroy(gameObject);
        }
        public static IEnumerator CoroutineMover(Transform trans, Vector3 targetPos, float duraiton, Easing.FunctionType ease, bool isLocal = false){
            var easeFunc = Easing.GetFunctionWithTypeEnum(ease);
            Vector3 initPos = isLocal?trans.localPosition:trans.position;
            yield return new WaitForLoop(duraiton, (t)=>{
                if(isLocal) trans.localPosition = Vector3.LerpUnclamped(initPos, targetPos, easeFunc(t));
                else trans.position = Vector3.LerpUnclamped(initPos, targetPos, easeFunc(t));
            });
        }
        public static IEnumerator CoroutineBlinkSprite(SpriteRenderer spriteRenderer, float duration, float blinkFreq){
            yield return new WaitForLoop(duration, (t)=>{
                spriteRenderer.enabled = Mathf.Cos(Mathf.PI*t*duration*blinkFreq)>0?true:false;
            });
            spriteRenderer.enabled = true;
        }
        public static IEnumerator CoroutineBlinkSprite(SpriteRenderer[] spriteRenderers, float duration, float blinkFreq){
            yield return new WaitForLoop(duration, (t)=>{
                for(int i=0; i<spriteRenderers.Length; i++){
                    spriteRenderers[i].enabled = Mathf.Cos(Mathf.PI*t*duration*blinkFreq)>0?true:false;
                }
            });
            for(int i=0; i<spriteRenderers.Length; i++){
                spriteRenderers[i].enabled = true;
            }
        }
    }
}
