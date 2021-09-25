using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

namespace Checkers
{
    public class CellComponent : BaseClickComponent
    {
        public bool isChosen = false;
        public bool isNeighbour = false;
        //private Dictionary<NeighborType, CellComponent> _neighbors;
        Dictionary<NeighborType, CellComponent> _neighbors = new Dictionary<NeighborType, CellComponent>();




        public GameManager manager;
        public ObserverComponent observer;
        /// <summary>
        /// Возвращает соседа клетки по указанному направлению
        /// </summary>
        /// <param name="type">Перечисление направления</param>
        /// <returns>Клетка-сосед или null</returns>
        public CellComponent GetNeighbors(NeighborType type) => _neighbors[type];

        protected override void Start()
        {
            manager = gameObject.AddComponent<GameManager>();
            manager.enabled = false;
            observer = gameObject.AddComponent<ObserverComponent>();
            observer.enabled = false;
            //Заполнение словаря клеток-соседок.
            EnlistDicNeighbour(_neighbors);
            OnFocusMethod();
            OnClickMethod(GameObject.Find("Main Camera").GetComponent<GameManager>()._blackCells);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            CallBackEvent(this, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
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
                if ((isChosen == false) && (isNeighbour == false))
                {
                    if (isSelect == true) 
                    {
                        ChangeMaterial(_chosenOne);
                        //Debug.Log("onFocusMethod Enter");
                    }
                    if (isSelect == false)
                    {
                        ChangeMaterial(_trueColor);
                        //Debug.Log("onFocusMethod Exit");
                    }
                }
            };
            OnFocusEventHandler += (_cell, isSelect) =>
            {
                //write to file
            };
       }
        public void OnClickMethod(Transform[] _massiveCells)
        {
            OnClickEventHandler += (_cell) =>
            {
                Click(_massiveCells);
            };
        }

        public void Click(Transform[] _massiveCells)
        {
            if (GameObject.Find("Main Camera").GetComponent<GameManager>()._isInputBlocked == false)
            {
                //Перемещение фишки
                //Если произошёл щелчок по подсвеченной клетке
                //if (_mesh.material == _neighbours)
                if (isNeighbour == true)
                {
                    //Запомнить её координаты
                    float x = this.gameObject.transform.position.x;
                    float z = this.gameObject.transform.position.z;
                    //Запоминаем её имя для observer
                    string _targetCell= this.gameObject.name;
                    foreach (var _cellChosen in _massiveCells)
                    {
                        //Найти выделенную клетку с материалом _chosenOne
                        //var _mesh2 = _cellChosen.GetComponent<MeshRenderer>();
                        var _cellChosenScript = _cellChosen.GetComponent<CellComponent>();
                        if (_cellChosenScript.isChosen == true)
                        {
                            //Найти фишку, которая стоит на выделенной клетке с материалом _chosenOne
                            var _chipChosen = Pair2(GameObject.Find("Main Camera").GetComponent<GameManager>()._whiteChips, _cellChosen);
                            if (_chipChosen == null)
                            {
                                _chipChosen = Pair2(GameObject.Find("Main Camera").GetComponent<GameManager>()._blackChips, _cellChosen);
                            }
                            //Записать координаты фишки, которая сейчас ходит. (Стоит на выделенной клетке)
                            float x0 = _chipChosen.position.x;
                            float z0 = _chipChosen.position.z;
                            //Записать имя этой клетки для observer
                            string _exodusCell = _cellChosen.gameObject.name;
                            //Вывести запись о передвижении фишки в лог observer
                            if (GameObject.Find("Main Camera").GetComponent<ObserverComponent>().isRecorded == true)
                            {
                                observer.WriteToLog("Передвижение фишки с клетки " + _exodusCell + " на клетку " + _targetCell); //Запись вида: передвижение фишки с клетки A3 на клетку B4
                            }
                            //Определить, съедают ли какую-то фишку
                            Transform _chipToDestroy = null;
                            if (Math.Abs(x - x0) == 2)
                            {
                                //Найти координаты фишки, которую съедают
                                float xD;
                                float zD;
                                if (x > x0) xD = x0 + 1; else xD = x0 - 1;
                                if (z > z0) zD = z0 + 1; else zD = z0 - 1;
                                //Найти Transform этой фишки

                                foreach (var _chipDestroy in GameObject.Find("Main Camera").GetComponent<GameManager>()._whiteChips)
                                {
                                    if ((_chipDestroy.position.x == xD) && (_chipDestroy.position.z == zD))
                                    {
                                        _chipToDestroy = _chipDestroy;
                                    }
                                }
                                if (_chipToDestroy == null)
                                {
                                    foreach (var _chipDestroy in GameObject.Find("Main Camera").GetComponent<GameManager>()._blackChips)
                                    {
                                        if ((_chipDestroy.position.x == xD) && (_chipDestroy.position.z == zD))
                                        {
                                            _chipToDestroy = _chipDestroy;
                                        }
                                    }
                                }
                            }
                            //Подвинуть фишку на подсвеченную клетку
                            //manager.MoveChip(_chipChosen,_chipToDestroy, this.gameObject.transform);
                            manager.MoveChip(_chipChosen, _chipToDestroy, x, z);

                            //Запись в лог о съедении фишки
                            if (GameObject.Find("Main Camera").GetComponent<ObserverComponent>().isRecorded == true && _chipToDestroy != null)
                            {
                                //Найти имя клетки, на которой стоит съедаемая фишка
                                foreach (var _cell in GameObject.Find("Main Camera").GetComponent<GameManager>()._blackCells)
                                {
                                    if ((_cell.position.x == _chipToDestroy.position.x) && (_cell.position.z == _chipToDestroy.position.z))
                                    {
                                        observer.WriteToLog("Съедена фишка на клетке " + _cell.gameObject.name);
                                    }
                                }
                            }

                            //Снять выделения клеток
                            UnCheck(_massiveCells);
                            //Смена хода
                            if (GameObject.Find("Main Camera").GetComponent<GameManager>()._isWhiteMove == true)
                            {
                                GameObject.Find("Main Camera").GetComponent<GameManager>()._isWhiteMove = false;
                                //Запись в observer о смене хода
                                if (GameObject.Find("Main Camera").GetComponent<ObserverComponent>().isRecorded == true)
                                {
                                    observer.WriteToLog("Ход чёрных");
                                }
                            }
                            else
                            {
                                GameObject.Find("Main Camera").GetComponent<GameManager>()._isWhiteMove = true;
                                //Запись в observer о смене хода
                                if (GameObject.Find("Main Camera").GetComponent<ObserverComponent>().isRecorded == true)
                                {
                                    observer.WriteToLog("Ход белых");
                                }
                            }
                            return;
                        }
                    }
                }
                else
                {
                    //Сигнал в observer о записи действия в лог
                    if (GameObject.Find("Main Camera").GetComponent<ObserverComponent>().isRecorded == true)
                    {
                        observer.WriteToLog("Выбрана клетка " + gameObject.name);
                    }
                    //Снять предыдущие выделения клеток
                    //Добавить проверку на цвет выделения. Нельзя выделить фишку не того игрока, чей сейчас ход и клетку вообще без фишки!
                    if (((GameObject.Find("Main Camera").GetComponent<GameManager>()._isWhiteMove == true) && (PairBool(this, GameObject.Find("Main Camera").GetComponent<GameManager>()._whiteChips))) || (((GameObject.Find("Main Camera").GetComponent<GameManager>()._isWhiteMove) == false) && (PairBool(this, GameObject.Find("Main Camera").GetComponent<GameManager>()._blackChips) == true)))
                    {
                        UnCheck(_massiveCells);
                        //Выделить эту клетку
                        isChosen = true;
                        ChangeMaterial(_chosenOne);

                        //Выделение соседних клеток для хода
                        ClickNeighbour();
                    }
                    else
                    {
                        Debug.Log("Ходит другой игрок или выбрана пустая клетка"); //Запись в лог о выделении клетки
                    }
                }
            }
        }

