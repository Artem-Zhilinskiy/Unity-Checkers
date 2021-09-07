using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Checkers
{
    public abstract class BaseClickComponent : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Tooltip("Цвет выбранного компонента"), SerializeField]
        public Material _chosenOne;
        [Tooltip("Цвет соседних компонентов"), SerializeField]
        public Material _neighbours;
        [Tooltip("Изначальный цвет клетки"), SerializeField]
        public Material _trueColor;
        //Меш игрового объекта
        private MeshRenderer _mesh;
        //Список материалов на меше объекта
        protected Material[] _meshMaterials = new Material[3];


        [Tooltip("Цветовая сторона игрового объекта"), SerializeField]
        private ColorType _color;

        /// <summary>
        /// Возвращает цветовую сторону игрового объекта
        /// </summary>
        public ColorType GetColor => _color;

        /// <summary>
        /// Возвращает или устанавливает пару игровому объекту
        /// </summary>
        /// <remarks>У клеток пара - фишка, у фишек - клетка</remarks>
       // public BaseClickComponent Pair { get; set; }

        protected bool PairBool (BaseClickComponent _component, Transform[] _massiveComponents)
        {
            float x1 = _component.gameObject.transform.position.x;
            float z1 = _component.gameObject.transform.position.z;
            foreach (var _pairComponent in _massiveComponents)
            {
                float x2 = _pairComponent.transform.position.x;
                float z2 = _pairComponent.transform.position.z;
                if ((x1 == x2) && (z1 == z2))
                {
                    return true;
                }
            }
            return false;
        }
        public Transform Pair2(Transform[] _massiveCells, Transform Chip)
        {
            float x = Chip.position.x;
            float z = Chip.position.z;
            //Debug.Log("x = " + x + " z = " + z);
            foreach (var cell in _massiveCells)
            {
                if ((cell.transform.position.x == x) && (cell.transform.position.z == z))
                {
                    return cell;
                }
            }
            return null;
        }

        public void ChangeMaterial(Material material)
        {
            //Debug.Log("material is changing");
            _mesh = GetComponent<MeshRenderer>();
            _mesh.material = material;
        }

        /// <summary>
        /// Событие клика на игровом объекте
        /// </summary>
        public event ClickEventHandler OnClickEventHandler;

        /// <summary>
        /// Событие наведения и сброса наведения на объект
        /// </summary>
        public event FocusEventHandler OnFocusEventHandler;


        //При навадении на объект мышки, вызывается данный метод
        //При наведении на фишку, должна подсвечиваться клетка под ней
        //При наведении на клетку - подсвечиваться сама клетка
        public abstract void OnPointerEnter(PointerEventData eventData);

        //Аналогично методу OnPointerEnter(), но срабатывает когда мышка перестает
        //указывать на объект, соответственно нужно снимать подсветку с клетки
        public abstract void OnPointerExit(PointerEventData eventData);

        //При нажатии мышкой по объекту, вызывается данный метод
        public void OnPointerClick(PointerEventData eventData)
		{
            OnClickEventHandler?.Invoke(this);
        }

        //Этот метод можно вызвать в дочерних классах (если они есть) и тем самым пробросить вызов
        //события из дочернего класса в родительский
        protected void CallBackEvent(CellComponent target, bool isSelect)
        {
            OnFocusEventHandler?.Invoke(target, isSelect);
		}
        protected virtual void Start()
        {

        }
    }

    public enum ColorType
    {
        White,
        Black
    }

    public delegate void ClickEventHandler(BaseClickComponent component);

    public delegate void FocusEventHandler(CellComponent component, bool isSelect);
}