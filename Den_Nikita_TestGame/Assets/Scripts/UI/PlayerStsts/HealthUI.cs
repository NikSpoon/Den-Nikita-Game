using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _text;

    private int _currentHealth;
    private void OnEnable()
    {
        _health.OnHealthChanged += UpdateBar;
    }

    private void OnDisable()
    {
        _health.OnHealthChanged -= UpdateBar;
    }

    private void UpdateBar(int current, int max)
    {
        float fill = (max > 0) ? (float)current / max : 0f;

        if (_image != null)
        {
            _image.fillAmount = fill;
        }
        if (_text != null)
        {
            _text.text = $"Health: {current}/{max}";
        }
        _currentHealth = current;
    }
}