using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class ChipComponent : BaseClickComponent
    {
        public override void OnPointerEnter(PointerEventData eventData)
        {
            //1. Найти координаты X и Z фишки, а затем вызов функции получения пары в GameManager, потому что там _blackCells private.
            var x = gameObject.transform.position.x;
            var z = gameObject.transform.position.z;
            //2. Найти клетку с такими же координатами x и z и вызвать у неё OnPointerEnter.
            Pair(GameObject.Find("Main Camera").GetComponent<GameManager>()._blackCells, x, z, true);
            
         //Так было, не знаю, что с этим делать
         //CallBackEvent((CellComponent)Pair, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            var x = gameObject.transform.position.x;
            var z = gameObject.transform.position.z;
            //2. Найти клетку с такими же координатами x и z и вызвать у неё OnPointerEnter.
            Pair(GameObject.Find("Main Camera").GetComponent<GameManager>()._blackCells, x, z, false);

            //CallBackEvent((CellComponent)Pair, false);
        }

        private void Pair(Transform[] _massiveCells, float x,float z, bool _add)
        {
            foreach (var cell in _massiveCells)
            {
                if ((gameObject.transform.position.x == x) && (gameObject.transform.position.z == z))
                {
                    var _cellScript = cell.GetComponent<CellComponent>();
                    if (_add == true)
                    {
                        _cellScript.SelectMaterial(1);
                    }
                    else 
                    {
                        _cellScript.SelectMaterial(0);
                    }
                    return;
                }
            }
        }
    }
}
