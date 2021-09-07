using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace Checkers
{
    public class CellComponent : BaseClickComponent
    {
        private Dictionary<NeighborType, CellComponent> _neighbors;

        /// <summary>
        /// Возвращает соседа клетки по указанному направлению
        /// </summary>
        /// <param name="type">Перечисление направления</param>
        /// <returns>Клетка-сосед или null</returns>
        public CellComponent GetNeighbors(NeighborType type) => _neighbors[type];

        protected override void Start()
        {
            BaseMeshMethod();

            return;
            OnFocusEventHandler += (_cell, isSelect) =>
            {
                SelectMaterial(1);
              //  AddAdditionalMaterial(_chosenOne);
                Debug.Log("onFocusMethod");
            };
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            SelectMaterial(1);
            CallBackEvent(this, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            SelectMaterial(0);
            CallBackEvent(this, false);
        }

        /// <summary>
        /// Конфигурирование связей клеток
        /// </summary>
		public void Configuration(Dictionary<NeighborType, CellComponent> neighbors)
		{
            if (_neighbors != null) return;
            _neighbors = neighbors;
		}
       public void OnFocusMethod()
        {
            OnFocusEventHandler += (_cell, isSelect) =>
            {
                AddAdditionalMaterial(_chosenOne, 2);
                Debug.Log("onFocusMethod");
            };
        }
    }

    /// <summary>
    /// Тип соседа клетки
    /// </summary>
    public enum NeighborType : byte
    {
        /// <summary>
        /// Клетка сверху и слева от данной
        /// </summary>
        TopLeft,
        /// <summary>
        /// Клетка сверху и справа от данной
        /// </summary>
        TopRight,
        /// <summary>
        /// Клетка снизу и слева от данной
        /// </summary>
        BottomLeft,
        /// <summary>
        /// Клетка снизу и справа от данной
        /// </summary>
        BottomRight
    }
}