using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.BonusesWindow.BonusItem
{
    public class BonusItemLine : MonoBehaviour
    {
        [SerializeField] private TMP_Text _leftText;
        [SerializeField] private TMP_Text _rightText;
        
        [SerializeField] private Graphic[] _graphicToColor;

        public string LeftText { set => _leftText.text = value; }

        public string RightText { set => _rightText.text = value; }

        public Color Color 
        { 
            set
            {
                foreach (var graphic in _graphicToColor)
                    graphic.color = value;
            }
        }
    }
}