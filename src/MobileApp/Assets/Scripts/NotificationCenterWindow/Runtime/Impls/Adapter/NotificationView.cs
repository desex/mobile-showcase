using System;
using Sotbify.MobileApp.Localization;
using Sotbify.MobileApp.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.NotificationCenterWindow
{
    public class NotificationView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private LocalizationVariableEntry<DateTimeVariable> _createdAtVariableEntry;
        [SerializeField] private Image _notificationCategoryImage;

        public string Title
        {
            get => _title != null ?  _title.text : string.Empty;
            set => _title.SetText(value);
        }

        public string Description
        {
            get => _description != null ?  _description.text : string.Empty;
            set => _description.SetText(value);
        }

        public DateTime CreatedAt
        {
            get => _createdAtVariableEntry?.Variable?.Value ?? DateTime.MinValue;
            set
            {
                if(_createdAtVariableEntry?.Variable is null)
                    return;
                
                _createdAtVariableEntry.Variable.Value = value;
            }
        }

        public Sprite CategoryImage
        {
            get => _notificationCategoryImage != null ?  _notificationCategoryImage.sprite : null;
            set
            {
                if(_notificationCategoryImage != null)
                    _notificationCategoryImage.sprite = value;
            }
        }
    }
}