        private void EnlistDicNeighbour(Dictionary<NeighborType, CellComponent> _neighbors)
        {
            float x = this.transform.position.x;
            float z = this.transform.position.z;
            foreach (var _cell in GameObject.Find("Main Camera").GetComponent<GameManager>()._blackCells)
            {
                var _cellScript = _cell.GetComponent<CellComponent>();
                //Верхняя левая клетка
                if ((_cell.transform.position.x == x-1) && (_cell.transform.position.z == z+1))
                {
                    _neighbors.Add(NeighborType.TopLeft, _cellScript);
                }
                //Верхняя правая клетка
                if ((_cell.transform.position.x == x + 1) && (_cell.transform.position.z == z + 1))
                {
                    _neighbors.Add(NeighborType.TopRight, _cellScript);
                }
                //Нижняя левая клетка
                if ((_cell.transform.position.x == x - 1) && (_cell.transform.position.z == z - 1))
                {
                    _neighbors.Add(NeighborType.BottomLeft, _cellScript);
                }
                //Нижняя правая клетка
                if ((_cell.transform.position.x == x + 1) && (_cell.transform.position.z == z - 1))
                {
                    _neighbors.Add(NeighborType.BottomRight, _cellScript);
                }
                //Верхняя левая клетка 2 радиуса
                if ((_cell.transform.position.x == x - 2) && (_cell.transform.position.z == z + 2))
                {
                    _neighbors.Add(NeighborType.TopTopLeftLeft, _cellScript);
                }
                //Верхняя правая клетка 2 радиуса
                if ((_cell.transform.position.x == x + 2) && (_cell.transform.position.z == z + 2))
                {
                    _neighbors.Add(NeighborType.TopTopRightRight, _cellScript);
                }
                //Нижняя левая клетка 2 радиуса
                if ((_cell.transform.position.x == x - 2) && (_cell.transform.position.z == z - 2))
                {
                    _neighbors.Add(NeighborType.BottomBottomLeftLeft, _cellScript);
                }
                //Нижняя правая клетка
                if ((_cell.transform.position.x == x + 2) && (_cell.transform.position.z == z - 2))
                {
                    _neighbors.Add(NeighborType.BottomBottomRightRight, _cellScript);
                }
            }
        }

