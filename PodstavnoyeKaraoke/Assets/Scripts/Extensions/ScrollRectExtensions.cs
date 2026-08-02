using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Extensions
{
    public static class ScrollRectExtensions
    {
		private static Vector2 CalculateFocusedScrollPosition( this ScrollRect target, Vector2 focusPoint )
		{
			Vector2 contentSize = target.content.rect.size;
			Vector2 viewportSize = ( (RectTransform) target.content.parent ).rect.size;
			Vector2 contentScale = target.content.localScale;

			contentSize.Scale( contentScale );
			focusPoint.Scale( contentScale );

			Vector2 scrollPosition = target.normalizedPosition;
			if( target.horizontal && contentSize.x > viewportSize.x )
				scrollPosition.x = Mathf.Clamp01( ( focusPoint.x - viewportSize.x * 0.5f ) / ( contentSize.x - viewportSize.x ) );
			if( target.vertical && contentSize.y > viewportSize.y )
				scrollPosition.y = Mathf.Clamp01( ( focusPoint.y - viewportSize.y * 0.5f ) / ( contentSize.y - viewportSize.y ) );

			return scrollPosition;
		}

		#region Visible

	    public static bool IsInViewport(this ScrollRect target, RectTransform item)
		{
			if(target.vertical) return target.CheckItemFullyVisibleVertical(item);
			if(target.horizontal) return target.CheckItemFullyVisibleByHorizontal(item);

			return false;
		}
		
		public static bool CheckItemFullyVisibleVertical(this ScrollRect target, RectTransform item)
		{
			RectTransform viewport = (RectTransform)target.viewport;
			Vector2 viewportSize = viewport.rect.size;
			
			Vector2 itemSize = item.rect.size;
			Vector2 itemCenterPoint = target.viewport.InverseTransformPoint( item.transform.TransformPoint( item.rect.center ) );
			
			Vector2 itemPosition = itemCenterPoint;
			
			Vector2 itemScale = item.localScale;
			itemSize.x *= itemScale.x;
			itemSize.y *= itemScale.y;
			
			float itemTop = itemPosition.y + itemSize.y * (1 - item.pivot.y);
			float itemBottom = itemPosition.y - itemSize.y * item.pivot.y;
			
			float viewportTop = viewportSize.y * (1 - viewport.pivot.y);
			float viewportBottom = -viewportSize.y * viewport.pivot.y;
			
			return itemTop < viewportTop && itemBottom > viewportBottom;
		}
		
		public static bool CheckItemFullyVisibleByHorizontal(this ScrollRect target, RectTransform item)
		{
			RectTransform viewport = (RectTransform)target.viewport;
			Vector2 viewportSize = viewport.rect.size * viewport.lossyScale;

			Vector2 itemSize = item.rect.size * item.lossyScale;
			Vector2 itemCenterPoint = target.viewport.InverseTransformPoint( item.transform.position );
			
			Vector2 itemPosition = itemCenterPoint * item.lossyScale;
			
			Vector2 itemScale = item.localScale;
			itemSize.x *= itemScale.x;
			itemSize.y *= itemScale.y;
			
			float itemLeft = itemPosition.x - itemSize.x * item.pivot.x;
			float itemRight = itemPosition.x + itemSize.x * (1 - item.pivot.x);
			
			float viewportLeft = -viewportSize.x * viewport.pivot.x;
			float viewportRight = viewportSize.x * (1 - viewport.pivot.x);
			
			return itemRight < viewportRight && itemLeft > viewportLeft;
		}
		
		public static bool CheckItemVisibleByHorizontal(this ScrollRect target, RectTransform item)
		{
			RectTransform viewport = (RectTransform)target.viewport;
			Vector2 viewportSize = viewport.rect.size * viewport.lossyScale;

			Vector2 itemSize = item.rect.size * item.lossyScale;
			Vector2 itemCenterPoint = target.viewport.InverseTransformPoint(item.transform.position);
			
			Vector2 itemPosition = itemCenterPoint * item.lossyScale;
			
			Vector2 itemScale = item.localScale;
			itemSize.x *= itemScale.x;
			itemSize.y *= itemScale.y;

			float itemLeft = itemPosition.x - (itemSize.x * item.pivot.x);
			float itemRight = itemPosition.x + (itemSize.x * (1 - item.pivot.x));
			
			float viewportLeft = -viewportSize.x * viewport.pivot.x;
			float viewportRight = viewportSize.x * (1 - viewport.pivot.x);
			
			if(itemRight < viewportLeft) return false;
			if(itemLeft >  viewportRight) return false;

			return true;
		}
		
		public static bool CheckItemInVisibleByVertical(this ScrollRect target, RectTransform item)
		{
			RectTransform viewport = (RectTransform)target.viewport;
			Vector2 viewportSize = viewport.rect.size * viewport.lossyScale;

			Vector2 itemSize = item.rect.size * item.lossyScale;
			Vector2 itemCenterPoint = target.viewport.InverseTransformPoint( item.transform.position );
			
			Vector2 itemPosition = itemCenterPoint * item.lossyScale;
			
			Vector2 itemScale = item.localScale;
			itemSize.x *= itemScale.x;
			itemSize.y *= itemScale.y;

			float itemTop = itemPosition.y + itemSize.y * (1 - item.pivot.y);
			float itemDown = itemPosition.y - itemSize.y * item.pivot.y;
			
			float viewportTop = viewportSize.y * (1 - viewport.pivot.y);
			float viewportDown = -viewportSize.y * viewport.pivot.y;
			
			return itemDown > viewportTop || itemTop < viewportDown;
		}

		#endregion
		
		public static string GetClosestEdgeHorizontal(this ScrollRect target, RectTransform item)
		{
			// Получаем размеры области просмотра
			RectTransform viewport = (RectTransform)target.viewport;
			Vector2 viewportSize = viewport.rect.size;
			
			Vector2 itemSize = item.rect.size;
			Vector2 itemCenterPoint = target.viewport.InverseTransformPoint( item.transform.TransformPoint( item.rect.center ) );
			Vector2 itemPosition = itemCenterPoint;
			//itemPosition.Scale( viewport.lossyScale );
			
			float itemLeft = itemPosition.x - itemSize.x * item.pivot.x;
			float itemRight = itemPosition.x + itemSize.x * (1 - item.pivot.x);
			
			float viewportLeft = -viewportSize.x * viewport.pivot.x;
			float viewportRight = viewportSize.x * (1 - viewport.pivot.x);
			
			float distanceToLeftEdge = Mathf.Abs(itemLeft - viewportLeft);
			float distanceToRightEdge = Mathf.Abs(itemRight - viewportRight);
			
			if (distanceToLeftEdge < distanceToRightEdge)
			{
				return "left";
			}
			else
			{
				return "right";
			}
		}
		
		public static RectTransform GetNearestItem(this ScrollRect target)
		{
			RectTransform viewport = (RectTransform)target.viewport;
			Vector2 viewportCenter = target.transform.position;

			RectTransform nearestItem = null;
			float closestDistance = float.MaxValue;

			// Получаем все RectTransform элементы внутри content
			List<RectTransform> items = new List<RectTransform>();

			for (int i = 0; i < target.content.childCount; i++)
			{
				var rectTransform = target.content.GetChild(i).GetComponent<RectTransform>();
				items.Add(rectTransform);
			}

			foreach (var item in items)
			{
				// Проверяем, что элемент является дочерним элементом content
				if (item != target.content)
				{
					// Получаем центр элемента
					Vector2 itemCenter = item.position;

					// Вычисляем расстояние до центра элемента
					float distance = Vector2.Distance(viewportCenter, itemCenter);

					if (distance < closestDistance)
					{
						closestDistance = distance;
						nearestItem = item;
					}
				}
			}

			return nearestItem;
		}

		public static Vector2 CalculateFocusedScrollPosition( this ScrollRect target, RectTransform item )
		{
			Vector2 itemCenterPoint = target.content.InverseTransformPoint( item.transform.TransformPoint( item.rect.center ) );

			Vector2 contentSizeOffset = target.content.rect.size;
			contentSizeOffset.Scale( target.content.pivot );

			return target.CalculateFocusedScrollPosition( itemCenterPoint + contentSizeOffset );
		}
		
		public static Vector2 CalculateFocusedLeftScrollPosition(this ScrollRect target, RectTransform item)
		{
			Vector2 itemLeftPoint = target.content.InverseTransformPoint(item.transform.TransformPoint(new Vector2(-item.rect.width * item.pivot.x, 0)));
			float normalizedPositionX = Mathf.Clamp01((itemLeftPoint.x) / (target.content.rect.width - ((RectTransform)target.content.parent).rect.width));
			return new Vector2(normalizedPositionX, target.normalizedPosition.y);
		}
		
		public static Vector2 CalculateFocusedRightScrollPosition (this ScrollRect target, RectTransform item)
		{
			Vector2 itemRightPoint = target.content.InverseTransformPoint(item.transform.TransformPoint(new Vector2(item.rect.width * (item.pivot.x), 0)));
			float normalizedPositionX = Mathf.Clamp01((itemRightPoint.x) / (target.content.rect.width - ((RectTransform)target.content.parent).rect.width));
			return new Vector2(normalizedPositionX, target.normalizedPosition.y);
		}

		public static void FocusAtPoint( this ScrollRect target, Vector2 focusPoint )
		{
			target.normalizedPosition = target.CalculateFocusedScrollPosition( focusPoint );
		}

		public static void FocusOnItem( this ScrollRect target, RectTransform item )
		{
			target.normalizedPosition = target.CalculateFocusedScrollPosition( item );
		}

		private static IEnumerator LerpToScrollPositionCoroutine( this ScrollRect target, Vector2 targetNormalizedPos, float speed )
		{
			Vector2 initialNormalizedPos = target.normalizedPosition;

			float t = 0f;
			while( t < 1f )
			{
				target.normalizedPosition = Vector2.LerpUnclamped( initialNormalizedPos, targetNormalizedPos, 1f - ( 1f - t ) * ( 1f - t ) );

				yield return null;
				t += speed * Time.unscaledDeltaTime;
			}

			target.normalizedPosition = targetNormalizedPos;
		}

		public static IEnumerator FocusAtPointCoroutine( this ScrollRect target, Vector2 focusPoint, float speed )
		{
			yield return target.LerpToScrollPositionCoroutine( target.CalculateFocusedScrollPosition( focusPoint ), speed );
		}

		public static IEnumerator FocusOnItemCoroutine( this ScrollRect target, RectTransform item, float speed )
		{
			yield return target.LerpToScrollPositionCoroutine( target.CalculateFocusedScrollPosition( item ), speed );
		}

		public static LTDescr ScrollToItem(this ScrollRect target, RectTransform item, float time)
		{
			var currentNormalizedPosition = target.normalizedPosition;
			var needNormalizedPosition = CalculateFocusedScrollPosition(target,item);

			return LeanTween.value(target.gameObject, currentNormalizedPosition, needNormalizedPosition, time).setOnUpdate(
				(Vector2 v) =>
				{
					target.normalizedPosition = v;
				});
		}
		
		public static LTDescr ScrollToNormalizedPositionX(this ScrollRect target, float normalizedPositionX, float time)
		{
			var currentNormalizedPosition = target.normalizedPosition;
			var needNormalizedPosition = currentNormalizedPosition;
			needNormalizedPosition.x = normalizedPositionX;

			return LeanTween.value(target.gameObject, currentNormalizedPosition, needNormalizedPosition, time).setOnUpdate(
				(Vector2 v) =>
				{
					target.normalizedPosition = v;
				});
		}
		
		public static void ScrollToNormalizedPositionX(this ScrollRect target, float normalizedPositionX)
		{
			var currentNormalizedPosition = target.normalizedPosition;
			currentNormalizedPosition.x = normalizedPositionX;
			target.normalizedPosition = currentNormalizedPosition;
		}
    }
}