using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class ChipComponent : BaseClickComponent
    {
        public override void OnPointerEnter(PointerEventData eventData)
        {
            Pair(GameObject.Find("Main Camera").GetComponent<GameManager>()._blackCells, true);
            
         //Так было, не знаю, что с этим делать
         //CallBackEvent((CellComponent)Pair, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            Pair(GameObject.Find("Main Camera").GetComponent<GameManager>()._blackCells, false);

            //CallBackEvent((CellComponent)Pair, false);
        }

        private void Pair(Transform[] _massiveCells, bool _add)
        {
            float x = gameObject.transform.position.x;
            float z = gameObject.transform.position.z;
            //Debug.Log("x = " + x + " z = " + z);

            foreach (var cell in _massiveCells)
            {
                if ((cell.transform.position.x == x) && (cell.transform.position.z == z))
                {
                    var _cellScript = cell.GetComponent<CellComponent>();
                    if (_cellScript.isChosen == true)
                    {
                        return;
                    }
                    else
                    {
                        if (_add == true)
                        {
                            _cellScript.ChangeMaterial(_chosenOne);
                        }
                        else
                        {
                            _cellScript.ChangeMaterial(_trueColor);
                        }
                    }
                }
            }
        }

        public void OnClickMethod(Transform[] _massiveCells)
        {
            OnClickEventHandler += (_chip) =>
            {
                var _cell = Pair2(GameObject.Find("Main Camera").GetComponent<GameManager>()._blackCells, gameObject.transform);
                var _cellScript = _cell.GetComponent<CellComponent>();
                _cellScript.Click(_massiveCells);
            };
        }

        protected override void Start()
        {
            OnClickMethod(GameObject.Find("Main Camera").GetComponent<GameManager>()._blackCells);
        }
    }
}
