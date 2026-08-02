using System;
using Controllers;
using UnityEngine;
using Utilities;

namespace Extensions
{
    public static class GameObjectExtensions
    {
        public static Vector2 Size(this GameObject target)
        {
            RectTransform rectTransform = target.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                Log.Assert("not RectTransform component");
                return Vector2.zero;
            }

            return rectTransform.sizeDelta;
        }

        public static Vector3 GetPositionOffScreenByDirection(this GameObject target, Vector2 direction, bool needConvertToLocalPosition = false)
        {
            Vector3 position = target.transform.position;
            
            if (direction == Vector2.left)
                position.x = -MainController.Instance.GameSettings.DefaultResolutionWidth / 2f - target.Size().x / 2f;
            else if (direction == Vector2.up)
                position.y = MainController.Instance.GameSettings.DefaultResolutionHeight / 2f + target.Size().y / 2f;
            else if (direction == Vector2.right)
                position.x = MainController.Instance.GameSettings.DefaultResolutionWidth / 2f + target.Size().x / 2f;
            else if (direction == Vector2.down)
                position.y = -MainController.Instance.GameSettings.DefaultResolutionHeight / 2f - target.Size().y / 2f;

            if (needConvertToLocalPosition)
                return target.transform.parent.InverseTransformPoint(position);
            else
                return position;
        }

        public static bool CheckMouseClickOnObject(this GameObject target)
        {
            Vector3 mousePosition = MainController.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);

            Vector3 targetPosition = target.transform.position;
            Vector3 targetSize = target.Size();

            float top = targetPosition.y + (targetSize.y / 2f);
            float bottom = targetPosition.y - (targetSize.y / 2f);
            float left = targetPosition.x - (targetSize.x / 2f);
            float right = targetPosition.x + (targetSize.x / 2f);

            return (mousePosition.y <= top & mousePosition.y >= bottom & mousePosition.x >= left &
                    mousePosition.x <= right);
        }

        public static LTDescr RotationZ(this GameObject target,float from,float to, float time)
        {
            return LeanTween.value(target, from, to, time).setOnUpdate(target.SetEulerAnglesZ);
        }

        public static void SetEulerAnglesZ(this GameObject target, float z)
        {
            var current = target.transform.rotation.eulerAngles;
            current.z = z;
            target.transform.eulerAngles = current;
        }

        public static void StartScaleAnimation(this GameObject target,float delta, float time,Action onCompletedUp = null, Action onCompletedDown = null)
        {
            var currentScale = target.transform.localScale;
            var needScale = currentScale.AddDelta(delta);

            target.LeanScale(needScale, time).setOnComplete(() =>
            {
                onCompletedUp?.Invoke();
                
                target.LeanScale(currentScale, time).setOnComplete(() =>
                {
                    onCompletedDown?.Invoke();
                });
            });
        }

        #region SHOW AND HIDE

        public static void Show(this GameObject target, float delay = 0f, float time = 0.3f,Action onCompleted = null, LeanTweenType ease = LeanTweenType.easeOutBack)
        {
            target.SetActive(true);
            
            var initPosition = MainController.Instance.GetGameObjectInfo(target).position;

            target.LeanMove(initPosition, time).setDelay(delay).setEase(ease).setOnComplete(() =>
            {
                onCompleted?.Invoke();
            });
        }
        
        public static void Hide(this GameObject target, Vector2 direction, bool smoothly, float time = 0.3f,float delay = 0f,Action onCompleted = null, LeanTweenType ease = LeanTweenType.easeInBack)
        {
            var hidePosition = target.GetPositionOffScreenByDirection(direction);
            
            if (smoothly)
            {
                target.LeanMove(hidePosition, time).setDelay(delay).setEase(ease).setOnComplete(
                    () =>
                    {
                        target.SetActive(false);
                        onCompleted?.Invoke();
                    });
            }
            else
            {
                target.transform.position = hidePosition;
                target.SetActive(false);
                onCompleted?.Invoke();
            }
        }

        #endregion

        #region SET LOCAL POSITION

        public static void SetLocalX(this GameObject target, float x)
        {
            Vector3 position = target.transform.localPosition;
            position.x = x;
            target.transform.localPosition = position;
        }
        
        public static void SetLocalY(this GameObject target, float y)
        {
            Vector3 position = target.transform.localPosition;
            position.y = y;
            target.transform.localPosition = position;
        }
        
        public static void SetLocalZ(this GameObject target, float z)
        {
            Vector3 position = target.transform.localPosition;
            position.z = z;
            target.transform.localPosition = position;
        }
        #endregion

        #region SET GLOBAL POSITION

        public static void SetGlobalX(this GameObject target, float x)
        {
            Vector3 position = target.transform.position;
            position.x = x;
            target.transform.position = position;
        }
        
        public static void SetGlobalY(this GameObject target, float y)
        {
            Vector3 position = target.transform.position;
            position.y = y;
            target.transform.position = position;
        }
        
        public static void SetGlobalZ(this GameObject target, float z)
        {
            Vector3 position = target.transform.position;
            position.z = z;
            target.transform.position = position;
        }

        #endregion

        #region SET SCALE

        public static void SetScaleX(this GameObject target, float x)
        {
            Vector3 scale = target.transform.localScale;
            scale.x = x;
            target.transform.localScale = scale;
        }
        
        public static void SetScaleY(this GameObject target, float y)
        {
            Vector3 scale = target.transform.localScale;
            scale.y = y;
            target.transform.localScale = scale;
        }

        #endregion
        
    }
}