        //Снять выделение клеток
        private void UnCheck(Transform[] _massiveCells)
        {
            foreach (var cell in _massiveCells)
            {

                var _cellScript = cell.GetComponent<CellComponent>();
                if (_cellScript.isChosen == true) _cellScript.isChosen = false;
                if (_cellScript.isNeighbour == true) _cellScript.isNeighbour = false;
                _cellScript.ChangeMaterial(_trueColor);
            }
        }

        private void ClickNeighbour()
        {
            //Ход белых
            if (GameObject.Find("Main Camera").GetComponent<GameManager>()._isWhiteMove == true)
            {
                ClickForward(NeighborType.TopLeft);
                ClickForward(NeighborType.TopRight);
                ClickBack(NeighborType.BottomLeft);
                ClickBack(NeighborType.BottomRight);
            }
            else //Ход чёрных
            {
                ClickForward(NeighborType.BottomLeft);
                ClickForward(NeighborType.BottomRight);
                ClickBack(NeighborType.TopLeft);
                ClickBack(NeighborType.TopRight);
            }
        }
        private void ClickForward(NeighborType neighborType)
        {
            CellComponent _cellScript;
            _neighbors.TryGetValue(neighborType, out _cellScript);
            if (_cellScript == null) return;
            //2 условия порверить:
            //1. Если TopLeft, т.е. _cell свободна, то подсветка TopLeft.
            //2. Если TopLeft, т.е. _cell занята фишкой другого цвета, и TopTopLeftLeft свободна, то подстветка TopTopLeftLeft

            //1. Если TopLeft, т.е. _cell свободна, то подсветка TopLeft.
            if (!PairBool(_cellScript, GameObject.Find("Main Camera").GetComponent<GameManager>()._whiteChips) && (!PairBool(_cellScript, GameObject.Find("Main Camera").GetComponent<GameManager>()._blackChips)))
            {
                var _mesh = _cellScript.gameObject.GetComponent<MeshRenderer>();
                _mesh.material = _neighbours;
                _cellScript.isNeighbour = true;
            }
            else
            {
                ClickBack(neighborType);
            }
        }
        private void ClickBack(NeighborType neighborType)
        {
            CellComponent _cellScript;
            _neighbors.TryGetValue(neighborType, out _cellScript);
            if (_cellScript == null) return;
            //Проверить, занята ли _cell фишкой другого цвета
            //Получим Transform фишки, которая занимает текущую клетку и определим её цвет
            bool _chipColorWhiteThisCell = true;
            Transform _chip = Pair2(GameObject.Find("Main Camera").GetComponent<GameManager>()._whiteChips, this.gameObject.transform);
            if (_chip == null)
            {
                _chipColorWhiteThisCell = false;
            }
            //Определим цвет фишки, которая заняла клетку _cell
            bool _chipColorWhite_cell = true;
            Transform _chip2 = Pair2(GameObject.Find("Main Camera").GetComponent<GameManager>()._whiteChips, _cellScript.gameObject.transform);
            if (_chip2 == null)
            {
                _chip = Pair2(GameObject.Find("Main Camera").GetComponent<GameManager>()._blackChips, _cellScript.gameObject.transform);
                if (_chip == null)
                {
                    return;
                }
                _chipColorWhite_cell = false;
            }
            //Сравниваем цвета, и если они разные, проверяем клетку TopTopLeftLeft
            if (!(_chipColorWhiteThisCell == _chipColorWhite_cell))
            {
                CellComponent _cellScript2;
                _neighbors.TryGetValue(neighborType + 4, out _cellScript2);
                if (_cellScript2 == null) return;
                //Свободна ли клетка TopTopLeftLeft?
                if (!PairBool(_cellScript2, GameObject.Find("Main Camera").GetComponent<GameManager>()._whiteChips) && (!PairBool(_cellScript2, GameObject.Find("Main Camera").GetComponent<GameManager>()._blackChips)))
                {
                    var _mesh = _cellScript2.gameObject.GetComponent<MeshRenderer>();
                    _mesh.material = _neighbours;
                    _cellScript2.isNeighbour = true;
                }
            }
        }
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
        BottomRight,
        /// <summary>
        /// Клетка сверху и слева от верхней левой клетки
        /// </summary>
        TopTopLeftLeft,
        /// <summary>
        /// Клетка сверху и справа от верхней правой клетки
        /// </summary>
        TopTopRightRight,
        /// <summary>
        /// Клетка снизу и слева от нижней левой клетки
        /// </summary>
        BottomBottomLeftLeft,
        /// <summary>
        /// Клетка снизу и справа от нижней правой клетки
        /// </summary>
        BottomBottomRightRight
    }